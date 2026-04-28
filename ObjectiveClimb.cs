using System;
using UnityEngine;

public class ObjectiveClimb : Objective
{
	[SerializeField]
	private Climb m_Climb;

	[SerializeField]
	private bool m_ClearNavigation = true;

	[SerializeField]
	private bool m_ForceComplete;

	protected override void InternalInitialize()
	{
		Enable();
	}

	protected override void InternalEnable()
	{
		AddListeners();
		m_Climb.Enable();
	}

	protected override void InternalInactive()
	{
		m_Climb.Inactive();
		SendOnComplete();
	}

	protected override void InternalDisable()
	{
		m_Climb.Disable();
	}

	protected override void InternalComplete()
	{
		RemoveListeners();
	}

	protected override void InternalForceComplete()
	{
		if (m_ForceComplete)
		{
			m_Climb.ForceActivateComplete();
			SendOnComplete();
		}
		else
		{
			Inactive();
		}
	}

	private void HandleClimbOnInteract(object sender, EventArgs e)
	{
		m_Climb.OnInteract -= HandleClimbOnInteract;
		GameManager.Instance.ClearNavigation();
	}

	private void HandleClimbOnComplete(object sender, EventArgs e)
	{
		RemoveListeners();
		SendOnComplete();
	}

	protected void AddListeners()
	{
		if (m_ClearNavigation)
		{
			m_Climb.OnInteract += HandleClimbOnInteract;
		}
		m_Climb.OnComplete += HandleClimbOnComplete;
	}

	protected override void RemoveListeners()
	{
		if (m_ClearNavigation)
		{
			m_Climb.OnInteract -= HandleClimbOnInteract;
		}
		m_Climb.OnComplete -= HandleClimbOnComplete;
	}
}
