using System;
using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRDoorActionEventAreaHook : S13ConditionalTrigger
{
	[SerializeField]
	private DoorID m_DoorID;

	[SerializeField]
	private S13LocalAction onActivateAction;

	[SerializeField]
	private S13LocalAction onActivatedAction;

	[SerializeField]
	private S13LocalAction onDeactivateAction;

	[SerializeField]
	private S13LocalAction onDeactivatedAction;

	private ActionEventController targetController;

	protected override void Start()
	{
		base.Start();
		if (m_DoorID == DoorID.NONE)
		{
			return;
		}
		Door[] componentsInChildren = base.transform.root.GetComponentsInChildren<Door>(includeInactive: true);
		if (componentsInChildren == null)
		{
			return;
		}
		foreach (Door door in componentsInChildren)
		{
			if (door.ID == m_DoorID)
			{
				targetController = door;
				break;
			}
		}
	}

	protected override void OnEnter(ConditionInfo colliderInfo)
	{
		if (!(targetController == null))
		{
			targetController.OnActivate += OnActivate;
			targetController.OnActivate += OnActivated;
			targetController.OnDeactivate += OnDeactivate;
			targetController.OnDeactivate += OnDeactivated;
		}
	}

	protected override void OnExit(ConditionInfo colliderInfo)
	{
		if (!(targetController == null))
		{
			targetController.OnActivate -= OnActivate;
			targetController.OnActivate -= OnActivated;
			targetController.OnDeactivate -= OnDeactivate;
			targetController.OnDeactivate -= OnDeactivated;
		}
	}

	private void OnActivate(object obj, EventArgs args)
	{
		if (onActivateAction.IsExecutable)
		{
			onActivateAction.Execute();
		}
	}

	private void OnActivated(object obj, EventArgs args)
	{
		if (onActivatedAction.IsExecutable)
		{
			onActivatedAction.Execute();
		}
	}

	private void OnDeactivate(object obj, EventArgs args)
	{
		if (onDeactivateAction.IsExecutable)
		{
			onDeactivateAction.Execute();
		}
	}

	private void OnDeactivated(object obj, EventArgs args)
	{
		if (onDeactivatedAction.IsExecutable)
		{
			onDeactivatedAction.Execute();
		}
	}
}
