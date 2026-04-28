using System;
using UnityEngine;

[Serializable]
public class ObjectiveDataObject : DataObject<int, ObjectiveDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private ObjectiveStatus m_Status;

	public override int ID => m_DataID;

	public ObjectiveStatus Status => m_Status;

	public void SetStatus(ObjectiveStatus status)
	{
		m_Status = status;
	}

	protected override void Deserialize()
	{
		m_DataID = m_ID;
	}
}
