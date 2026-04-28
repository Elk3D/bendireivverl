using System;
using UnityEngine;

[Serializable]
public class GentLockDataObject : DataObject<GentLockID, GentLockDataObject>, IDataObject<GentLockID>, IDataObject
{
	[SerializeField]
	private GentLockID m_GentLockID;

	[SerializeField]
	private CutsceneStatus m_Status;

	[SerializeField]
	private double m_Timeline;

	public override GentLockID ID => m_GentLockID;

	public CutsceneStatus Status => m_Status;

	public double Timeline => m_Timeline;

	public void SetStatus(CutsceneStatus status)
	{
		m_Status = status;
	}

	public void SetTimeline(double timeline)
	{
		m_Timeline = timeline;
	}

	protected override void Deserialize()
	{
		m_GentLockID = m_ID;
	}
}
