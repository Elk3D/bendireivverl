using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionObjectiveController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private ObjectiveController[] m_PrimaryObjectives;

	[SerializeField]
	private ObjectiveController[] m_ActiveObjectives;

	private ObjectiveController m_CurrentPrimaryObjective;

	private int m_PrimaryObjectiveIndex;

	private int m_ActiveObjectiveIndex;

	public SectionID SectionID => m_SectionID;

	public bool IsPrimaryComplete { get; private set; }

	public bool IsActiveComplete { get; private set; }

	protected override IEnumerator InternalInitialize()
	{
		RemoveListeners();
		InitializeObjectives();
		InitializeActiveObjectives();
		yield return null;
	}

	private void InitializeObjectives()
	{
		if (m_PrimaryObjectives != null && m_PrimaryObjectives.Length != 0)
		{
			for (int i = 0; i < m_PrimaryObjectives.Length; i++)
			{
				ObjectiveController objectiveController = m_PrimaryObjectives[i];
				objectiveController.OnComplete -= HandlePrimaryObjectiveOnComplete;
				ObjectiveDataObject objectiveDataObject = (ObjectiveDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, ObjectiveDataObject>(m_SectionID, objectiveController.ID);
				if (objectiveDataObject != null)
				{
					if (objectiveDataObject.Status == ObjectiveStatus.Complete)
					{
						objectiveController.ForceComplete();
						m_PrimaryObjectiveIndex++;
					}
					else
					{
						objectiveController.OnComplete += HandlePrimaryObjectiveOnComplete;
						objectiveController.OnStart();
					}
				}
				else
				{
					objectiveDataObject = DataObject<int, ObjectiveDataObject>.Create(objectiveController.ID);
					objectiveDataObject.SetStatus(ObjectiveStatus.Inactive);
					GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, objectiveDataObject);
					objectiveController.OnComplete += HandlePrimaryObjectiveOnComplete;
					objectiveController.OnStart();
				}
			}
			if (m_PrimaryObjectiveIndex < m_PrimaryObjectives.Length)
			{
				m_CurrentPrimaryObjective = m_PrimaryObjectives[m_PrimaryObjectiveIndex];
				ExecuteCurrentPrimaryObjective();
			}
			else
			{
				CheckPrimaryObjectivesComplete();
			}
		}
		else
		{
			AbortPrimaryObjectives();
		}
	}

	private void InitializeActiveObjectives()
	{
		if (m_ActiveObjectives != null && m_ActiveObjectives.Length != 0)
		{
			for (int i = 0; i < m_ActiveObjectives.Length; i++)
			{
				ObjectiveController objectiveController = m_ActiveObjectives[i];
				objectiveController.OnComplete -= HandleActiveObjectiveOnComplete;
				ObjectiveDataObject objectiveDataObject = (ObjectiveDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, ObjectiveDataObject>(m_SectionID, objectiveController.ID);
				if (objectiveDataObject != null)
				{
					if (objectiveDataObject.Status == ObjectiveStatus.Complete)
					{
						objectiveController.ForceComplete();
						m_ActiveObjectiveIndex++;
					}
					else
					{
						objectiveController.OnComplete += HandleActiveObjectiveOnComplete;
						objectiveController.OnStart();
						objectiveController.Execute();
					}
				}
				else
				{
					objectiveDataObject = DataObject<int, ObjectiveDataObject>.Create(objectiveController.ID);
					objectiveDataObject.SetStatus(ObjectiveStatus.Active);
					GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, objectiveDataObject);
					objectiveController.OnComplete += HandleActiveObjectiveOnComplete;
					objectiveController.OnStart();
					objectiveController.Execute();
				}
			}
			if (m_ActiveObjectiveIndex >= m_ActiveObjectives.Length)
			{
				CheckActiveObjectivesComplete();
			}
		}
		else
		{
			AbortActiveObjectives();
		}
	}

	private void HandlePrimaryObjectiveOnComplete(object sender, EventArgs e)
	{
		ObjectiveController objectiveController = (ObjectiveController)sender;
		objectiveController.OnComplete -= HandlePrimaryObjectiveOnComplete;
		UpdateObjectiveData(objectiveController.ID, ObjectiveStatus.Complete);
		CheckNextPrimaryObjective();
	}

	private void HandleActiveObjectiveOnComplete(object sender, EventArgs e)
	{
		ObjectiveController objectiveController = (ObjectiveController)sender;
		objectiveController.OnComplete -= HandleActiveObjectiveOnComplete;
		UpdateObjectiveData(objectiveController.ID, ObjectiveStatus.Complete);
		m_ActiveObjectiveIndex++;
		if (m_ActiveObjectiveIndex >= m_ActiveObjectives.Length)
		{
			CheckActiveObjectivesComplete();
		}
	}

	private void CheckActiveObjectivesComplete()
	{
		IsActiveComplete = true;
		CheckSectionControllerComplete();
	}

	private void UpdateObjectiveData(int id, ObjectiveStatus status)
	{
		ObjectiveDataObject objectiveDataObject = (ObjectiveDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, ObjectiveDataObject>(m_SectionID, id);
		if (objectiveDataObject != null)
		{
			objectiveDataObject.SetStatus(status);
			return;
		}
		objectiveDataObject = DataObject<int, ObjectiveDataObject>.Create(id);
		objectiveDataObject.SetStatus(status);
		GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, objectiveDataObject);
	}

	private void CheckNextPrimaryObjective()
	{
		m_PrimaryObjectiveIndex++;
		if (m_PrimaryObjectiveIndex < m_PrimaryObjectives.Length)
		{
			m_CurrentPrimaryObjective = m_PrimaryObjectives[m_PrimaryObjectiveIndex];
			if (m_CurrentPrimaryObjective.IsComplete)
			{
				m_CurrentPrimaryObjective.OnComplete -= HandlePrimaryObjectiveOnComplete;
				CheckNextPrimaryObjective();
			}
			else
			{
				ExecuteCurrentPrimaryObjective();
			}
		}
		else
		{
			CheckPrimaryObjectivesComplete();
		}
	}

	private void CheckPrimaryObjectivesComplete()
	{
		IsPrimaryComplete = true;
		CheckSectionControllerComplete();
	}

	private void ExecuteCurrentPrimaryObjective()
	{
		m_CurrentPrimaryObjective.Execute();
		UpdateObjectiveData(m_CurrentPrimaryObjective.ID, ObjectiveStatus.Active);
	}

	private void CheckSectionControllerComplete()
	{
		if (IsPrimaryComplete && IsActiveComplete)
		{
			((SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(m_SectionID))?.SetComplete();
			SendOnComplete();
			GameManager.Instance.CompleteSection();
		}
	}

	private void AbortPrimaryObjectives()
	{
		JDebug.Log("[Abort Primary Objectives] :: There are no m_PrimaryObjectives", this, JDebug.JDebugType.Objectives);
		IsPrimaryComplete = true;
		CheckSectionControllerComplete();
	}

	private void AbortActiveObjectives()
	{
		JDebug.Log("[Abort Active Objectives] :: There are no m_ActiveObjectives", this, JDebug.JDebugType.Objectives);
		IsActiveComplete = true;
		CheckSectionControllerComplete();
	}

	protected override void RemoveListeners()
	{
		if (m_PrimaryObjectives != null && m_PrimaryObjectives.Length != 0)
		{
			for (int i = 0; i < m_PrimaryObjectives.Length; i++)
			{
				m_PrimaryObjectives[i].OnComplete -= HandlePrimaryObjectiveOnComplete;
			}
		}
		if (m_ActiveObjectives != null && m_ActiveObjectives.Length != 0)
		{
			for (int j = 0; j < m_ActiveObjectives.Length; j++)
			{
				m_ActiveObjectives[j].OnComplete -= HandleActiveObjectiveOnComplete;
			}
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		m_CurrentPrimaryObjective = null;
	}
}
