using System;
using UnityEngine;

[Serializable]
public class ComboLockDataObject : DataObject<int, ComboLockDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private int m_Code;

	[SerializeField]
	private int m_IsComplete;

	public override int ID => m_DataID;

	public int Code => m_Code;

	public int IsComplete => m_IsComplete;

	public void SetCode(int code)
	{
		m_Code = code;
	}

	public void SetComplete()
	{
		m_IsComplete = 1;
	}

	protected override void Deserialize()
	{
		m_DataID = m_ID;
	}
}
