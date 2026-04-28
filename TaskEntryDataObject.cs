using System;
using UnityEngine;

[Serializable]
public class TaskEntryDataObject : DataObject<TaskEntryID, TaskEntryDataObject>, IDataObject<TaskEntryID>, IDataObject
{
	[SerializeField]
	private TaskEntryID m_DataID;

	[SerializeField]
	private bool m_IsComplete;

	public override TaskEntryID ID => m_DataID;

	public bool IsComplete => m_IsComplete;

	public void Complete()
	{
		m_IsComplete = true;
	}

	protected override void Deserialize()
	{
		m_DataID = m_ID;
	}
}
