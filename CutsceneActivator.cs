using System;
using UnityEngine;

public class CutsceneActivator : JMonoBehaviour
{
	[SerializeField]
	private CutsceneActivateType m_ActivateType;

	[SerializeField]
	private ActionEvent m_Action;

	public CutsceneActivateType ActivateType => m_ActivateType;

	public ActionEvent Action => m_Action;

	public bool IsActive { get; private set; }

	public event EventHandler OnActivated;

	public override void Awake()
	{
		if (m_ActivateType == CutsceneActivateType.OnAwake)
		{
			InternalPlay();
		}
	}

	public override void Start()
	{
		if (m_ActivateType == CutsceneActivateType.OnStart)
		{
			InternalPlay();
		}
		else if (m_ActivateType == CutsceneActivateType.EventAction && m_Action != null)
		{
			m_Action.OnInteract += HandleCutsceneOnInteract;
		}
	}

	public void Play()
	{
		if (m_ActivateType == CutsceneActivateType.Callback)
		{
			InternalPlay();
		}
	}

	public void ForcePlay()
	{
		InternalPlay();
	}

	private void InternalPlay()
	{
		JDebug.Log("CutsceneActivator :: InternalPlay", this, JDebug.JDebugType.Cutscene);
		this.OnActivated.Send(this);
	}

	public void SetActive(bool active)
	{
		InternalSetActive(active);
	}

	private void InternalSetActive(bool active)
	{
		IsActive = active;
		m_Action?.SetActive(IsActive);
	}

	private void HandleCutsceneOnInteract(object sender, EventArgs e)
	{
		JDebug.Log("CutsceneActivator :: HandleCutsceneOnInteract", this, JDebug.JDebugType.Cutscene);
		if (IsActive)
		{
			m_Action.OnInteract -= HandleCutsceneOnInteract;
			m_Action.SetActive(active: false);
			InternalPlay();
		}
	}

	public void ResetInteraction()
	{
		if (m_Action != null)
		{
			m_Action.OnInteract -= HandleCutsceneOnInteract;
			m_Action.OnInteract += HandleCutsceneOnInteract;
		}
	}

	protected override void OnDisposed()
	{
		if (m_Action != null)
		{
			m_Action.OnInteract -= HandleCutsceneOnInteract;
		}
		this.OnActivated = null;
		base.OnDisposed();
	}
}
