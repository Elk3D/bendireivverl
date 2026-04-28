using System;
using UnityEngine;

[Serializable]
public class MemoryDataObject : DataObject<MemoryID, MemoryDataObject>, IDataObject<MemoryID>, IDataObject
{
	[SerializeField]
	private MemoryID m_DataID;

	public override MemoryID ID => m_DataID;

	protected override void Deserialize()
	{
		m_DataID = m_ID;
	}
}
