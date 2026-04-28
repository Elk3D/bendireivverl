using System;
using UnityEngine;

public class ObjectiveGentRecharger : Objective
{
	[Header("Gent Recharger")]
	[SerializeField]
	private GentRecharger m_GentRecharger;

	[SerializeField]
	private bool m_IsBattery;

	[SerializeField]
	private bool m_IsDisable;

	protected override void InternalEnable()
	{
		if (m_IsDisable)
		{
			JDebug.Log("GentRecharger :: InternalEnable", this, JDebug.JDebugType.Objectives);
			m_GentRecharger.SetGentRecharger(active: true);
		}
	}

	protected override void InternalDisable()
	{
		if (m_IsDisable)
		{
			JDebug.Log("GentRecharger :: InternalDisable", this, JDebug.JDebugType.Objectives);
			m_GentRecharger.SetGentRecharger(active: false);
		}
	}

	protected override void InternalInitialize()
	{
		if (!m_IsDisable)
		{
			JDebug.Log("GentRecharger :: InternalInitialize", this, JDebug.JDebugType.Objectives);
			if (!m_IsBattery)
			{
				m_GentRecharger.InteractableCharger.OnInteractionExit -= HandleGentRechargerOnInteractionExit;
				m_GentRecharger.InteractableCharger.OnInteractionExit += HandleGentRechargerOnInteractionExit;
			}
			else
			{
				m_GentRecharger.InteractableBatteries.OnInteract -= HandleGentRechargerBatteryOnInteract;
				m_GentRecharger.InteractableBatteries.OnInteract += HandleGentRechargerBatteryOnInteract;
			}
		}
	}

	private void HandleGentRechargerOnInteractionExit(object sender, EventArgs e)
	{
		JDebug.Log("GentRecharger :: HandleGentRechargerOnInteractionExit", this, JDebug.JDebugType.Objectives);
		m_GentRecharger.InteractableCharger.OnInteractionExit -= HandleGentRechargerOnInteractionExit;
		SendOnComplete();
	}

	private void HandleGentRechargerBatteryOnInteract(object sender, EventArgs e)
	{
		JDebug.Log("GentRecharger :: HandleGentRechargerOnInteractionExit", this, JDebug.JDebugType.Objectives);
		m_GentRecharger.InteractableBatteries.OnInteract -= HandleGentRechargerBatteryOnInteract;
		SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		JDebug.Log("GentRecharger :: InternalForceComplete", this, JDebug.JDebugType.Objectives);
		if (m_IsDisable)
		{
			m_GentRecharger.SetGentRecharger(active: true);
		}
		RemoveListeners();
		SendOnComplete();
	}

	protected override void InternalComplete()
	{
		JDebug.Log("GentRecharger :: InternalComplete", this, JDebug.JDebugType.Objectives);
		RemoveListeners();
	}

	protected override void RemoveListeners()
	{
		if (m_GentRecharger != null)
		{
			m_GentRecharger.InteractableCharger.OnInteractionExit -= HandleGentRechargerOnInteractionExit;
			m_GentRecharger.InteractableBatteries.OnInteract -= HandleGentRechargerBatteryOnInteract;
		}
	}
}
