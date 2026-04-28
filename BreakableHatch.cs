using System;
using DG.Tweening;
using UnityEngine;

public class BreakableHatch : Breakable
{
	private const float SWITCH_HIT_ROTATION = 25f;

	private const float SWITCH_HIT_ROTATION_SPEED = 0.1f;

	private const float SWITCH_OPEN_ROTATION = 40f;

	private const float SWITCH_OPEN_ROTATION_SPEED = 0.5f;

	private const int HIT_LIMIT = 3;

	[Header("Switch")]
	[SerializeField]
	private Transform m_Switch;

	[SerializeField]
	private Transform m_SwitchEndLocation;

	[SerializeField]
	private ParticleSystem m_Sparks;

	[SerializeField]
	private Renderer[] m_SwitchRenderers;

	[Header("Chain")]
	[SerializeField]
	private Transform m_Chain;

	[SerializeField]
	private Transform m_ChainEndLocation;

	[Header("Hatch")]
	[SerializeField]
	private Transform m_Hatch;

	[SerializeField]
	private Transform m_HatchEndLocation;

	[Header("Sparkles")]
	[SerializeField]
	private GameObject m_Sparkles;

	private Sequence m_Sequence;

	private int m_HitCount;

	public bool IsOpen => m_HitCount >= 3;

	public event EventHandler OnSwitchHit;

	public event EventHandler OnSwitchBroken;

	public event EventHandler OnSwitchOpenStart;

	public event EventHandler OnSwitchOpen;

	public event EventHandler OnSwitchOpenEnd;

	public override void Start()
	{
		if (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty != DifficultyLevel.Easy && GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty != DifficultyLevel.Normal)
		{
			m_Sparkles.SetActive(value: false);
		}
	}

	protected override void InternalHit(RaycastHit hit)
	{
		if ((bool)m_Switch || (bool)m_SwitchEndLocation || (bool)m_Chain || (bool)m_ChainEndLocation || (bool)m_Hatch || (bool)m_HatchEndLocation)
		{
			if (!HitSwitchEffects())
			{
				if (IsOpen)
				{
					OpenHatch();
				}
				else
				{
					HitSwitch();
				}
			}
		}
		else
		{
			HitOnComplete();
		}
	}

	private bool HitSwitchEffects()
	{
		if (IsOpen)
		{
			m_Sparks.Emit(1);
			CameraEffects.ShakeRotation(0.4f, 1f);
			this.OnSwitchBroken.Send(this);
			return true;
		}
		m_Sparks.Emit(2);
		CameraEffects.ShakeRotation(0.3f, 1.75f);
		m_HitCount++;
		this.OnSwitchHit.Send(this);
		return false;
	}

	private void HitSwitch()
	{
		ResetSequence();
		m_Sequence.Insert(0f, m_Switch.DOLocalRotate(new Vector3(25f, 0f, 0f), 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
		object[] switchRenderers = m_SwitchRenderers;
		ShaderEffects.Fade("_PostHitGlow", 1f, 0f, 0.4f, switchRenderers);
	}

	private void OpenHatch()
	{
		this.OnSwitchOpenStart.Send(this);
		m_Sparkles.SetActive(value: false);
		ResetSequence();
		float num = 0f;
		m_Sequence.Insert(num, m_Switch.DOLocalRotate(new Vector3(40f, 0f, 0f), 0.5f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBounce));
		num += 0.15f;
		Vector3 endValue = (m_Chain.localPosition + m_ChainEndLocation.localPosition) / 2f;
		m_Sequence.Insert(num, m_Chain.DOLocalMove(endValue, 1.5f).SetEase(Ease.InSine));
		num += 0.5f;
		m_Sequence.InsertCallback(num, base.HitOnComplete);
		num += 1f;
		m_Sequence.Insert(num, m_Chain.DOLocalMove(m_ChainEndLocation.localPosition, 1.5f).SetEase(Ease.OutBounce));
		m_Sequence.Insert(num, m_Hatch.DOLocalRotate(m_HatchEndLocation.localEulerAngles, 1.5f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBounce));
		m_Sequence.InsertCallback(num, delegate
		{
			this.OnSwitchOpen.Send(this);
		});
		num += 1f;
		m_Sequence.InsertCallback(num, delegate
		{
			this.OnSwitchOpenEnd.Send(this);
		});
		SendOnBroken();
	}

	protected override void InternalForceComplete()
	{
		m_Sparkles.SetActive(value: false);
		m_Switch.localEulerAngles = new Vector3(90f, 0f, 0f);
		m_Chain.localPosition = (m_Chain.localPosition + m_ChainEndLocation.localPosition) / 2f;
		m_Chain.localPosition = m_ChainEndLocation.localPosition;
		m_Hatch.localEulerAngles = m_HatchEndLocation.localEulerAngles;
		m_HitCount = 3;
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			m_Sequence.Kill();
			m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		this.OnSwitchHit = null;
		this.OnSwitchBroken = null;
		this.OnSwitchOpenStart = null;
		this.OnSwitchOpen = null;
		this.OnSwitchOpenEnd = null;
		KillSequence();
		base.OnDisposed();
	}
}
