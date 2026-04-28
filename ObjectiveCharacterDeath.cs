using System;
using UnityEngine;

[DefaultExecutionOrder(-55)]
public class ObjectiveCharacterDeath : Objective
{
	[SerializeField]
	private Character m_Character;

	protected override void InternalEnable()
	{
		Initialize();
	}

	protected override void InternalInitialize()
	{
		if (m_Character != null)
		{
			Enemy enemy = m_Character as Enemy;
			if (enemy != null)
			{
				enemy.SetSection(base.gameObject.GetComponentInParent<Section>().SectionID);
			}
			m_Character.OnCharacterDeath -= HandleOnCharacterDeath;
			m_Character.OnCharacterDeath += HandleOnCharacterDeath;
			m_Character.SetTarget(GameManager.Instance.Player.transform);
			m_Character.SetState(State.Character.Follow);
			if (m_Character.Agent != null)
			{
				m_Character.Agent.ResetAgent();
			}
		}
	}

	protected override void InternalDisable()
	{
		Inactive();
	}

	protected override void InternalInactive()
	{
		if (m_Character != null)
		{
			m_Character.OnCharacterDeath -= HandleOnCharacterDeath;
			m_Character.OnCharacterDeath += HandleOnCharacterDeath;
		}
	}

	private void HandleOnCharacterDeath(object sender, EventArgs e)
	{
		if (m_Character != null)
		{
			m_Character.OnCharacterDeath -= HandleOnCharacterDeath;
		}
		SendOnComplete();
	}

	protected override void RemoveListeners()
	{
		if (m_Character != null)
		{
			m_Character.OnCharacterDeath -= HandleOnCharacterDeath;
		}
	}
}
