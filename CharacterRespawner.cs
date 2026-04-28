using System;
using UnityEngine;

public class CharacterRespawner : JMonoBehaviour
{
	[SerializeField]
	private Character m_Character;

	[SerializeField]
	private Transform m_RespawnLocation;

	[SerializeField]
	private EventTrigger m_EventTrigger;

	public void Activate()
	{
		GameManager.Instance.Player.OnRespawn -= HandlePlayerOnRespawn;
		GameManager.Instance.Player.OnRespawn += HandlePlayerOnRespawn;
	}

	public void Deactivate()
	{
		GameManager.Instance.Player.OnRespawn -= HandlePlayerOnRespawn;
		if (m_EventTrigger != null)
		{
			m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
			m_EventTrigger.SetActive(active: false);
		}
	}

	private void HandlePlayerOnRespawn(object sender, EventArgs e)
	{
		if (m_Character != null && m_RespawnLocation != null)
		{
			m_Character.SetState(State.Character.Cutscene);
			m_Character.SetTarget(null);
			m_Character.CancelPath();
			if (m_Character.Content != null)
			{
				m_Character.Content.Animator.SetMovementState(0f, smooth: false);
				m_Character.Content.Animator.SetMovementSpeed(0f, smooth: false);
				m_Character.ClearAnimationTriggers();
				m_Character.Content.Animator.enabled = false;
				m_Character.Content.Animator.enabled = true;
			}
			m_Character.transform.position = m_RespawnLocation.position;
			m_Character.transform.eulerAngles = m_RespawnLocation.eulerAngles;
			if (m_EventTrigger != null)
			{
				m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
				m_EventTrigger.OnEnter += HandleEventTriggerOnEnter;
				m_EventTrigger.ResetAction();
				m_EventTrigger.SetActive(active: true);
			}
			else
			{
				m_Character.SetTarget(GameManager.Instance.Player.transform);
				m_Character.SetState(State.Character.Follow);
				m_Character.Agent.ResetAgent();
			}
		}
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		m_EventTrigger.SetActive(active: false);
		m_Character.SetTarget(GameManager.Instance.Player.transform);
		m_Character.SetState(State.Character.Follow);
		m_Character.Agent.ResetAgent();
	}

	protected override void OnDisposed()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnRespawn -= HandlePlayerOnRespawn;
		}
		if (m_EventTrigger != null)
		{
			m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		}
		base.OnDisposed();
	}
}
