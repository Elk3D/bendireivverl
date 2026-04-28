using System;
using UnityEngine;

[Serializable]
public class BreakableDataObject : DataObject<int, BreakableDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_BreakableID;

	public override int ID => m_BreakableID;

	protected override void Deserialize()
	{
		m_BreakableID = m_ID;
	}
}
