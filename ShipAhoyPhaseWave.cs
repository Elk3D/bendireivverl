using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipAhoyPhaseWave : BossPhase
{
	[Header("Boss")]
	[SerializeField]
	private GameObject m_Boss;

	[SerializeField]
	private CharacterContent m_CharacterContent;

	[SerializeField]
	private AnimationClip m_ShockAnimationClip;

	[Header("Spawn Locations")]
	[SerializeField]
	private Transform[] m_SpawnLocations;

	[Header("Prefabs")]
	[SerializeField]
	private Enemy[] m_EnemyPrefabs;

	private List<Enemy> m_ActivePrefabs;

	private Transform m_CurrentSpawnLocation;

	private float m_Timer;

	private float m_TimerLimit = 1f;

	private int m_MaxWave = 5;

	private int m_MaxWaveCount;

	private int m_Kills;

	private int m_SpawnCount;

	public bool IsActive { get; private set; }

	protected override void InternalInitialize()
	{
		m_SpawnCount = UnityEngine.Random.Range(0, m_SpawnLocations.Length);
		m_ShockAnimationClip.name = "Idle";
		m_CharacterContent.UpdateClipOverride(m_ShockAnimationClip);
		m_CharacterContent.SetAnimationTrigger("Exit");
		m_CharacterContent.Animator.enabled = false;
		m_CharacterContent.Animator.enabled = true;
		SetActive(active: true);
	}

	public void SetActive(bool active)
	{
		IsActive = active;
	}

	private void Update()
	{
		if (!IsActive)
		{
			return;
		}
		if (m_Timer >= m_TimerLimit && m_MaxWaveCount < m_MaxWave)
		{
			if (m_SpawnCount >= m_SpawnLocations.Length)
			{
				m_SpawnCount = 0;
			}
			m_CurrentSpawnLocation = m_SpawnLocations[m_SpawnCount];
			Enemy assetPrefab = m_EnemyPrefabs[UnityEngine.Random.Range(0, m_EnemyPrefabs.Length)];
			Enemy enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>(assetPrefab);
			enemy.SetSection(base.gameObject.GetComponentInParent<Section>().SectionID);
			enemy.AddData();
			enemy.OnCharacterDeath -= HandleEnemyOnCharacterDeath;
			enemy.OnCharacterDeath += HandleEnemyOnCharacterDeath;
			enemy.transform.position = m_CurrentSpawnLocation.position;
			enemy.transform.eulerAngles = m_CurrentSpawnLocation.eulerAngles;
			enemy.SetTarget(GameManager.Instance.Player.transform);
			AddEnemy(enemy);
			m_Timer = 0f;
			m_MaxWaveCount++;
			m_SpawnCount++;
		}
		else
		{
			m_Timer += Time.deltaTime;
		}
	}

	private void HandleEnemyOnCharacterDeath(object sender, EventArgs e)
	{
		Enemy enemy = sender as Enemy;
		enemy.OnCharacterDeath -= HandleEnemyOnCharacterDeath;
		RemoveEnemy(enemy);
		float health = UpgradeCheck.GetHealth();
		if (GameManager.Instance.Player.Health < health)
		{
			CameraEffects.Damage(0f);
		}
		GameManager.Instance.Player.Heal((int)health);
		m_Kills++;
		if (m_Kills >= m_MaxWave)
		{
			Complete();
		}
	}

	protected override void InternalComplete()
	{
		m_Boss.SetActive(value: true);
		m_MaxWaveCount = 0;
		m_Timer = 0f;
		m_Kills = 0;
		ClearEnemies();
		SetActive(active: false);
	}

	private void AddEnemy(Enemy enemy)
	{
		if (m_ActivePrefabs == null)
		{
			m_ActivePrefabs = new List<Enemy>();
		}
		m_ActivePrefabs.Add(enemy);
	}

	private void RemoveEnemy(Enemy enemy)
	{
		if (m_ActivePrefabs != null)
		{
			m_ActivePrefabs.Remove(enemy);
		}
	}

	private void ClearEnemies()
	{
		if (m_ActivePrefabs == null)
		{
			return;
		}
		for (int i = 0; i < m_ActivePrefabs.Count; i++)
		{
			Enemy enemy = m_ActivePrefabs[i];
			if (enemy != null)
			{
				enemy.OnCharacterDeath -= HandleEnemyOnCharacterDeath;
			}
		}
		m_ActivePrefabs.Clear();
		m_ActivePrefabs = null;
	}

	protected override void OnDisposed()
	{
		ClearEnemies();
		m_CurrentSpawnLocation = null;
		base.OnDisposed();
	}
}
