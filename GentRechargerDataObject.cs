using System;
using UnityEngine;

[Serializable]
public class GentRechargerDataObject : DataObject<int, GentRechargerDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private int m_Batteries;

	public override int ID => m_DataID;

	public int Batteries => m_Batteries;

	protected override void Deserialize()
	{
		m_DataID = m_ID;
	}

	public void SetBatteries(int batteries)
	{
		m_Batteries = batteries;
	}

	public void Clear()
	{
		m_Batteries = 0;
	}
}
