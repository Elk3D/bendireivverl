using System;
using UnityEngine;

public class ObjectiveMemo : Objective
{
	[SerializeField]
	private ActionEventController m_ActionEventController;

	[SerializeField]
	private bool m_OnComplete;

	protected override void InternalInitialize()
	{
		if (!(m_ActionEventController == null))
		{
			if (m_OnComplete)
			{
				m_ActionEventController.OnDeactivate -= HandleActionEventControllerOnEvent;
				m_ActionEventController.OnDeactivate += HandleActionEventControllerOnEvent;
			}
			else
			{
				m_ActionEventController.OnActivate -= HandleActionEventControllerOnEvent;
				m_ActionEventController.OnActivate += HandleActionEventControllerOnEvent;
			}
		}
	}

	private void HandleActionEventControllerOnEvent(object sender, EventArgs e)
	{
		m_ActionEventController.OnActivate -= HandleActionEventControllerOnEvent;
		m_ActionEventController.OnDeactivate -= HandleActionEventControllerOnEvent;
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (m_ActionEventController != null)
		{
			m_ActionEventController.OnActivate -= HandleActionEventControllerOnEvent;
			m_ActionEventController.OnDeactivate -= HandleActionEventControllerOnEvent;
		}
		base.OnDisposed();
	}
}
