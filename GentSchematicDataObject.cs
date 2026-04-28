using System;
using UnityEngine;

[Serializable]
public class GentSchematicDataObject : DataObject<GentSchematicID, GentSchematicDataObject>, IDataObject<GentSchematicID>, IDataObject
{
	[SerializeField]
	private GentSchematicID m_DataID;

	public override GentSchematicID ID => m_DataID;

	protected override void Deserialize()
	{
		m_DataID = m_ID;
	}
}
