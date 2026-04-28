using System;
using UnityEngine;

[Serializable]
public class HideDataObject : DataObject<int, HideDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private int m_HideState;

	[SerializeField]
	private Quaternion m_RotationX;

	[SerializeField]
	private Quaternion m_RotationY;

	public override int ID => m_DataID;

	public int HideState => m_HideState;

	public Quaternion RotationX => m_RotationX;

	public Quaternion RotationY => m_RotationY;

	public void SetHideState(bool isHiding)
	{
		m_HideState = (isHiding ? 1 : 0);
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
