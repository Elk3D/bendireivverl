using System;
using UnityEngine;

[Serializable]
public class IllusionDataObject : DataObject<IllusionID, IllusionDataObject>, IDataObject<IllusionID>, IDataObject
{
	[SerializeField]
	private IllusionID m_DataID;

	public override IllusionID ID => m_DataID;

	protected override void Deserialize()
	{
		m_DataID = m_ID;
	}
}
