using System;
using UnityEngine;

public class ObjectiveSwitch : Objective
{
	[SerializeField]
	private Switch m_Switch;

	protected override void InternalEnable()
	{
		Initialize();
	}

	protected override void InternalInitialize()
	{
		RemoveListeners();
		AddListeners();
		m_Switch.Content.Enable();
	}

	protected override void InternalDisable()
	{
		Inactive();
	}

	protected override void InternalInactive()
	{
		RemoveListeners();
		m_Switch.Content.Disable();
	}

	protected override void InternalComplete()
	{
		RemoveListeners();
		m_Switch.Content.Disable();
	}

	protected override void InternalForceComplete()
	{
		m_Switch.Content.ForceActivateComplete();
		SendOnComplete();
	}

	private void HandleActionEventControllerOnActivated(object sender, EventArgs e)
	{
		m_Switch.OnActivated -= HandleActionEventControllerOnActivated;
		SendOnComplete();
	}

	protected void AddListeners()
	{
		m_Switch.OnActivated += HandleActionEventControllerOnActivated;
	}

	protected override void RemoveListeners()
	{
		m_Switch.OnActivated -= HandleActionEventControllerOnActivated;
	}
}
