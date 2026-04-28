using System;
using DG.Tweening;
using UnityEngine;

public class GentPower : JMonoBehaviour
{
	[Header("Section Identifier")]
	[SerializeField]
	protected SectionID m_SectionID;

	[Header("GentPower Identifier")]
	[SerializeField]
	protected GentPowerID m_GentPowerID;

	[Header("GentPower Content")]
	[SerializeField]
	private GentPowerContent m_Content;

	[Header("GentPower Complete")]
	[SerializeField]
	private GameObject m_CompleteContent;

	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	public SectionID SectionID => m_SectionID;

	public GentPowerID GentPowerID => m_GentPowerID;

	public GentPowerContent Content { get; private set; }

	public bool IsActive { get; private set; }

	public bool IsActivated { get; private set; }

	public bool IsComplete { get; private set; }

	public event EventHandler OnActivated;

	public event EventHandler OnComplete;

	public void Initialize()
	{
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.GentPowerDirectory.ContainsKey(m_GentPowerID))
		{
			AddContent();
		}
		else
		{
			AddCompleteContent();
		}
	}

	private void AddContent()
	{
		Content = GameManager.Instance.AssetManager.CreateAsset<GentPowerContent>(m_Content);
		Content.transform.SetParent(base.transform);
		Content.transform.localPosition = Vector3.zero;
		Content.transform.localEulerAngles = Vector3.zero;
		Content.OnForceComplete -= HandleContentOnForceComplete;
		Content.OnForceComplete += HandleContentOnForceComplete;
		Content.Activator.OnActivated -= HandleActivatorOnActivated;
		Content.Activator.OnActivated += HandleActivatorOnActivated;
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		if (!CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
		}
	}

	private void HandleContentOnForceComplete(object sender, EventArgs e)
	{
		Content.OnForceComplete -= HandleContentOnForceComplete;
		ForceComplete();
	}

	private void AddCompleteContent()
	{
		if (m_CompleteContent != null)
		{
			CutsceneComplete cutsceneComplete = GameManager.Instance.AssetManager.CreateAsset<CutsceneComplete>(m_CompleteContent.GetComponent<CutsceneComplete>());
			cutsceneComplete.transform.SetParent(base.transform);
			cutsceneComplete.transform.localPosition = Vector3.zero;
			cutsceneComplete.transform.localEulerAngles = Vector3.zero;
		}
	}

	public void Play()
	{
		Content.Activator.Play();
	}

	private void InternalPreparePlay()
	{
		GameManager.Instance.HideCrosshair();
		GameManager.Instance.Player.HideFirstPersonArms();
		GameManager.Instance.Player.CancelMovement();
		GameManager.Instance.Player.ResetAnimation();
		GameManager.Instance.Player.ResetRotation();
		GameManager.Instance.Player.SetState(State.Player.Cutscene);
		Vector3 vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(Content.StartLocation.position + vector, 0.25f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
		sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(Content.StartLocation.eulerAngles, 0.25f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
		sequence.OnComplete(InternalPreparePlayOnComplete);
	}

	private void InternalPreparePlayOnComplete()
	{
		GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
		Content.Director.Play();
		this.OnActivated.Send(this);
	}

	private void DirectorOnComplete()
	{
		RemoveListeners();
		GameManager.Instance.ShowCrosshair();
		IsComplete = true;
		this.OnComplete.Send(this);
	}

	public void ForceComplete()
	{
		DirectorOnComplete();
	}

	public void SetActive(bool active)
	{
		Content.Activator.SetActive(active);
	}

	private void HandleActivatorOnActivated(object sender, EventArgs e)
	{
		Content.Activator.OnActivated -= HandleActivatorOnActivated;
		GameManager.Instance.GameData.CurrentSave.DataDirectories.GentPowerDirectory.Add(m_GentPowerID, DataObject<GentPowerID, GentPowerDataObject>.Create(m_GentPowerID));
		Content.Director.OnComplete -= HandleDirectorOnComplete;
		Content.Director.OnComplete += HandleDirectorOnComplete;
		InternalPreparePlay();
	}

	private void HandleDirectorOnComplete(object sender, EventArgs e)
	{
		DirectorOnComplete();
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("GentPower :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
		CheckStatus();
	}

	private void Update()
	{
		if (IsComplete || !IsActive || base.IsDisposed || GameManager.Instance.IsPaused || GameManager.Instance.Player.CurrentWeapon == null)
		{
			return;
		}
		if (!IsActivated)
		{
			if (GameManager.Instance.Player.CombatStatus != CombatStatus.Combat && GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.Power >= 6)
			{
				IsActivated = true;
				SetActive(active: true);
			}
		}
		else if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat)
		{
			IsActivated = false;
			Content.Activator.Action.ForceDisable();
			SetActive(active: false);
		}
		else if (GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.Power < 6)
		{
			IsActivated = false;
			Content.Activator.Action.ForceDisable();
			SetActive(active: false);
		}
	}

	private bool CheckStatus()
	{
		bool flag = true;
		if (m_Requirements != null)
		{
			flag = m_Requirements.IsComplete();
		}
		if (flag)
		{
			GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
			IsActive = true;
		}
		return flag;
	}

	private void RemoveListeners()
	{
		if (Content != null)
		{
			Content.OnForceComplete -= HandleContentOnForceComplete;
			Content.Activator.OnActivated -= HandleActivatorOnActivated;
			Content.Director.OnComplete -= HandleDirectorOnComplete;
		}
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		base.OnDisposed();
	}
}
