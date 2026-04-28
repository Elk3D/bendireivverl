using System;
using UnityEngine;

[Serializable]
public class MovableDataObject : DataObject<MovableID, MovableDataObject>, IDataObject<MovableID>, IDataObject
{
	[SerializeField]
	private MovableID m_MovableID;

	[SerializeField]
	private bool m_IsActive;

	[SerializeField]
	private int m_CurrentIndex;

	[SerializeField]
	private int m_MoveState;

	[SerializeField]
	private int m_Side;

	[SerializeField]
	private Quaternion m_RotationX;

	[SerializeField]
	private Quaternion m_RotationY;

	public override MovableID ID => m_MovableID;

	public bool IsActive => m_IsActive;

	public int CurrentIndex => m_CurrentIndex;

	public int MoveState => m_MoveState;

	public int Side => m_Side;

	public Quaternion RotationX => m_RotationX;

	public Quaternion RotationY => m_RotationY;

	public void SetActive(bool active)
	{
		m_IsActive = active;
	}

	public void SetCurrentIndex(int currentIndex)
	{
		m_CurrentIndex = currentIndex;
	}

	public void SetMoveState(bool canMove)
	{
		m_MoveState = (canMove ? 1 : 0);
	}

	public void SetSide(int side)
	{
		m_Side = side;
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
		m_MovableID = m_ID;
	}
}
