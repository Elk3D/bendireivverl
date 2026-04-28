using System;
using UnityEngine;

public class ObjectiveRadio : Objective
{
	[SerializeField]
	private Radio m_Radio;

	[SerializeField]
	private Door m_Door;

	protected override void InternalEnable()
	{
		Initialize();
	}

	protected override void InternalInitialize()
	{
		InitializeObjective();
	}

	protected override void InternalForceComplete()
	{
		InitializeObjective();
	}

	private void HandleRadioOnInteract(object sender, EventArgs e)
	{
		m_Door.Content.ForceActivate();
	}

	private void HandleRadioOnComplete(object sender, EventArgs e)
	{
		m_Door.Content.ForceDeactivate();
	}

	private void InitializeObjective()
	{
		m_Door.Content.ForceDeactivateComplete();
		RemoveListeners();
		m_Radio.Content.OnInteract += HandleRadioOnInteract;
		m_Radio.Content.OnComplete += HandleRadioOnComplete;
	}

	protected override void RemoveListeners()
	{
		m_Radio.Content.OnInteract -= HandleRadioOnInteract;
		m_Radio.Content.OnComplete -= HandleRadioOnComplete;
	}
}
