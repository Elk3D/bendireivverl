using System;
using UnityEngine;

[Serializable]
public class JackatoyDataObject : DataObject<JackatoyID, JackatoyDataObject>, IDataObject<JackatoyID>, IDataObject
{
	[SerializeField]
	private JackatoyID m_DataID;

	public override JackatoyID ID => m_DataID;

	protected override void Deserialize()
	{
		m_DataID = m_ID;
	}
}
