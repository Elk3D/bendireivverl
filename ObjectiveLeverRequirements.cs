using System;
using UnityEngine;

public class ObjectiveLeverRequirements : Objective
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Lever")]
	[SerializeField]
	private Lever m_Lever;

	[Header("Interact Settings")]
	[SerializeField]
	private bool m_ClearNavigation = true;

	protected override void InternalEnable()
	{
		Initialize();
	}

	protected override void InternalInitialize()
	{
		RemoveListeners();
		if (!CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
		}
	}

	protected override void InternalDisable()
	{
		Inactive();
	}

	protected override void InternalInactive()
	{
		RemoveListeners();
		m_Lever.Content.Disable();
	}

	protected override void InternalComplete()
	{
		RemoveListeners();
		m_Lever.Content.Disable();
	}

	protected override void InternalForceComplete()
	{
		m_Lever.Content.ForceActivateComplete();
		SendOnComplete();
	}

	private void HandleLeverOnActivate(object sender, EventArgs e)
	{
		m_Lever.OnActivate -= HandleLeverOnActivate;
		GameManager.Instance.ClearNavigation();
	}

	private void HandleActionEventControllerOnActivated(object sender, EventArgs e)
	{
		m_Lever.OnActivated -= HandleActionEventControllerOnActivated;
		SendOnComplete();
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		CheckStatus();
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
			RemoveListeners();
			AddListeners();
			m_Lever.Content.Enable();
		}
		return flag;
	}

	protected void AddListeners()
	{
		if (m_ClearNavigation)
		{
			m_Lever.OnActivate += HandleLeverOnActivate;
		}
		m_Lever.OnActivated += HandleActionEventControllerOnActivated;
	}

	protected override void RemoveListeners()
	{
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		m_Lever.OnActivate -= HandleLeverOnActivate;
		m_Lever.OnActivated -= HandleActionEventControllerOnActivated;
	}
}
