using System;
using UnityEngine;

[Serializable]
public class ActionEventData : DataObject<int, ActionEventData>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_ActionEventID;

	public override int ID => m_ActionEventID;

	protected override void Deserialize()
	{
		m_ActionEventID = m_ID;
	}
}
