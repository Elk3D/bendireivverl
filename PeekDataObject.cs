using System;
using UnityEngine;

[Serializable]
public class PeekDataObject : DataObject<int, PeekDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private int m_PeekState;

	[SerializeField]
	private int m_Index;

	[SerializeField]
	private Quaternion m_RotationX;

	[SerializeField]
	private Quaternion m_RotationY;

	public override int ID => m_DataID;

	public int PeekState => m_PeekState;

	public int Index => m_Index;

	public Quaternion RotationX => m_RotationX;

	public Quaternion RotationY => m_RotationY;

	public void SetPeekState(bool isPeeking)
	{
		m_PeekState = (isPeeking ? 1 : 0);
	}

	public void SetIndex(int index)
	{
		m_Index = index;
	}

	public void SetRotationX(Quaternion rotation)
	{
		m_RotationX = rotation;
	}

	public void SetRotationY(Quaternion rotation)
	{
		m_RotationY = rotation;
	}

	protected override void Deserialize()
	{
		m_DataID = m_ID;
	}
}
