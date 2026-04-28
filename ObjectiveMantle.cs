using System;
using UnityEngine;

public class ObjectiveMantle : Objective
{
	[SerializeField]
	private Mantle m_Mantle;

	protected override void InternalInitialize()
	{
		if (m_Mantle != null && m_Mantle.FlowMantle != null)
		{
			m_Mantle.FlowMantle.OnEnter -= HandleMantleOnEnter;
			m_Mantle.FlowMantle.OnEnter += HandleMantleOnEnter;
		}
	}

	private void HandleMantleOnEnter(object sender, EventArgs e)
	{
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (m_Mantle != null && m_Mantle.FlowMantle != null)
		{
			m_Mantle.FlowMantle.OnEnter -= HandleMantleOnEnter;
		}
		base.OnDisposed();
	}
}
