using System;
using UnityEngine;

[Serializable]
public class LadderDataObject : DataObject<int, LadderDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private int m_ClimbState;

	[SerializeField]
	private int m_ClimbDirection;

	[SerializeField]
	private int m_LadderSide;

	[SerializeField]
	private int m_LadderIndex;

	[SerializeField]
	private Quaternion m_RotationX;

	[SerializeField]
	private Quaternion m_RotationY;

	public override int ID => m_DataID;

	public int ClimbState => m_ClimbState;

	public int ClimbDirection => m_ClimbDirection;

	public int LadderSide => m_LadderSide;

	public int LadderIndex => m_LadderIndex;

	public Quaternion RotationX => m_RotationX;

	public Quaternion RotationY => m_RotationY;

	public void SetClimbState(bool isClimbing)
	{
		m_ClimbState = (isClimbing ? 1 : 0);
	}

	public void SetClimbDirection(int direction)
	{
		m_ClimbDirection = direction;
	}

	public void SetLadderSide(int side)
	{
		m_LadderSide = side;
	}

	public void SetLadderIndex(int index)
	{
		m_LadderIndex = index;
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
