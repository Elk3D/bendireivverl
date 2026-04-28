using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class InkBulb : JMonoBehaviour, IHittable, ISpawner
{
	[SerializeField]
	private LayerMask m_LayerMask;

	[Header("Health Settings")]
	[Header("Easy")]
	[SerializeField]
	private int m_MaxHitPointsEasy = 3;

	[Header("Normal")]
	[SerializeField]
	private int m_MaxHitPoints = 5;

	[Header("Hard")]
	[SerializeField]
	private int m_MaxHitPointsHard = 7;

	[Header("Spawn Options")]
	[SerializeField]
	private bool m_CanSpawn = true;

	[SerializeField]
	private bool m_AutoSpawn;

	[SerializeField]
	private int m_SpawnLimit = 5;

	[SerializeField]
	private float m_SpawnTimerLimit = 1.5f;

	[SerializeField]
	private float m_SpawnDistance = 8f;

	[SerializeField]
	private float m_ActiveDistance = 16f;

	[SerializeField]
	private float m_InactiveDistance = 18f;

	[Header("Animation Clips")]
	[SerializeField]
	private AnimationClipGroup m_ShakeClip;

	[SerializeField]
	private AnimationClipGroup m_SpawnClip;

	[SerializeField]
	private AnimationClipGroup m_InkWidowSpawnClip;

	[Header("Prefabs")]
	[SerializeField]
	private Character[] m_InkWidowPrefabs;

	private CharacterContent m_CharacterContent;

	private List<Character> m_InkWidows = new List<Character>();

	private bool m_IsActive;

	private bool m_IsSpawning;

	private bool m_IsDead;

	private bool m_PauseAutoSpawn;

	private float m_HitType;

	private float m_LastHitType;

	private float[] m_HitTypes = new float[2] { 0f, 1f };

	private float m_SpawnTimer;

	private int m_Health;

	private int m_SpawnCount;

	public int MaxHitPoints
	{
		get
		{
			int result = m_MaxHitPoints;
			switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
			{
			case DifficultyLevel.Easy:
				result = m_MaxHitPointsEasy;
				break;
			case DifficultyLevel.Hard:
				result = m_MaxHitPointsHard;
				break;
			}
			return result;
		}
	}

	public List<Character> InkWidows => m_InkWidows;

	private float m_Distance => Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position);

	public bool IsBroken { get; private set; }

	public bool IsPlayerBreakable => true;

	public event EventHandler OnFlee;

	public event EventHandler OnHit;

	public override void Start()
	{
		m_CharacterContent = GetComponentInChildren<CharacterContent>();
		m_CharacterContent.GenericAnimationEvents.SetReciever(this);
		m_SpawnTimer = m_SpawnTimerLimit;
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
			GameManager.Instance.Player.OnDeath += HandlePlayerOnDeath;
			GameManager.Instance.Player.OnRespawn -= HandlePlayerOnRespawn;
			GameManager.Instance.Player.OnRespawn += HandlePlayerOnRespawn;
		}
	}

	private void Update()
	{
		if (m_PauseAutoSpawn || m_IsDead || m_IsSpawning || base.IsDisposed || GameManager.Instance.IsPaused || m_Health >= MaxHitPoints)
		{
			return;
		}
		m_SpawnTimer += Time.deltaTime;
		if (m_IsActive)
		{
			if (m_AutoSpawn)
			{
				Spawn();
			}
			else if (IsSpawnDistance())
			{
				Spawn();
			}
			else if (IsInactiveDistance())
			{
				m_IsActive = false;
			}
		}
		else if (!m_AutoSpawn && IsActiveDistance())
		{
			Shake();
		}
	}

	public void SetActive(bool active)
	{
		m_IsActive = active;
		m_CanSpawn = active;
	}

	public void Shake()
	{
		if (!m_IsDead && !m_IsSpawning)
		{
			m_IsActive = true;
			m_ShakeClip.Initialize();
			m_CharacterContent.UpdateClipOverrides(m_ShakeClip.AnimationClip);
			m_CharacterContent.SetAnimationTrigger("Interact");
		}
	}

	public void Spawn()
	{
		if (m_CanSpawn && !m_IsDead && !m_IsSpawning && m_SpawnCount < m_SpawnLimit && !(m_SpawnTimer < m_SpawnTimerLimit))
		{
			m_IsSpawning = true;
			m_SpawnCount++;
			m_SpawnTimer = 0f;
			Vector3 vector = new Vector3(GetRandomCircle(1.5f, 2f), 0f, GetRandomCircle(1.5f, 2f));
			vector += base.transform.position;
			Vector3 euler = new Vector3(0f, UnityEngine.Random.Range(-45f, 45f), 0f);
			euler += base.transform.eulerAngles;
			if (NavMesh.SamplePosition(vector, out var hit, 15f, -1))
			{
				vector = hit.position;
			}
			Character character = GameManager.Instance.AssetManager.CreateAsset<Character>(m_InkWidowPrefabs[UnityEngine.Random.Range(0, m_InkWidowPrefabs.Length)]);
			character.transform.position = vector;
			character.transform.rotation = Quaternion.Euler(euler);
			m_InkWidows.Add(character);
			character.OnInitializeOnComplete -= HandleInkWidowOnInitializeOnComplete;
			character.OnInitializeOnComplete += HandleInkWidowOnInitializeOnComplete;
		}
	}

	private void HandleInkWidowOnInitializeOnComplete(object sender, EventArgs e)
	{
		Character character = sender as Character;
		character.OnInitializeOnComplete -= HandleInkWidowOnInitializeOnComplete;
		if (m_CanSpawn)
		{
			character.SetState(State.Character.Cutscene);
			character.Agent.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
			character.Content.Model.transform.position = base.transform.position;
			character.Content.Model.transform.eulerAngles = base.transform.eulerAngles;
			m_SpawnClip.Initialize();
			m_CharacterContent.UpdateClipOverrides(m_SpawnClip.AnimationClip);
			m_CharacterContent.SetAnimationTrigger("Interact");
			m_InkWidowSpawnClip.Initialize();
			character.Content.UpdateClipOverrides(m_InkWidowSpawnClip.AnimationClip);
			character.Content.SetAnimationTrigger("InteractInstant");
			character.OnCharacterDeath -= HandleInkWidowOnCharacterDeath;
			character.OnCharacterDeath += HandleInkWidowOnCharacterDeath;
			character.OnAnimationEnter -= HandleSpawnedInkWidowOnAnimationEnter;
			character.OnAnimationEnter += HandleSpawnedInkWidowOnAnimationEnter;
			character.OnAnimationComplete -= HandleSpawnedInkWidowOnAnimationComplete;
			character.OnAnimationComplete += HandleSpawnedInkWidowOnAnimationComplete;
			character.OnTargetLost -= HandleSpawnedInkWidowOnLostTarget;
			character.OnTargetLost += HandleSpawnedInkWidowOnLostTarget;
		}
		else
		{
			KillInkWidow(character);
		}
	}

	private void HandleInkWidowOnCharacterDeath(object sender, EventArgs e)
	{
		Character character = (Character)sender;
		character.OnCharacterDeath -= HandleInkWidowOnCharacterDeath;
		RemoveInkWidow(character);
	}

	private void HandleSpawnedInkWidowOnAnimationEnter(object sender, EventArgs e)
	{
		Character character = (Character)sender;
		character.OnAnimationEnter -= HandleSpawnedInkWidowOnAnimationEnter;
		character.Content.Model.transform.DOKill();
		if (m_CanSpawn)
		{
			character.Content.Model.transform.DOLocalMove(new Vector3(0f, 0f, -2.15f), 0.25f).SetEase(Ease.Linear);
			character.Content.Model.transform.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.Linear);
		}
		else
		{
			KillInkWidow(character);
		}
	}

	private void HandleSpawnedInkWidowOnAnimationComplete(object sender, EventArgs e)
	{
		Character character = (Character)sender;
		character.OnAnimationComplete -= HandleSpawnedInkWidowOnAnimationComplete;
		if (m_CanSpawn)
		{
			character.Content.Model.transform.localPosition = Vector3.zero;
			character.Content.Model.transform.localEulerAngles = Vector3.zero;
			character.SetTarget(GameManager.Instance.Player.transform);
			character.SetState(State.Character.Follow);
			character.Agent.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
			character.Agent.ResetAgent();
		}
		else
		{
			KillInkWidow(character);
		}
	}

	private void HandleSpawnedInkWidowOnLostTarget(object sender, EventArgs e)
	{
		((Character)sender).OnTargetLost -= HandleSpawnedInkWidowOnLostTarget;
		this.OnFlee.Send(this);
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		if (m_AutoSpawn)
		{
			m_PauseAutoSpawn = true;
		}
		ClearInkWidows();
	}

	private void HandlePlayerOnRespawn(object sender, EventArgs e)
	{
		if (m_AutoSpawn)
		{
			m_PauseAutoSpawn = false;
		}
		m_SpawnTimer = m_SpawnTimerLimit;
		m_SpawnCount = 0;
		if (m_InkWidows != null)
		{
			for (int i = 0; i < m_InkWidows.Count; i++)
			{
				Character character = m_InkWidows[i];
				character.SetTarget(GameManager.Instance.Player.transform);
				character.SetState(State.Character.Follow);
			}
		}
	}

	public void AnimationComplete()
	{
		m_IsSpawning = false;
	}

	public void Hit(RaycastHit hit)
	{
		GameManager.Instance.GameCamera.ShakeCamera(0.2f, 1.75f, 5, 90f, fadeOut: false);
		GameManager.Instance.PoolingManager.GetFromPool("Effects/Effects_Hit_Ink").transform.position = hit.point;
		if (m_IsDead)
		{
			return;
		}
		object[] modelRenderers = m_CharacterContent.ModelRenderers;
		ShaderEffects.Fade("_PostHitGlow", 2f, 0.3f, 0.75f, modelRenderers);
		this.OnHit.Send(this);
		if (m_IsSpawning)
		{
			return;
		}
		m_Health++;
		if (m_Health >= MaxHitPoints)
		{
			m_IsDead = true;
			m_CharacterContent.SetAnimationTrigger("Death");
			Collider[] componentsInChildren = base.transform.GetComponentsInChildren<Collider>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}
		else if (!m_IsSpawning)
		{
			CheckHitType();
			m_CharacterContent.Animator.Hit(m_HitType);
		}
	}

	public void Activate()
	{
		m_CanSpawn = true;
		m_SpawnTimer = 0f;
		m_SpawnDistance = 8f;
		m_ActiveDistance = 18f;
		m_InactiveDistance = 20f;
	}

	public void Deactivate()
	{
		m_CanSpawn = false;
		m_IsSpawning = false;
		m_SpawnTimer = 0f;
		m_SpawnCount = 0;
		m_SpawnDistance = 0f;
		m_ActiveDistance = 9f;
		m_InactiveDistance = 10f;
	}

	public void Death()
	{
		m_Health = MaxHitPoints;
		m_IsDead = true;
		m_CanSpawn = false;
		m_IsSpawning = false;
		m_CharacterContent.SetAnimationTrigger("Death");
	}

	private void ClearInkWidows()
	{
		if (m_InkWidows == null)
		{
			return;
		}
		for (int i = 0; i < m_InkWidows.Count; i++)
		{
			Character character = m_InkWidows[i];
			if (character != null)
			{
				character.OnInitializeOnComplete -= HandleInkWidowOnInitializeOnComplete;
				character.OnCharacterDeath -= HandleInkWidowOnCharacterDeath;
				character.OnAnimationEnter -= HandleSpawnedInkWidowOnAnimationEnter;
				character.OnAnimationComplete -= HandleSpawnedInkWidowOnAnimationComplete;
				character.OnTargetLost -= HandleSpawnedInkWidowOnLostTarget;
				character.Dispose();
			}
		}
		m_InkWidows.Clear();
	}

	public void RemoveInkWidow(Character inkWidow)
	{
		if (inkWidow == null)
		{
			return;
		}
		if (m_InkWidows.Contains(inkWidow))
		{
			m_InkWidows.Remove(inkWidow);
		}
		if (m_CanSpawn)
		{
			if (m_SpawnCount > 0)
			{
				m_SpawnCount--;
			}
			if (m_SpawnCount < 0)
			{
				m_SpawnCount = 0;
			}
		}
	}

	public void KillInkWidow(Character inkWidow)
	{
		if (!(inkWidow == null))
		{
			RemoveInkWidow(inkWidow);
			inkWidow.Dispose();
		}
	}

	private bool IsSpawnDistance()
	{
		bool flag = m_Distance < m_SpawnDistance;
		if (flag)
		{
			Vector3 vector = base.transform.position + Vector3.up;
			Vector3 normalized = (GameManager.Instance.GameCamera.transform.position - vector).normalized;
			if (Physics.SphereCast(base.transform.position + Vector3.up, 0.25f, normalized, out var hitInfo, float.PositiveInfinity, ~(int)m_LayerMask, QueryTriggerInteraction.Ignore))
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

	private bool IsInactiveDistance()
	{
		return m_Distance > m_InactiveDistance;
	}

	private bool IsActiveDistance()
	{
		return m_Distance < m_ActiveDistance;
	}

	private float GetRandomCircle(float min, float max)
	{
		if (UnityEngine.Random.value < 0.5f)
		{
			return UnityEngine.Random.Range(min, max);
		}
		return UnityEngine.Random.Range(0f - min, 0f - max);
	}

	private void CheckHitType()
	{
		while (m_HitType == m_LastHitType)
		{
			m_HitType = m_HitTypes[UnityEngine.Random.Range(0, m_HitTypes.Length)];
		}
		m_LastHitType = m_HitType;
	}

	protected override void OnDisposed()
	{
		this.OnFlee = null;
		this.OnHit = null;
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
			GameManager.Instance.Player.OnRespawn -= HandlePlayerOnRespawn;
		}
		ClearInkWidows();
		m_InkWidows = null;
		m_CharacterContent = null;
		base.OnDisposed();
	}
}
