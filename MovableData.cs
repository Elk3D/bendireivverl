using System;
using UnityEngine;

[Serializable]
public class MovableData
{
	[SerializeField]
	private bool m_IsActive = true;

	[SerializeField]
	private int m_CurrentIndex;

	[SerializeField]
	private int m_MoveState;

	[SerializeField]
	private int m_Side;

	public bool IsActive => m_IsActive;

	public int CurrentIndex => m_CurrentIndex;

	public int MoveState => m_MoveState;

	public int Side => m_Side;

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
}
