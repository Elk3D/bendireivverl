using System;
using UnityEngine;

public class ObjectiveCutscene : Objective
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Cutscene")]
	[SerializeField]
	private Cutscene m_Cutscene;

	[Header("Activation Options")]
	[SerializeField]
	private bool m_ClearNavigation = true;

	[SerializeField]
	private bool m_CanInterrupt;

	protected override void InternalUpdate()
	{
		if (m_CanInterrupt || GameManager.Instance.Player == null)
		{
			return;
		}
		if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat || GameManager.Instance.Player.BattleStatus != BattleStatus.None)
		{
			if (m_Cutscene.Content != null && !m_Cutscene.Content.Director.IsPlaying && m_Cutscene.Content.Activator.IsActive)
			{
				if (m_Cutscene.Content.Activator.Action != null)
				{
					m_Cutscene.Content.Activator.Action.ForceDisable();
				}
				m_Cutscene.SetActive(active: false);
			}
		}
		else if (m_Cutscene.Content != null && !m_Cutscene.Content.Director.IsPlaying && CheckStatusSimple() && !m_Cutscene.Content.Activator.IsActive)
		{
			m_Cutscene.SetActive(active: true);
		}
	}

	protected override void InternalInitialize()
	{
		JDebug.Log("Cutscene :: InternalInitialize", this, JDebug.JDebugType.Cutscene);
		if (m_Cutscene == null)
		{
			return;
		}
		RemoveListeners();
		CutsceneDataObject cutsceneDataObject = (CutsceneDataObject)GameManager.Instance.GameData.CurrentSave.GetData<CutsceneID, CutsceneDataObject>(m_Cutscene.SectionID, m_Cutscene.CutsceneID);
		if (cutsceneDataObject != null)
		{
			if (cutsceneDataObject.Status == CutsceneStatus.Complete)
			{
				SendOnComplete();
			}
			else if (cutsceneDataObject.Status == CutsceneStatus.Active)
			{
				if (m_ClearNavigation)
				{
					GameManager.Instance.ClearNavigation();
				}
				m_Cutscene.OnComplete += HandleCutsceneOnComplete;
				m_Cutscene.SetActive(active: false);
				if (m_Cutscene.Content != null)
				{
					m_Cutscene.Content.Initialize();
					if (m_Cutscene.Content.Activator != null && m_Cutscene.Content.Activator.Action != null)
					{
						m_Cutscene.Content.Activator.Action.Dispose();
					}
				}
				m_Cutscene.ForcePlay(cutsceneDataObject.Timeline);
			}
			else
			{
				CheckInitializer();
			}
		}
		else
		{
			cutsceneDataObject = DataObject<CutsceneID, CutsceneDataObject>.Create(m_Cutscene.CutsceneID);
			cutsceneDataObject.SetStatus(CutsceneStatus.None);
			GameManager.Instance.GameData.CurrentSave.AddData(m_Cutscene.SectionID, cutsceneDataObject);
			CheckInitializer();
		}
	}

	private void CheckInitializer()
	{
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		if (!CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
		}
	}

	private bool CheckStatusSimple()
	{
		bool result = true;
		if (m_Requirements != null)
		{
			result = m_Requirements.IsComplete();
		}
		return result;
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
			InitializeCutscene();
		}
		return flag;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		CheckStatus();
	}

	private void InitializeCutscene()
	{
		m_Cutscene.OnActivated += HandleCutsceneOnActivated;
		m_Cutscene.OnComplete += HandleCutsceneOnComplete;
		m_Cutscene.SetActive(active: true);
		InteractableInactive componentInChildren = m_Cutscene.GetComponentInChildren<InteractableInactive>(includeInactive: true);
		if (componentInChildren != null)
		{
			componentInChildren.InitializeInactive();
		}
		if (m_Cutscene.Content != null)
		{
			m_Cutscene.Content.Initialize();
			if (m_Cutscene.Content.Activator.ActivateType == CutsceneActivateType.Callback)
			{
				m_Cutscene.Play();
			}
		}
	}

	protected override void InternalEnable()
	{
		JDebug.Log("Cutscene :: InternalEnable", this, JDebug.JDebugType.Cutscene);
		if (m_Cutscene == null)
		{
			return;
		}
		RemoveListeners();
		CutsceneDataObject cutsceneDataObject = (CutsceneDataObject)GameManager.Instance.GameData.CurrentSave.GetData<CutsceneID, CutsceneDataObject>(m_Cutscene.SectionID, m_Cutscene.CutsceneID);
		if (cutsceneDataObject != null)
		{
			if (cutsceneDataObject.Status == CutsceneStatus.Complete)
			{
				SendOnComplete();
			}
			else
			{
				m_Cutscene.OnActivated += HandleCutsceneEnableOnActivated;
			}
		}
		else
		{
			cutsceneDataObject = DataObject<CutsceneID, CutsceneDataObject>.Create(m_Cutscene.CutsceneID);
			cutsceneDataObject.SetStatus(CutsceneStatus.None);
			GameManager.Instance.GameData.CurrentSave.AddData(m_Cutscene.SectionID, cutsceneDataObject);
			m_Cutscene.OnActivated += HandleCutsceneEnableOnActivated;
		}
	}

	private void HandleCutsceneEnableOnActivated(object sender, EventArgs e)
	{
		m_Cutscene.OnActivated -= HandleCutsceneEnableOnActivated;
		SendOnComplete();
	}

	private void HandleCutsceneOnActivated(object sender, EventArgs e)
	{
		m_Cutscene.OnActivated -= HandleCutsceneOnActivated;
		CutsceneDataObject cutsceneDataObject = (CutsceneDataObject)GameManager.Instance.GameData.CurrentSave.GetData<CutsceneID, CutsceneDataObject>(m_Cutscene.SectionID, m_Cutscene.CutsceneID);
		if (cutsceneDataObject == null)
		{
			cutsceneDataObject = DataObject<CutsceneID, CutsceneDataObject>.Create(m_Cutscene.CutsceneID);
			cutsceneDataObject.SetStatus(CutsceneStatus.Active);
			GameManager.Instance.GameData.CurrentSave.AddData(m_Cutscene.SectionID, cutsceneDataObject);
		}
		else
		{
			cutsceneDataObject.SetStatus(CutsceneStatus.Active);
		}
		if (m_ClearNavigation)
		{
			GameManager.Instance.ClearNavigation();
		}
	}

	private void HandleCutsceneOnComplete(object sender, EventArgs e)
	{
		m_Cutscene.OnComplete -= HandleCutsceneOnComplete;
		CompleteData();
		SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		JDebug.Log("Cutscene :: InternalForceComplete", this, JDebug.JDebugType.Cutscene);
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		RemoveListeners();
		if (m_Cutscene.ResetOnComplete)
		{
			m_Cutscene.OnActivated += HandleCutsceneOnActivated;
			m_Cutscene.OnComplete += HandleCutsceneOnComplete;
			m_Cutscene.SetActive(active: true);
		}
		else
		{
			m_Cutscene.SetActive(active: false);
			m_Cutscene.ForceComplete();
			SendOnComplete();
		}
	}

	protected override void InternalComplete()
	{
		JDebug.Log("Cutscene :: InternalComplete", this, JDebug.JDebugType.Cutscene);
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		RemoveListeners();
	}

	private void CompleteData()
	{
		CutsceneDataObject cutsceneDataObject = (CutsceneDataObject)GameManager.Instance.GameData.CurrentSave.GetData<CutsceneID, CutsceneDataObject>(m_Cutscene.SectionID, m_Cutscene.CutsceneID);
		if (cutsceneDataObject != null)
		{
			cutsceneDataObject.SetStatus(CutsceneStatus.Complete);
			return;
		}
		cutsceneDataObject = DataObject<CutsceneID, CutsceneDataObject>.Create(m_Cutscene.CutsceneID);
		cutsceneDataObject.SetStatus(CutsceneStatus.Complete);
		GameManager.Instance.GameData.CurrentSave.AddData(m_Cutscene.SectionID, cutsceneDataObject);
	}

	protected override void RemoveListeners()
	{
		if (m_Cutscene != null)
		{
			m_Cutscene.OnActivated -= HandleCutsceneEnableOnActivated;
			m_Cutscene.OnActivated -= HandleCutsceneOnActivated;
			m_Cutscene.OnComplete -= HandleCutsceneOnComplete;
		}
	}

	protected override void OnDisposed()
	{
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		base.OnDisposed();
	}
}
