using System;
using UnityEngine;
using UnityEngine.AI;

public class SearcherSpawner : Spawner
{
	[SerializeField]
	private bool m_OnStart;

	[Header("Settings")]
	[SerializeField]
	private LayerMask m_LayerMask;

	[SerializeField]
	private Transform m_SpawnLocation;

	[SerializeField]
	private bool m_LookAtPlayer;

	[Header("Spawn Options")]
	[SerializeField]
	private int m_SpawnLimit = 1;

	[SerializeField]
	private float m_SpawnTimerLimit = 1.5f;

	[SerializeField]
	private float m_SpawnDistance = 8f;

	[SerializeField]
	private bool m_IsSingleSpawn;

	[Header("Enemy Animation Clips")]
	[SerializeField]
	private Animator m_PropAnimator;

	[SerializeField]
	private AnimationClipGroup[] m_EnemySpawnClips;

	[Header("Prefabs")]
	[SerializeField]
	private Enemy[] m_EnemyPrefabs;

	private Enemy m_Enemy;

	private bool m_IsActive;

	private bool m_IsSpawning;

	private float m_SpawnTimer;

	private int m_SpawnCount;

	private float m_Distance => Vector3.Distance(m_SpawnLocation.position, GameManager.Instance.Player.transform.position);

	public override void Start()
	{
		if (m_OnStart)
		{
			Activate();
		}
	}

	public override void Activate()
	{
		GameManager.Instance.Player.OnRespawn -= HandlePlayerOnRespawn;
		GameManager.Instance.Player.OnRespawn += HandlePlayerOnRespawn;
		m_SpawnTimer = m_SpawnTimerLimit;
		m_IsActive = true;
		if (m_SpawnLocation == null)
		{
			m_SpawnLocation = base.transform;
		}
	}

	public override void Deactivate()
	{
		GameManager.Instance.Player.OnRespawn -= HandlePlayerOnRespawn;
		ResetEnemy();
		m_IsActive = false;
		m_IsSpawning = false;
		m_SpawnTimer = m_SpawnTimerLimit;
		m_SpawnCount = 0;
	}

	private void KillEnemy()
	{
		if (m_Enemy != null)
		{
			GameManager.Instance.PoolingManager.GetFromPool("Effects/Effects_Hit_Ink").transform.position = m_Enemy.transform.position + Vector3.up * 3f;
			m_Enemy.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
			m_Enemy.OnCharacterDeath -= HandleEnemyOnCharacterDeath;
			m_Enemy.OnAnimationComplete -= HandleSpawnedEnemyOnAnimationComplete;
			m_Enemy.OnAnimationComplete -= HandleEnemyDespawnOnAnimationComplete;
			m_Enemy.Dispose();
			m_Enemy = null;
		}
	}

	private void ResetEnemy()
	{
		if (m_Enemy != null)
		{
			m_Enemy.SetState(State.Character.Cutscene);
			m_Enemy.DespawnAnimationClip.name = "Interact";
			m_Enemy.Content.UpdateClipOverride(m_Enemy.DespawnAnimationClip);
			m_Enemy.Content.SetAnimationTrigger("Interact");
			m_Enemy.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
			m_Enemy.OnCharacterDeath -= HandleEnemyOnCharacterDeath;
			m_Enemy.OnAnimationComplete -= HandleSpawnedEnemyOnAnimationComplete;
			m_Enemy.OnAnimationComplete -= HandleEnemyDespawnOnAnimationComplete;
			m_Enemy.OnAnimationComplete += HandleEnemyDespawnOnAnimationComplete;
		}
	}

	private void HandleEnemyDespawnOnAnimationComplete(object sender, EventArgs e)
	{
		m_Enemy.OnAnimationComplete -= HandleEnemyDespawnOnAnimationComplete;
		m_Enemy.Dispose();
		m_Enemy = null;
		m_IsSpawning = false;
		m_SpawnTimer = m_SpawnTimerLimit;
		m_SpawnCount = 0;
	}

