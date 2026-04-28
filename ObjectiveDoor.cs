using System;
using UnityEngine;

public class ObjectiveDoor : Objective
{
	[SerializeField]
	private Door m_Door;

	[SerializeField]
	private bool m_ForceComplete;

	[SerializeField]
	private bool m_EnabledOnComplete = true;

	[SerializeField]
	private bool m_InitializeDisables;

	[SerializeField]
	private bool m_CheckInactive;

	[SerializeField]
	private DoorType m_DoorOpenType;

	protected override void InternalInitialize()
	{
		RemoveListeners();
		CheckEnableOnComplete();
		if (!m_InitializeDisables)
		{
			if (m_CheckInactive)
			{
				if (!m_Door.Content.IsActivated || m_Door.Content.IsPlaying)
				{
					m_Door.Content.ForceActivate();
				}
			}
			else
			{
				m_Door.Content.ForceActivate();
			}
		}
		else if (m_CheckInactive)
		{
			if (m_Door.Content.IsActivated || m_Door.Content.IsPlaying)
			{
				m_Door.Content.ForceDeactivate();
			}
		}
		else
		{
			m_Door.Content.ForceDeactivate();
		}
		SendOnComplete();
	}

	protected override void InternalEnable()
	{
		RemoveListeners();
		m_Door.OnActivated += HandleDoorOnActivated;
		if (m_Door.Data.Status == DoorStatus.Closed || m_Door.Data.Status == DoorStatus.Open)
		{
			m_Door.Content.Enable();
		}
	}

	protected override void InternalInactive()
	{
		RemoveListeners();
		m_Door.OnInactive += HandleActionDoorOnInactive;
		m_Door.Content.ForceInactive();
	}

	protected override void InternalDisable()
	{
		RemoveListeners();
		m_Door.Content.Disable();
	}

	protected override void InternalComplete()
	{
		RemoveListeners();
	}

	protected override void InternalForceComplete()
	{
		if (m_ForceComplete)
		{
			if (!m_InitializeDisables)
			{
				if (m_DoorOpenType != DoorType.Single)
				{
					m_Door.Data.SetType(m_DoorOpenType);
				}
				if (m_Door.Content.IsActivated)
				{
					m_Door.Content.ForceDeactivateComplete();
				}
				if (!m_Door.Content.IsActivated)
				{
					m_Door.Content.ForceActivateComplete();
				}
			}
			else if (m_Door.Content.IsActivated)
			{
				m_Door.Content.ForceDeactivateComplete();
			}
		}
		CheckEnableOnComplete();
		SendOnComplete();
	}

	private void CheckEnableOnComplete()
	{
		if (m_EnabledOnComplete)
		{
			m_Door.Content.Enable();
		}
		else
		{
			m_Door.Content.Disable();
		}
	}

	private void HandleDoorOnActivated(object sender, EventArgs e)
	{
		RemoveListeners();
		SendOnComplete();
	}

	private void HandleActionDoorOnInactive(object sender, EventArgs e)
	{
		RemoveListeners();
		SendOnComplete();
	}

	protected override void RemoveListeners()
	{
		if (m_Door != null)
		{
			m_Door.OnActivate -= HandleDoorOnActivated;
			m_Door.OnActivated -= HandleDoorOnActivated;
			m_Door.OnInactive -= HandleActionDoorOnInactive;
		}
	}
}
