using System;
using UnityEngine;

[Serializable]
public class FoodDataObject : DataObject<int, FoodDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	public override int ID => m_DataID;

	protected override void Deserialize()
	{
		m_DataID = m_ID;
	}
}
