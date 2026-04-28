using System;
using UnityEngine;

[Serializable]
public class GentPowerDataObject : DataObject<GentPowerID, GentPowerDataObject>, IDataObject<GentPowerID>, IDataObject
{
	[SerializeField]
	private GentPowerID m_GentPowerID;

	[SerializeField]
	private CutsceneStatus m_Status;

	public override GentPowerID ID => m_GentPowerID;

	public CutsceneStatus Status => m_Status;

	public void SetStatus(CutsceneStatus status)
	{
		m_Status = status;
	}

	protected override void Deserialize()
	{
		m_GentPowerID = m_ID;
	}
}
