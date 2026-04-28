using System;
using UnityEngine;

public class ObjectiveActionEvent : Objective
{
	[SerializeField]
	private ActionEvent m_ActionEvent;

	[SerializeField]
	private bool m_SetActiveOnInitialize = true;

	[SerializeField]
	private bool m_CanInterrupt;

	[SerializeField]
	private bool m_DisposeOnForceComplete;

	protected override void InternalUpdate()
	{
		if (GameManager.Instance.Player == null || m_CanInterrupt || base.IsComplete)
		{
			return;
		}
		if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat || GameManager.Instance.Player._inkDemonActive)
		{
			if (m_ActionEvent.IsActive)
			{
				m_ActionEvent.SetActive(active: false);
			}
		}
		else if (!m_ActionEvent.IsActive)
		{
			m_ActionEvent.SetActive(active: true);
		}
	}

	protected override void InternalInitialize()
	{
		Enable();
	}

	protected override void InternalEnable()
	{
		if (!(m_ActionEvent == null))
		{
			m_ActionEvent.OnInteract -= HandleActionEventOnInteract;
			m_ActionEvent.OnInteract += HandleActionEventOnInteract;
			if (m_SetActiveOnInitialize)
			{
				m_ActionEvent.SetActive(active: true);
			}
		}
	}

	private void HandleActionEventOnInteract(object sender, EventArgs e)
	{
		m_ActionEvent.OnInteract -= HandleActionEventOnInteract;
		SendOnComplete();
	}

	protected override void InternalInactive()
	{
		Disable();
	}

	protected override void InternalDisable()
	{
		if (!(m_ActionEvent == null))
		{
			m_ActionEvent.OnInteract -= HandleActionEventOnInteract;
			m_ActionEvent.SetActive(active: false);
		}
	}

	protected override void InternalForceComplete()
	{
		if (m_DisposeOnForceComplete && m_ActionEvent != null)
		{
			m_ActionEvent.Dispose();
		}
		SendOnComplete();
	}

	protected override void InternalComplete()
	{
		RemoveListeners();
	}

	protected override void RemoveListeners()
	{
		if (m_ActionEvent != null)
		{
			m_ActionEvent.OnInteract -= HandleActionEventOnInteract;
		}
	}
}
