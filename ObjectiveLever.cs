using System;
using UnityEngine;

public class ObjectiveLever : Objective
{
	[SerializeField]
	private Lever m_Lever;

	[SerializeField]
	private bool m_ClearNavigation = true;

	[SerializeField]
	private bool m_EnableOnInitialize = true;

	[SerializeField]
	private bool m_DontSendCompleteOnForceComplete;

	protected override void InternalEnable()
	{
		Initialize();
	}

	protected override void InternalInitialize()
	{
		RemoveListeners();
		AddListeners();
		if (m_EnableOnInitialize)
		{
			m_Lever.Content.Enable();
			CheckInteractableInactive(isActive: true);
		}
		else
		{
			m_Lever.Content.Disable();
			CheckInteractableInactive(isActive: false);
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
		CheckInteractableInactive(isActive: false);
	}

	protected override void InternalComplete()
	{
		RemoveListeners();
		m_Lever.Content.Disable();
		CheckInteractableInactive(isActive: false);
	}

	protected override void InternalForceComplete()
	{
		m_Lever.Content.ForceActivateComplete();
		CheckInteractableInactive(isActive: false);
		if (!m_DontSendCompleteOnForceComplete)
		{
			SendOnComplete();
		}
	}

	private void CheckInteractableInactive(bool isActive)
	{
		InteractableInactive componentInChildren = m_Lever.GetComponentInChildren<InteractableInactive>(includeInactive: true);
		if (componentInChildren != null)
		{
			if (isActive)
			{
				componentInChildren.InitializeInactive();
			}
			else
			{
				componentInChildren.DisableInitialize();
			}
		}
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
		m_Lever.OnActivate -= HandleLeverOnActivate;
		m_Lever.OnActivated -= HandleActionEventControllerOnActivated;
	}
}
