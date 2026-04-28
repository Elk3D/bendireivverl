using System;
using DG.Tweening;
using UnityEngine;

public class AmokBattle : JMonoBehaviour
{
	[Serializable]
	public class AmokEnemyGroup
	{
		public Enemy Enemy;

		public CharacterNode CharacterNode;
	}

	[SerializeField]
	private AmokEnemyGroup[] m_Enemies;

	[SerializeField]
	private Door m_Door;

	public void Begin()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
			GameManager.Instance.Player.OnDeath += HandlePlayerOnDeath;
		}
		for (int i = 0; i < m_Enemies.Length; i++)
		{
			AmokEnemyGroup amokEnemyGroup = m_Enemies[i];
			if (amokEnemyGroup.Enemy != null && amokEnemyGroup.Enemy.gameObject.activeSelf && amokEnemyGroup.Enemy.CurrentState != State.Character.Death)
			{
				amokEnemyGroup.Enemy.OnCharacterDeath -= HandleEnemyOnCharacterDeath;
				amokEnemyGroup.Enemy.OnCharacterDeath += HandleEnemyOnCharacterDeath;
			}
		}
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		if (!m_Door.Content.IsActivated)
		{
			m_Door.Content.ForceActivate();
		}
		for (int i = 0; i < m_Enemies.Length; i++)
		{
			AmokEnemyGroup amokEnemyGroup = m_Enemies[i];
			if (amokEnemyGroup.Enemy != null && amokEnemyGroup.Enemy.gameObject.activeSelf && amokEnemyGroup.Enemy.CurrentState != State.Character.Death)
			{
				amokEnemyGroup.Enemy.SetNode(amokEnemyGroup.CharacterNode);
				amokEnemyGroup.Enemy.OnNodeReached -= HandleEnemyOnNodeReached;
				amokEnemyGroup.Enemy.OnNodeReached += HandleEnemyOnNodeReached;
				amokEnemyGroup.Enemy.SetState(State.Character.Patrol);
			}
		}
	}

	private void HandleEnemyOnCharacterDeath(object sender, EventArgs e)
	{
		Enemy obj = sender as Enemy;
		obj.OnCharacterDeath -= HandleEnemyOnCharacterDeath;
		obj.OnNodeReached -= HandleEnemyOnNodeReached;
	}

	private void HandleEnemyOnNodeReached(object sender, EventArgs e)
	{
		Enemy enemy = sender as Enemy;
		enemy.OnNodeReached -= HandleEnemyOnNodeReached;
		enemy.transform.DOMove(enemy.CurrentNode.transform.position, 0.25f);
		enemy.transform.DORotate(enemy.CurrentNode.transform.eulerAngles, 0.25f);
		enemy.Content.Animator.SetMovementState(0f, smooth: false);
		enemy.Content.Animator.SetMovementSpeed(0f, smooth: false);
		enemy.SetState(State.Character.Idle);
	}

	public void End()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		}
		for (int i = 0; i < m_Enemies.Length; i++)
		{
			AmokEnemyGroup amokEnemyGroup = m_Enemies[i];
			if (amokEnemyGroup.Enemy != null)
			{
				amokEnemyGroup.Enemy.OnCharacterDeath -= HandleEnemyOnCharacterDeath;
				amokEnemyGroup.Enemy.OnNodeReached -= HandleEnemyOnNodeReached;
			}
		}
	}

	protected override void OnDisposed()
	{
		End();
		base.OnDisposed();
	}
}
