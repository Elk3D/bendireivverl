using System;
using UnityEngine;

[Serializable]
public class DoorData
{
	[SerializeField]
	private DoorStatus m_Status;

	[SerializeField]
	private DoorType m_Type;

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
}
