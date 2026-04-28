using System;
using UnityEngine;

public class ObjectiveBettyBox : Objective
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Interactable")]
	[SerializeField]
	private ActionEventController m_ActionEventController;

	protected override void InternalInitialize()
	{
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		if (!CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
			m_ActionEventController.Content.Disable();
		}
	}

	private void HandleActionEventOnActivate(object sender, EventArgs e)
	{
		m_ActionEventController.OnActivate -= HandleActionEventOnActivate;
		SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		m_ActionEventController.Dispose();
		SendOnComplete();
	}

	protected override void InternalComplete()
	{
		RemoveListeners();
	}

	private bool CheckStatus()
	{
		bool flag = true;
		if (m_Requirements != null)
		{
			flag = m_Requirements.IsComplete();
		}
		if (flag)
		{
			GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
			m_ActionEventController.OnActivate -= HandleActionEventOnActivate;
			m_ActionEventController.OnActivate += HandleActionEventOnActivate;
			m_ActionEventController.Content.Enable();
		}
		return flag;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		CheckStatus();
	}

	protected override void RemoveListeners()
	{
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		if (m_ActionEventController != null)
		{
			m_ActionEventController.OnActivate -= HandleActionEventOnActivate;
		}
	}
}
