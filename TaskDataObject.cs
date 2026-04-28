using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TaskDataObject : DataObject<TaskID, TaskDataObject>, IDataObject<TaskID>, IDataObject
{
	[SerializeField]
	private TaskID m_DataID;

	[SerializeField]
	private bool m_IsComplete;

	[SerializeField]
	private List<TaskEntryDataObject> m_Tasks = new List<TaskEntryDataObject>();

	public override TaskID ID => m_DataID;

	public bool IsComplete => m_IsComplete;

	public List<TaskEntryDataObject> Tasks => m_Tasks;

	public void Complete()
	{
		m_IsComplete = true;
	}

	public TaskEntryDataObject GetEntry(TaskEntryID taskEntryID)
	{
		TaskEntryDataObject taskEntryDataObject = null;
		for (int i = 0; i < m_Tasks.Count; i++)
		{
			taskEntryDataObject = m_Tasks[i];
			if (taskEntryDataObject.ID == taskEntryID)
			{
				break;
			}
			taskEntryDataObject = null;
		}
		return taskEntryDataObject;
	}

	public bool CheckComplete()
	{
		bool result = true;
		for (int i = 0; i < m_Tasks.Count; i++)
		{
			if (!m_Tasks[i].IsComplete)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	protected override void Deserialize()
	{
		m_DataID = m_ID;
	}
}
