using System;
using UnityEngine;

public class Sit : JMonoBehaviour
{
	[SerializeField]
	private InteractableAnimationEnter m_Interaction;

	private bool m_IsInactive;

	public override void Awake()
	{
		if (m_Interaction != null)
		{
			m_Interaction.OnInteract -= HandleInteractionOnInteract;
			m_Interaction.OnInteract += HandleInteractionOnInteract;
			m_Interaction.OnInteractionExit -= HandleInteractionActivatedOnInteractionExit;
			m_Interaction.OnInteractionExit += HandleInteractionActivatedOnInteractionExit;
		}
	}

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		bool flag = GameManager.Instance.Player.CombatStatus == CombatStatus.Combat;
		if (flag)
		{
			if (!m_IsInactive)
			{
				if (m_Interaction.IsInteracted)
				{
					m_Interaction.OnInteractionExit -= HandleInteractionOnInteractionExit;
					m_Interaction.OnInteractionExit += HandleInteractionOnInteractionExit;
					m_Interaction.Exit();
				}
				if (m_Interaction.IsActive)
				{
					m_Interaction.SetActive(active: false);
				}
			}
		}
		else if (m_IsInactive && !m_Interaction.IsActive)
		{
			m_Interaction.SetActive(active: true);
		}
		m_IsInactive = flag;
	}

	private void HandleInteractionOnInteractionExit(object sender, EventArgs e)
	{
		InteractableAnimationEnter obj = sender as InteractableAnimationEnter;
		obj.OnInteractionExit -= HandleInteractionOnInteractionExit;
		obj.SetActive(active: false);
	}

	private void HandleInteractionOnInteract(object sender, EventArgs e)
	{
		if (GameManager.Instance.Player.BattleStatus == BattleStatus.None)
		{
			GameManager.Instance.Player.SetBattleStatus(BattleStatus.Sit);
		}
	}

	private void HandleInteractionActivatedOnInteractionExit(object sender, EventArgs e)
	{
		if (GameManager.Instance.Player.BattleStatus == BattleStatus.Sit)
		{
			GameManager.Instance.Player.SetBattleStatus(BattleStatus.None);
		}
	}

	private void RemoveListeners()
	{
		m_Interaction.OnInteract -= HandleInteractionOnInteract;
		m_Interaction.OnInteractionExit -= HandleInteractionActivatedOnInteractionExit;
		m_Interaction.OnInteractionExit -= HandleInteractionOnInteractionExit;
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		base.OnDisposed();
	}
}
