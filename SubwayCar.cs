using System;
using DG.Tweening;
using UnityEngine;

public class SubwayCar : JMonoBehaviour
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Cutscene Parents")]
	[SerializeField]
	private Transform m_CutsceneParent;

	[SerializeField]
	private Transform m_Cutscenes;

	[Header("Section Controllers")]
	[SerializeField]
	private Cutscene m_CutsceneStationArrival;

	[SerializeField]
	private Door m_Door;

	[SerializeField]
	private Door m_DoorCar;

	[Header("Subway")]
	[SerializeField]
	private Transform m_SubwayCarParent;

	[SerializeField]
	private Transform m_Subway;

	[SerializeField]
	private Transform m_SubwayStart;

	[SerializeField]
	private Transform m_SubwayEnd;

	[SerializeField]
	private Transform m_SubwayLeave;

	[Header("Tunnel")]
	[SerializeField]
	private Transform m_Tunnel;

	[SerializeField]
	private Transform m_TunnelStart;

	[SerializeField]
	private Transform m_TunnelEnd;

	[Header("DOTween Animations")]
	[SerializeField]
	private DOTweenAnimation[] m_Handles;

	private Sequence m_TunnelSequence;

	private Sequence m_Sequence;

	public event EventHandler OnLeave;

	public override void Start()
	{
		if (!CheckStatus())
		{
			for (int i = 0; i < m_Handles.Length; i++)
			{
				m_Handles[i].DOPlay();
			}
			m_Cutscenes.SetParent(m_CutsceneParent);
			Cutscene[] componentsInChildren = m_Cutscenes.GetComponentsInChildren<Cutscene>(includeInactive: true);
			foreach (Cutscene cutscene in componentsInChildren)
			{
				if (cutscene != null)
				{
					cutscene.Initialize();
				}
			}
			GameManager.Instance.Player.transform.SetParent(m_SubwayCarParent);
			m_CutsceneStationArrival.OnActivated -= HandleRetreatInitializeOnInteract;
			m_CutsceneStationArrival.OnActivated += HandleRetreatInitializeOnInteract;
			Loop();
		}
		else
		{
			Dispose();
		}
	}

	private bool CheckStatus()
	{
		bool result = true;
		if (m_Requirements != null)
		{
			result = m_Requirements.IsComplete();
		}
		return result;
	}

	private void Loop()
	{
		CameraEffects.ShakeRotation(1.5f, 0.5f, 10, 90f, fadeOut: false);
		ResetTunnelSequence();
		m_TunnelSequence.InsertCallback(0f, delegate
		{
			m_Tunnel.position = m_TunnelStart.position;
		});
		m_TunnelSequence.Insert(0f, m_Tunnel.DOMove(m_TunnelEnd.position, 1.5f).SetEase(Ease.Linear));
		m_TunnelSequence.OnComplete(Loop);
	}

	private void HandleRetreatInitializeOnInteract(object sender, EventArgs e)
	{
		m_CutsceneStationArrival.OnActivated -= HandleRetreatInitializeOnInteract;
		float num = 0.5f;
		ResetSequence();
		m_Sequence.InsertCallback(num, delegate
		{
			m_Subway.position = m_SubwayStart.position;
			KillTunnelSequence();
		});
		m_Sequence.Insert(num, m_Subway.DOMove(m_SubwayEnd.position, 4f).SetEase(Ease.OutQuad));
		num += 4f;
		m_Sequence.InsertCallback(num, RetreatInitializeOnComplete);
	}

	private void RetreatInitializeOnComplete()
	{
		for (int i = 0; i < m_Handles.Length; i++)
		{
			m_Handles[i].DOKill();
		}
		m_DoorCar.Data.SetStatus(DoorStatus.Closed);
		m_DoorCar.Content.ForceDeactivateComplete();
		m_DoorCar.Content.Enable();
		m_Door.Content.ForceActivate();
		CameraEffects.ShakeRotation(0.5f, 5f);
		GameManager.Instance.Player.transform.SetParent(null);
		UnityEngine.Object.DontDestroyOnLoad(GameManager.Instance.Player.gameObject);
	}

	public void Leave()
	{
		if (GameManager.Instance.Player.transform.parent != null)
		{
			GameManager.Instance.Player.transform.SetParent(null);
			UnityEngine.Object.DontDestroyOnLoad(GameManager.Instance.Player.gameObject);
		}
		m_Door.Content.ForceDeactivate();
		ResetSequence();
		m_Sequence.Insert(1f, m_Subway.DOMove(m_SubwayLeave.position, 4f).SetEase(Ease.InSine));
		m_Sequence.OnComplete(Dispose);
		this.OnLeave.Send(this);
	}

	private void ResetTunnelSequence()
	{
		KillTunnelSequence();
		m_TunnelSequence = DOTween.Sequence();
	}

	private void KillTunnelSequence()
	{
		if (m_TunnelSequence != null)
		{
			m_TunnelSequence.Kill();
			m_TunnelSequence = null;
		}
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
		this.OnLeave = null;
		KillTunnelSequence();
		KillSequence();
		for (int i = 0; i < m_Handles.Length; i++)
		{
			m_Handles[i].DOKill();
		}
		if (m_CutsceneStationArrival != null)
		{
			m_CutsceneStationArrival.OnActivated -= HandleRetreatInitializeOnInteract;
		}
		base.OnDisposed();
	}
}
