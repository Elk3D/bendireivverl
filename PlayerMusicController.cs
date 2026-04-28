using System;
using System.Collections.Generic;
using S13Audio.BATDR;
using UnityEngine;

public class PlayerMusicController : JMonoBehaviour
{
	[SerializeField]
	private BATDRCombatMusic combatMusic;

	private List<Enemy> m_ActiveCharacters = new List<Enemy>();

	private Enemy m_ClosestCharacter;

	private float m_Distance;

	private bool m_IsAlerted;

	public void Initialize()
	{
		GameManager.Instance.Player.OnRespawn -= HandlePlayerOnRespawn;
		GameManager.Instance.Player.OnRespawn += HandlePlayerOnRespawn;
	}

	private void HandlePlayerOnRespawn(object sender, EventArgs e)
	{
		combatMusic.StopCombat();
	}

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat)
		{
			CheckCombat();
		}
		else if (GameManager.Instance.Player.CombatStatus == CombatStatus.None)
		{
			if (GameManager.Instance.Player.PreviousCombatStatus == CombatStatus.Combat)
			{
				if (m_IsAlerted)
				{
					m_IsAlerted = false;
					combatMusic.OnDefeatedEnemies();
				}
			}
			else if (m_IsAlerted)
			{
				m_IsAlerted = false;
				combatMusic.StopCombat();
			}
		}
		else if (GameManager.Instance.Player.CombatStatus == CombatStatus.Hide)
		{
			CheckHide();
		}
		CheckEnemyDistance();
	}

	private void CheckCombat()
	{
		if (!m_IsAlerted && m_ClosestCharacter != null)
		{
			Vector3 end = ((m_ClosestCharacter.Content.EyeSight == null) ? (m_ClosestCharacter.transform.position + Vector3.up * 3f) : m_ClosestCharacter.Content.EyeSight.position);
			if (Physics.Linecast(GameManager.Instance.GameCamera.transform.position, end, out var hitInfo, ~((1 << LayerMask.NameToLayer("InvisibleCollider")) | (1 << LayerMask.NameToLayer("IgnoreInvisibleCollider"))), QueryTriggerInteraction.Ignore) && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("AI") && Vector3.Distance(GameManager.Instance.Player.transform.position, m_ClosestCharacter.transform.position) < 20f)
			{
				m_IsAlerted = true;
				combatMusic.OnAlert(m_ClosestCharacter.EnemyType);
			}
		}
	}

	private void CheckHide()
	{
		if (GameManager.Instance.Player.PreviousCombatStatus == CombatStatus.Combat && m_IsAlerted)
		{
			m_IsAlerted = false;
			combatMusic.OnHiddenFromEnemies();
		}
	}

	private void CheckEnemyDistance()
	{
		Collider[] array = Physics.OverlapSphere(GameManager.Instance.Player.transform.position, 30f, 1 << LayerMask.NameToLayer("AI"));
		m_ActiveCharacters.Clear();
		for (int i = 0; i < array.Length; i++)
		{
			Enemy component = array[i].GetComponent<Enemy>();
			if (component != null)
			{
				m_ActiveCharacters.Add(component);
			}
		}
		m_ClosestCharacter = GetClosest(m_ActiveCharacters.ToArray(), out var minDist);
		if (m_ClosestCharacter != null)
		{
			if (m_ClosestCharacter.EnemyType != EnemyType.LostOne)
			{
				return;
			}
			if (m_ClosestCharacter.Content != null && m_ClosestCharacter.transform != null)
			{
				Vector3 end = ((m_ClosestCharacter.Content.EyeSight == null) ? (m_ClosestCharacter.transform.position + Vector3.up * 3f) : m_ClosestCharacter.Content.EyeSight.position);
				if (Physics.Linecast(GameManager.Instance.GameCamera.transform.position, end, out var hitInfo, ~((1 << LayerMask.NameToLayer("InvisibleCollider")) | (1 << LayerMask.NameToLayer("IgnoreInvisibleCollider"))), QueryTriggerInteraction.Ignore))
				{
					float num = minDist;
					if (hitInfo.transform.gameObject.layer != LayerMask.NameToLayer("AI"))
					{
						num += 15f;
					}
					minDist = Mathf.Lerp(m_Distance, num, 0.75f * Time.deltaTime);
				}
			}
			m_Distance = minDist;
			if (GameManager.Instance.Player.CombatStatus != CombatStatus.Combat)
			{
				if (GameManager.Instance.Player.CombatStatus == CombatStatus.None)
				{
					combatMusic.StopDanger();
				}
				else
				{
					combatMusic.UpdateEnemyDistance(m_Distance);
				}
			}
		}
		else
		{
			combatMusic.StopDanger();
		}
	}

	private Enemy GetClosest(Enemy[] enemies, out float minDist)
	{
		Enemy result = null;
		minDist = 50f;
		Vector3 position = GameManager.Instance.Player.transform.position;
		foreach (Enemy enemy in enemies)
		{
			float num = Vector3.Distance(enemy.transform.position, position);
			if (num < minDist)
			{
				result = enemy;
				minDist = num;
			}
		}
		return result;
	}

	protected override void OnDisposed()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnRespawn -= HandlePlayerOnRespawn;
		}
		base.OnDisposed();
	}
}
