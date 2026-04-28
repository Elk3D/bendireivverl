using System;
using UnityEngine;

public class ObjectiveConcertTicket : Objective
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Interactable")]
	[SerializeField]
	private ActionEvent m_ActionEvent;

	protected override void InternalInitialize()
	{
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		if (!CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
			m_ActionEvent.SetActive(active: false);
		}
	}

	private void HandleActionEventOnInteract(object sender, EventArgs e)
	{
		m_ActionEvent.OnInteract -= HandleActionEventOnInteract;
		SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		m_ActionEvent.Dispose();
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
			m_ActionEvent.OnInteract -= HandleActionEventOnInteract;
			m_ActionEvent.OnInteract += HandleActionEventOnInteract;
			m_ActionEvent.SetActive(active: true);
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
		if (m_ActionEvent != null)
		{
			m_ActionEvent.OnInteract -= HandleActionEventOnInteract;
		}
	}
}
