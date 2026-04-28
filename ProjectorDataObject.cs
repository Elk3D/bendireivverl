using System;
using UnityEngine;

[Serializable]
public class ProjectorDataObject : DataObject<int, ProjectorDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private int m_Index;

	public override int ID => m_DataID;

	public int Index => m_Index;

	public void SetIndex(int index)
	{
		m_Index = index;
	}

	protected override void Deserialize()
	{
		m_DataID = m_ID;
	}
}
