using System;
using UnityEngine;

[Serializable]
public class DoorDataObject : DataObject<DoorID, DoorDataObject>, IDataObject<DoorID>, IDataObject
{
	[SerializeField]
	private DoorID m_DoorID;

	[SerializeField]
	private DoorStatus m_Status;

	[SerializeField]
	private DoorType m_Type;

	public override DoorID ID => m_DoorID;

	public DoorStatus Status => m_Status;

	public DoorType Type => m_Type;

	public void SetStatus(DoorStatus status)
	{
		m_Status = status;
	}

	public void SetType(DoorType type)
	{
		m_Type = type;
	}

	protected override void Deserialize()
	{
		m_DoorID = m_ID;
	}
}
