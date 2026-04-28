using System;
using UnityEngine;

public class ObjectiveControllerLink : Objective
{
	[SerializeField]
	private ObjectiveController m_ObjectiveController;

	protected override void InternalInitialize()
	{
		if (!(m_ObjectiveController == null))
		{
			if (m_ObjectiveController.IsComplete)
			{
				ForceComplete();
			}
			else
			{
				m_ObjectiveController.OnComplete += HandleObjectiveControllerOnComplete;
			}
		}
	}

	protected override void InternalForceComplete()
	{
		if (!(m_ObjectiveController == null))
		{
			m_ObjectiveController.ForceComplete();
			SendOnComplete();
		}
	}

	protected override void InternalInactive()
	{
		if (!(m_ObjectiveController == null))
		{
			m_ObjectiveController.Complete();
			SendOnComplete();
		}
	}

	private void HandleObjectiveControllerOnComplete(object sender, EventArgs e)
	{
		m_ObjectiveController.OnComplete -= HandleObjectiveControllerOnComplete;
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (m_ObjectiveController != null)
		{
			m_ObjectiveController.OnComplete -= HandleObjectiveControllerOnComplete;
		}
		base.OnDisposed();
	}
}
