using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionSlowZoneController : SectionController
{
	private EventTrigger[] m_EventTriggers;

	protected override IEnumerator InternalInitialize()
	{
		m_EventTriggers = GetComponentsInChildren<EventTrigger>(includeInactive: true);
		RemoveListeners();
		AddListeners();
		yield return null;
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		if (!(GameManager.Instance.Player == null))
		{
			GameManager.Instance.Player.PlayerMovement.SetMoveSpeed(0.5f, _isSlowed: true);
		}
	}

	private void HandleEventTriggerOnExit(object sender, EventArgs e)
	{
		if (!(GameManager.Instance.Player == null))
		{
			GameManager.Instance.Player.PlayerMovement.ResetMoveSpeed();
		}
	}

	private void AddListeners()
	{
		if (m_EventTriggers != null)
		{
			for (int i = 0; i < m_EventTriggers.Length; i++)
			{
				EventTrigger obj = m_EventTriggers[i];
				obj.OnEnter += HandleEventTriggerOnEnter;
				obj.OnExit += HandleEventTriggerOnExit;
			}
		}
	}

	protected override void RemoveListeners()
	{
		if (m_EventTriggers != null)
		{
			for (int i = 0; i < m_EventTriggers.Length; i++)
			{
				EventTrigger obj = m_EventTriggers[i];
				obj.OnEnter -= HandleEventTriggerOnEnter;
				obj.OnExit -= HandleEventTriggerOnExit;
			}
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		m_EventTriggers = null;
	}
}
