using System;
using UnityEngine;

public class ObjectiveGentSchematic : Objective
{
	[SerializeField]
	private GentSchematic m_GentSchematic;

	protected override void InternalInitialize()
	{
		Enable();
	}

	protected override void InternalEnable()
	{
		AddListeners();
	}

	private void HandleClimbOnActivate(object sender, EventArgs e)
	{
		m_GentSchematic.OnActivate -= HandleClimbOnActivate;
		SendOnComplete();
	}

	protected override void InternalComplete()
	{
		RemoveListeners();
	}

	protected void AddListeners()
	{
		m_GentSchematic.OnActivate += HandleClimbOnActivate;
	}

	protected override void RemoveListeners()
	{
		m_GentSchematic.OnActivate -= HandleClimbOnActivate;
	}
}
