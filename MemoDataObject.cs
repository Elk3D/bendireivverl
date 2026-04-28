using System;
using UnityEngine;

[Serializable]
public class MemoDataObject : DataObject<MemoID, MemoDataObject>, IDataObject<MemoID>, IDataObject
{
	[SerializeField]
	private MemoID m_DataID;

	public override MemoID ID => m_DataID;

	protected override void Deserialize()
	{
		m_DataID = m_ID;
	}
}
