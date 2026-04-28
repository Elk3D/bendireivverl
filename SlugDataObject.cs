using System;
using UnityEngine;

[Serializable]
public class SlugDataObject : DataObject<int, SlugDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	public override int ID => m_DataID;

	protected override void Deserialize()
	{
		m_DataID = m_ID;
	}
}
