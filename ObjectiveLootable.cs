using System;
using UnityEngine;

public class ObjectiveLootable : Objective
{
	[SerializeField]
	private Loot m_Loot;

	protected override void InternalEnable()
	{
		Initialize();
	}

	protected override void InternalInitialize()
	{
		RemoveListeners();
		m_Loot.OnActivated += HandleLootOnActivated;
	}

	private void HandleLootOnActivated(object sender, EventArgs e)
	{
		m_Loot.OnActivated -= HandleLootOnActivated;
		SendOnComplete();
	}

	protected override void RemoveListeners()
	{
		m_Loot.OnActivated -= HandleLootOnActivated;
	}
}
