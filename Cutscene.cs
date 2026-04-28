using System;
using DG.Tweening;
using UnityEngine;

public class Cutscene : DataMonoBehaviour<CutsceneID, CutsceneDataObject>
{
	[Header("Section Identifier")]
	[SerializeField]
	protected SectionID m_SectionID;

	[Header("Cutscene Identifier")]
	[SerializeField]
	protected CutsceneID m_CutsceneID;

	[Header("Cutscene Content")]
	[SerializeField]
	private CutsceneContent m_Content;

	[Header("Cutscene Complete Content")]
	[SerializeField]
	private GameObject m_CompleteContent;

	[Header("Options")]
	[SerializeField]
	private bool m_ResetOnComplete;

	[SerializeField]
	private bool m_KillAudioLogs = true;

	public SectionID SectionID => m_SectionID;

	protected override CutsceneID m_ID => m_CutsceneID;

	public CutsceneID CutsceneID => m_CutsceneID;

	public bool ResetOnComplete => m_ResetOnComplete;

	public bool KillAudioLogs => m_KillAudioLogs;

	public CutsceneContent Content { get; private set; }

	public event EventHandler OnActivated;

	public event EventHandler OnUnityEvent;

	public event EventHandler OnComplete;

	protected override void InternalInitialize()
	{
		if (!(m_Content != null))
		{
			return;
		}
		CutsceneDataObject cutsceneDataObject = (CutsceneDataObject)GameManager.Instance.GameData.CurrentSave.GetData<CutsceneID, CutsceneDataObject>(m_SectionID, m_CutsceneID);
		if (cutsceneDataObject != null)
		{
			if (cutsceneDataObject.Status != CutsceneStatus.Complete || m_ResetOnComplete)
			{
				AddContent();
			}
			else
			{
				AddCompleteContent();
			}
		}
		else
		{
			cutsceneDataObject = DataObject<CutsceneID, CutsceneDataObject>.Create(m_CutsceneID);
			cutsceneDataObject.SetStatus(CutsceneStatus.None);
			GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, cutsceneDataObject);
			AddContent();
		}
	}

	public void AddContent()
	{
		Content = GameManager.Instance.AssetManager.CreateAsset<CutsceneContent>(m_Content);
		Content.transform.SetParent(base.transform);
		Content.transform.localPosition = Vector3.zero;
		Content.transform.localEulerAngles = Vector3.zero;
		Content.OnForceComplete -= HandleCutsceneContentOnForceComplete;
		Content.OnForceComplete += HandleCutsceneContentOnForceComplete;
		Content.Activator.OnActivated -= HandleCutsceneActivatorOnActivated;
		Content.Activator.OnActivated += HandleCutsceneActivatorOnActivated;
	}

	private void HandleCutsceneContentOnForceComplete(object sender, EventArgs e)
	{
		Content.OnForceComplete -= HandleCutsceneContentOnForceComplete;
		ForceComplete();
	}

	public void AddCompleteContent()
	{
		if (m_CompleteContent != null)
		{
			CutsceneComplete cutsceneComplete = GameManager.Instance.AssetManager.CreateAsset<CutsceneComplete>(m_CompleteContent.GetComponent<CutsceneComplete>());
			cutsceneComplete.transform.SetParent(base.transform);
			cutsceneComplete.transform.localPosition = Vector3.zero;
			cutsceneComplete.transform.localEulerAngles = Vector3.zero;
			cutsceneComplete.Initialize();
		}
	}

	private void InternalPreparePlay()
	{
		if (m_KillAudioLogs)
		{
			GameManager.Instance.KillAudioLog();
		}
		GameManager.Instance.HideCrosshair();
		if (GameManager.Instance.BeastBendy == null)
		{
			GameManager.Instance.Player.HideFirstPersonArms();
			GameManager.Instance.Player.CancelMovement();
			GameManager.Instance.Player.ResetAnimation();
			GameManager.Instance.Player.ResetRotation();
			GameManager.Instance.Player.SetState(State.Player.Cutscene);
		}
		Vector3 vector = ((GameManager.Instance.BeastBendy == null) ? (Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth) : Vector3.zero);
		Sequence sequence = DOTween.Sequence();
		Transform transform = null;
		transform = Content.StartLocation;
		if (transform != null)
		{
			if (GameManager.Instance.BeastBendy == null)
			{
				sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(transform.position + vector, 0.25f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
				sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(transform.eulerAngles, 0.25f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
			}
			else
			{
				sequence.Insert(0f, GameManager.Instance.BeastBendy.transform.DOMove(transform.position, 0.5f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
				sequence.Insert(0f, GameManager.Instance.BeastBendy.transform.DORotate(transform.eulerAngles, 0.5f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
			}
		}
		sequence.OnComplete(InternalPreparePlayOnComplete);
	}

	private void InternalPreparePlayOnComplete()
	{
		GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
		InternalPlay();
	}

	private void InternalPlay()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.Interaction.ResetInteraction();
		}
		Content.Director.Play();
		this.OnActivated.Send(this);
	}

	public void ForcePlay(double timeline)
	{
		Content.Activator.OnActivated -= HandleCutsceneActivatorOnActivated;
		if (!Content.CompleteOnPlay)
		{
			Content.Director.OnComplete -= HandleDirectorOnComplete;
			Content.Director.OnComplete += HandleDirectorOnComplete;
		}
		else
		{
			DirectorOnComplete();
		}
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.Interaction.ResetInteraction();
		}
		Content.Director.Play();
		Content.Director.PlayableDirector.time = timeline;
		this.OnActivated.Send(this);
	}

	public void ForceComplete()
	{
		if (!m_ResetOnComplete && Content != null)
		{
			Content.Activator.OnActivated -= HandleCutsceneActivatorOnActivated;
			Content.Director.OnComplete -= HandleDirectorOnComplete;
		}
		SendOnComplete();
	}

	private void HandleCutsceneActivatorOnActivated(object sender, EventArgs e)
	{
		JDebug.Log("Cutscene :: HandleCutsceneActivatorOnActivated", this, JDebug.JDebugType.Cutscene);
		Content.Activator.OnActivated -= HandleCutsceneActivatorOnActivated;
		if (!Content.CompleteOnPlay)
		{
			Content.Director.OnComplete -= HandleDirectorOnComplete;
			Content.Director.OnComplete += HandleDirectorOnComplete;
		}
		else
		{
			DirectorOnComplete();
		}
		if (Content.StartLocation != null)
		{
			InternalPreparePlay();
		}
		else
		{
			InternalPlay();
		}
	}

	private void HandleDirectorOnComplete(object sender, EventArgs e)
	{
		Content.Director.OnComplete -= HandleDirectorOnComplete;
		DirectorOnComplete();
	}

	private void DirectorOnComplete()
	{
		if (m_ResetOnComplete)
		{
			Content.Director.ResetDirector();
			Content.Activator.Action?.ResetAction();
			SetActive(active: true);
			Content.Activator.OnActivated -= HandleCutsceneActivatorOnActivated;
			Content.Activator.OnActivated += HandleCutsceneActivatorOnActivated;
			Content.Activator.ResetInteraction();
		}
		SendOnComplete();
	}

	protected void SendOnComplete()
	{
		this.OnComplete.Send(this);
		Load(null);
	}

	public void SetActive(bool active)
	{
		if (Content != null)
		{
			Content.Activator.SetActive(active);
		}
	}

	public void Play()
	{
		if (Content != null)
		{
			Content.Activator.Play();
		}
	}

	public void SendOnUnityEvent()
	{
		this.OnUnityEvent.Send(this);
	}

	protected override void OnDisposed()
	{
		if (Content != null)
		{
			Content.OnForceComplete -= HandleCutsceneContentOnForceComplete;
			Content.Activator.OnActivated -= HandleCutsceneActivatorOnActivated;
			Content.Director.OnComplete -= HandleDirectorOnComplete;
		}
		base.OnDisposed();
	}
}
