using System;
using UnityEngine;

[Serializable]
public class MeatlyDataObject : DataObject<MeatlyID, MeatlyDataObject>, IDataObject<MeatlyID>, IDataObject
{
	[SerializeField]
	private MeatlyID m_DataID;

	public override MeatlyID ID => m_DataID;

	protected override void Deserialize()
	{
		m_DataID = m_ID;
	}
}
