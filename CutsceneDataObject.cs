using System;
using UnityEngine;

[Serializable]
public class CutsceneDataObject : DataObject<CutsceneID, CutsceneDataObject>, IDataObject<CutsceneID>, IDataObject
{
	[SerializeField]
	private CutsceneID m_CutsceneID;

	[SerializeField]
	private CutsceneStatus m_Status;

	[SerializeField]
	private double m_Timeline;

	public override CutsceneID ID => m_CutsceneID;

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
		m_CutsceneID = m_ID;
	}
}
