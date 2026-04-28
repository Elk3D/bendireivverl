using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class PiperRespawn : ButcherGangSpawner
{
	[Header("Trigger")]
	[SerializeField]
	private EventTrigger m_EventTrigger;

	[Header("Locations")]
	[SerializeField]
	private Transform m_EnterLocation;

	[SerializeField]
	private Transform m_ExitLocation;

	[Header("Effects")]
	[SerializeField]
	private GameObject m_Particles;

	[Header("Enemy Animation Clips")]
	[SerializeField]
	private AnimationClipGroup m_EnemySpawnClip;

	[SerializeField]
	private AnimationClipGroup m_EnemyReturnClip;

	[Header("Prefabs")]
	[SerializeField]
	private Enemy m_Prefab;

	private Enemy m_Enemy;

	private Sequence m_ActiveSequence;

	private float m_ActiveTimer;

	private float m_ActiveTimerLimit = 7.5f;

	private float m_BrokenPathTimer;

	private float m_BrokenPathTimerLimit = 0.75f;

	private void Update()
	{
		if (!base.IsSpawned || m_Enemy == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (m_ActiveTimer >= m_ActiveTimerLimit || GameManager.Instance.Player.CombatStatus == CombatStatus.Hide)
		{
			Return();
			return;
		}
		m_ActiveTimer += Time.deltaTime;
		if (!(m_Enemy.Target == GameManager.Instance.Player.transform))
		{
			return;
		}
		if (m_Enemy.Agent.Agent.pathStatus == NavMeshPathStatus.PathInvalid || m_Enemy.Agent.Agent.pathStatus == NavMeshPathStatus.PathPartial)
		{
			if (m_BrokenPathTimer >= m_BrokenPathTimerLimit)
			{
				Return();
			}
			else
			{
				m_BrokenPathTimer += Time.deltaTime;
			}
		}
		else
		{
			m_BrokenPathTimer = 0f;
		}
	}

	protected override void Enable()
	{
		m_Particles.SetActive(value: true);
		m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		m_EventTrigger.OnEnter += HandleEventTriggerOnEnter;
		m_EventTrigger.SetActive(active: true);
		m_EventTrigger.ResetAction();
		SendOnEnabled();
	}

	protected override void Disable()
	{
		m_Particles.SetActive(value: false);
		m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		m_EventTrigger.SetActive(active: false);
		SendOnDisabled();
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		m_EventTrigger.SetActive(active: false);
		m_Particles.SetActive(value: false);
		m_Enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>(m_Prefab);
		m_Enemy.SetSection(base.gameObject.GetComponentInParent<Section>().SectionID);
		m_Enemy.transform.position = m_EnterLocation.position;
		m_Enemy.transform.rotation = m_EnterLocation.rotation;
		m_Enemy.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
		m_Enemy.OnInitializeOnComplete += HandleEnemyOnInitializeOnComplete;
		ResetActiveSequence();
		m_ActiveSequence.InsertCallback(0.2f, delegate
		{
			CameraEffects.Damage();
			Vector3 position = base.transform.position;
			position.y = GameManager.Instance.Player.transform.position.y;
			GameManager.Instance.Player.PlayerMovement.SetPreviouslyGrounded(active: true);
			GameManager.Instance.Player.AddForce((GameManager.Instance.Player.transform.position - position).normalized * 25f);
			GameManager.Instance.Player.PlayerMovement.SetPreviouslyGrounded(active: true);
		});
		SendOnSpawned();
	}

	private void HandleEnemyOnInitializeOnComplete(object sender, EventArgs e)
	{
		m_Enemy.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
		m_Enemy.SetState(State.Character.Cutscene);
		m_Enemy.Agent.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
		base.IsSpawned = true;
		m_Enemy.Controller.enabled = false;
		m_Enemy.transform.position = m_EnterLocation.position;
		m_Enemy.transform.rotation = m_EnterLocation.rotation;
		m_EnemySpawnClip.Initialize();
		m_Enemy.Content.UpdateClipOverrides(m_EnemySpawnClip.AnimationClip);
		m_Enemy.Content.SetAnimationTrigger("InteractInstant");
		m_Enemy.OnAnimationComplete -= HandleSpawnedEnemyOnAnimationComplete;
		m_Enemy.OnAnimationComplete += HandleSpawnedEnemyOnAnimationComplete;
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

	private void Return()
	{
		base.IsSpawned = false;
		m_ActiveTimer = 0f;
		m_Enemy.Controller.enabled = false;
		m_Enemy.ForceStop();
		m_Enemy.SetDisableVision(active: true);
		m_Enemy.SetNode(m_ExitLocation.GetComponent<CharacterNode>());
		m_Enemy.SetTarget(m_ExitLocation);
		m_Enemy.SetState(State.Character.Flee);
		m_Enemy.OnNodeReached -= HandleEnemyOnNodeReached;
		m_Enemy.OnNodeReached += HandleEnemyOnNodeReached;
	}

	private void HandleEnemyOnNodeReached(object sender, EventArgs e)
	{
		m_Enemy.OnNodeReached -= HandleEnemyOnNodeReached;
		m_Enemy.SetState(State.Character.Cutscene);
		m_Enemy.Agent.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
		m_Enemy.Controller.enabled = false;
		ResetActiveSequence();
		m_ActiveSequence.Insert(0f, m_Enemy.transform.DOMove(m_ExitLocation.position, 0.1f).SetEase(Ease.Linear));
		m_ActiveSequence.Insert(0f, m_Enemy.transform.DORotate(m_ExitLocation.eulerAngles, 0.1f).SetEase(Ease.Linear));
		m_EnemyReturnClip.Initialize();
		m_Enemy.Content.UpdateClipOverrides(m_EnemyReturnClip.AnimationClip);
		m_Enemy.Content.SetAnimationTrigger("InteractInstant");
		m_Enemy.OnAnimationComplete -= HandleEnemyReturnOnAnimationComplete;
		m_Enemy.OnAnimationComplete += HandleEnemyReturnOnAnimationComplete;
		StartCoroutine(ClearEnemyDelay());
	}

	private IEnumerator ClearEnemyDelay()
	{
		yield return new WaitForSeconds(5f);
		yield return new WaitForEndOfFrame();
		if (!base.IsSpawned)
		{
			ClearEnemy();
			SendOnReturned();
		}
	}

	private void HandleEnemyReturnOnAnimationComplete(object sender, EventArgs e)
	{
		m_Enemy.OnAnimationComplete -= HandleEnemyReturnOnAnimationComplete;
		ClearEnemy();
		SendOnReturned();
		StopCoroutine(ClearEnemyDelay());
	}

	private void ClearEnemy()
	{
		if (m_Enemy != null)
		{
			m_Enemy.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
			m_Enemy.OnAnimationComplete -= HandleSpawnedEnemyOnAnimationComplete;
			m_Enemy.OnAnimationComplete -= HandleEnemyReturnOnAnimationComplete;
			m_Enemy.Dispose();
			m_Enemy = null;
		}
	}

	private void ResetActiveSequence()
	{
		KillActiveSequence();
		m_ActiveSequence = DOTween.Sequence();
	}

	private void KillActiveSequence()
	{
		if (m_ActiveSequence != null)
		{
			m_ActiveSequence.Kill();
			m_ActiveSequence = null;
		}
	}

	public override void Cancel()
	{
		KillActiveSequence();
		ClearEnemy();
		SendOnReturned();
	}

	protected override void OnDisposed()
	{
		ClearEnemy();
		KillActiveSequence();
		base.OnDisposed();
	}
}