	private void Update()
	{
		if (!m_IsActive || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (!m_IsSpawning)
		{
			if (m_SpawnCount < m_SpawnLimit)
			{
				m_SpawnTimer += Time.deltaTime;
			}
			if (IsSpawnDistance())
			{
				Spawn();
			}
		}
		else if (GameManager.Instance.Player.CombatStatus == CombatStatus.Hide)
		{
			m_Enemy.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
			m_Enemy.OnCharacterDeath -= HandleEnemyOnCharacterDeath;
			m_Enemy.OnAnimationComplete -= HandleSpawnedEnemyOnAnimationComplete;
			m_Enemy.OnAnimationComplete -= HandleEnemyDespawnOnAnimationComplete;
			m_Enemy.OnAnimationComplete += HandleEnemyDespawnOnAnimationComplete;
			m_IsSpawning = false;
		}
	}

	public override void Spawn()
	{
		if (!m_IsSpawning && m_SpawnCount < m_SpawnLimit && !(m_SpawnTimer < m_SpawnTimerLimit))
		{
			m_IsSpawning = true;
			m_SpawnCount++;
			m_SpawnTimer = 0f;
			m_Enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>(m_EnemyPrefabs[UnityEngine.Random.Range(0, m_EnemyPrefabs.Length)]);
			m_Enemy.SetSection(base.gameObject.GetComponentInParent<Section>().SectionID);
			m_Enemy.transform.position = m_SpawnLocation.position;
			m_Enemy.transform.rotation = m_SpawnLocation.rotation;
			m_Enemy.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
			m_Enemy.OnInitializeOnComplete += HandleEnemyOnInitializeOnComplete;
		}
	}

	public void Disable()
	{
		if (m_Enemy != null)
		{
			m_Enemy.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
			m_Enemy.OnCharacterDeath -= HandleEnemyOnCharacterDeath;
			m_Enemy.OnAnimationComplete -= HandleSpawnedEnemyOnAnimationComplete;
			m_Enemy.OnAnimationComplete -= HandleEnemyDespawnOnAnimationComplete;
			m_Enemy.Dispose();
			m_Enemy = null;
		}
		Deactivate();
	}

	private void HandleEnemyOnInitializeOnComplete(object sender, EventArgs e)
	{
		m_Enemy.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
		m_Enemy.SetState(State.Character.Cutscene);
		m_Enemy.Agent.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
		m_Enemy.Controller.enabled = false;
		m_Enemy.transform.position = m_SpawnLocation.position;
		if (m_LookAtPlayer)
		{
			Vector3 position = GameManager.Instance.Player.transform.position;
			position.y = m_SpawnLocation.position.y;
			Vector3 normalized = (position - m_SpawnLocation.position).normalized;
			m_Enemy.transform.rotation = Quaternion.LookRotation(normalized);
		}
		else
		{
			m_Enemy.transform.rotation = m_SpawnLocation.rotation;
		}
		int num = UnityEngine.Random.Range(0, m_EnemySpawnClips.Length);
		AnimationClipGroup animationClipGroup = m_EnemySpawnClips[num];
		animationClipGroup.Initialize();
		m_Enemy.Content.UpdateClipOverrides(animationClipGroup.AnimationClip);
		m_Enemy.Content.SetAnimationTrigger("InteractInstant");
		if (m_PropAnimator != null)
		{
			m_PropAnimator.SetTrigger("InteractInstant");
		}
		m_Enemy.OnCharacterDeath -= HandleEnemyOnCharacterDeath;
		m_Enemy.OnCharacterDeath += HandleEnemyOnCharacterDeath;
		m_Enemy.OnAnimationComplete -= HandleSpawnedEnemyOnAnimationComplete;
		m_Enemy.OnAnimationComplete += HandleSpawnedEnemyOnAnimationComplete;
		SendOnSpawn();
	}

	private void HandleEnemyOnCharacterDeath(object sender, EventArgs e)
	{
		m_Enemy.OnCharacterDeath -= HandleEnemyOnCharacterDeath;
		SingleSpawn();
		m_Enemy = null;
		if (m_SpawnCount > 0)
		{
			m_SpawnCount--;
		}
		if (m_SpawnCount < 0)
		{
			m_SpawnCount = 0;
		}
		m_SpawnTimer = 0f;
		m_IsSpawning = false;
	}

	private void SingleSpawn()
	{
		if (m_IsSingleSpawn)
		{
			GameManager.Instance.Player.OnRespawn -= HandlePlayerOnRespawn;
			m_IsActive = false;
			m_IsSpawning = false;
			m_SpawnTimer = m_SpawnTimerLimit;
			m_SpawnCount = 0;
			if (m_Enemy != null)
			{
				m_Enemy.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
				m_Enemy.OnCharacterDeath -= HandleEnemyOnCharacterDeath;
				m_Enemy.OnAnimationComplete -= HandleSpawnedEnemyOnAnimationComplete;
				m_Enemy.OnAnimationComplete -= HandleEnemyDespawnOnAnimationComplete;
			}
		}
	}

	private void HandleSpawnedEnemyOnAnimationComplete(object sender, EventArgs e)
	{
		m_Enemy.OnAnimationComplete -= HandleSpawnedEnemyOnAnimationComplete;
		m_Enemy.SetTarget(GameManager.Instance.Player.transform);
		m_Enemy.SetState(State.Character.Follow);
		m_Enemy.Agent.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
		m_Enemy.Agent.ResetAgent();
		m_Enemy.Controller.enabled = true;
	}

	private void HandlePlayerOnRespawn(object sender, EventArgs e)
	{
		if (m_IsActive)
		{
			m_SpawnTimer = m_SpawnTimerLimit;
			m_SpawnCount = 0;
			m_Enemy = null;
			m_IsSpawning = false;
		}
	}

	private bool IsSpawnDistance()
	{
		bool flag = m_Distance < m_SpawnDistance;
		if (flag)
		{
			Vector3 vector = m_SpawnLocation.position + Vector3.up;
			Vector3 normalized = (GameManager.Instance.GameCamera.transform.position - vector).normalized;
			if (Physics.SphereCast(m_SpawnLocation.position + Vector3.up, 0.25f, normalized, out var hitInfo, float.PositiveInfinity, ~(int)m_LayerMask, QueryTriggerInteraction.Ignore))
			{
				if (hitInfo.transform.gameObject.layer != LayerMask.NameToLayer("Player"))
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
		}
		return flag;
	}
}
