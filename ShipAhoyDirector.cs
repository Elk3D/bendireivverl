using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ShipAhoyDirector : JMonoBehaviour
{
	[Header("Locations")]
	[SerializeField]
	private Transform m_InitialLocation;

	[SerializeField]
	private CharacterNode m_AnimationLocation;

	[SerializeField]
	private CharacterNode m_PunchLocation;

	[Header("Bounds")]
	[SerializeField]
	private BoxCollider[] m_Bounds;

	[Header("Animation Clips")]
	[SerializeField]
	private AnimationClip m_PlayerDeathAnimationClip;

	[SerializeField]
	private AnimationClip m_EnemyPlayerDeathAnimationClip;

	[SerializeField]
	private AnimationClipOverrideGroup[] m_AnimationClipOverrideGroup;

	[Header("Prefabs")]
	[SerializeField]
	private Enemy m_EnemyPrefab;

	private Enemy m_Enemy;

	private HittableWilson m_Wilson;

	private EventTriggerDamage m_AnchorSwingDamage;

	private bool m_IsActive;

	private bool m_IsAttacking;

	private float m_Timer;

	private float m_TimeLimit = 4f;

	private float m_AnchorTimer;

	private float m_AnchorTimeLimit = 9f;

	private int m_WilsonHitPoints;

	private int m_PhaseHits;

	private bool m_IsStuck;

	private float m_StuckTimer;

	private float m_StuckTimerLimit = 5f;

	public event EventHandler OnComplete;

	public void Initialize(Transform startLocation)
	{
		m_Timer = m_TimeLimit;
		m_Enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>(m_EnemyPrefab);
		m_Enemy.transform.position = startLocation.position;
		m_Enemy.transform.eulerAngles = startLocation.eulerAngles;
		m_Enemy.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
		m_Enemy.OnInitializeOnComplete += HandleEnemyOnInitializeOnComplete;
		m_Wilson = m_Enemy.GetComponentInChildren<HittableWilson>();
		m_AnchorSwingDamage = m_Enemy.GetComponentInChildren<EventTriggerDamage>();
		for (int i = 0; i < m_AnimationClipOverrideGroup.Length; i++)
		{
			m_AnimationClipOverrideGroup[i].Initialize();
		}
		GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		GameManager.Instance.Player.OnDeath += HandlePlayerOnDeath;
		GameManager.Instance.Player.SetDeathSequence(active: true);
	}

	private void Update()
	{
		if (!m_IsActive || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (m_IsStuck)
		{
			if (m_StuckTimer >= m_StuckTimerLimit)
			{
				NextPhase();
			}
			else
			{
				m_StuckTimer += Time.deltaTime;
			}
		}
		else
		{
			if (!m_IsAttacking || m_Enemy.CurrentState != State.Character.Follow)
			{
				return;
			}
			if (m_AnchorTimer >= m_AnchorTimeLimit)
			{
				m_IsAttacking = false;
				m_Timer = 0f;
				m_AnchorTimer = 0f;
				UpdateAnimationClips("Flee");
				m_Enemy.SetCanAttack(active: false);
				m_Enemy.OnNodeReached -= HandleEnemyOnNodeReached;
				m_Enemy.OnNodeReached += HandleEnemyOnNodeReached;
				m_Enemy.SetNode(m_AnimationLocation);
				m_Enemy.Movement.SetRunSpeed(8f);
			}
			else if (m_Timer >= m_TimeLimit)
			{
				m_IsAttacking = false;
				m_Timer = 0f;
				UpdateAnimationClips("Flee");
				m_Enemy.SetCanAttack(active: false);
				Vector3 position = m_Enemy.transform.position;
				Vector3 vector = GameManager.Instance.Player.transform.position;
				vector.y = position.y;
				Vector3 normalized = (vector - position).normalized;
				float num = Vector3.Distance(position, vector);
				if (Physics.Raycast(position + Vector3.up * 1f, normalized, out var hitInfo, float.PositiveInfinity, ~(LayerMaskUtility.Player() | LayerMaskUtility.Enemy()), QueryTriggerInteraction.Ignore))
				{
					vector = hitInfo.point + -normalized * 10f;
				}
				bool flag = false;
				for (int i = 0; i < m_Bounds.Length; i++)
				{
					if (m_Bounds[i].bounds.Contains(vector))
					{
						flag = true;
					}
				}
				if (num < 12f || !flag)
				{
					GroundSlam();
					m_Enemy.transform.DOKill();
					m_Enemy.transform.DORotateQuaternion(Quaternion.LookRotation(normalized), 0.9f).SetEase(Ease.OutSine);
					return;
				}
				vector.y = m_Enemy.transform.position.y;
				m_PunchLocation.transform.position = vector;
				m_PunchLocation.transform.rotation = Quaternion.LookRotation((vector - m_Enemy.transform.position).normalized);
				m_Enemy.OnNodeReached -= HandleEnemyPunchOnNodeReached;
				m_Enemy.OnNodeReached += HandleEnemyPunchOnNodeReached;
				m_Enemy.SetNode(m_PunchLocation);
				m_Enemy.Movement.SetRunSpeed(12f);
				m_Enemy.SetDisableVision(active: true);
				m_Enemy.SetState(State.Character.Flee);
			}
			else
			{
				m_Timer += Time.deltaTime;
				m_AnchorTimer += Time.deltaTime;
			}
		}
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		m_Enemy.Content.GenericAnimationEvents.SetReciever(this);
		m_EnemyPlayerDeathAnimationClip.name = "Death";
		m_Enemy.Content.UpdateClipOverride(m_EnemyPlayerDeathAnimationClip);
		m_Enemy.Content.SetAnimationTrigger("Death");
		m_PlayerDeathAnimationClip.name = "Cutscene";
		GameManager.Instance.Player.UpdatePlayerContentClipOverrides(m_PlayerDeathAnimationClip);
		GameManager.Instance.Player.Death();
		CameraEffects.Damage();
		GameManager.Instance.Player.transform.DORotate(m_Enemy.transform.eulerAngles + new Vector3(0f, 180f, 0f), 0.3f).SetEase(Ease.InOutSine);
		GameManager.Instance.Player.transform.DOMove(m_Enemy.transform.position + m_Enemy.transform.forward * 8.5f, 0.3f).SetEase(Ease.InOutSine);
	}

	private void HandleEnemyOnNodeReached(object sender, EventArgs e)
	{
		m_Enemy.OnNodeReached -= HandleEnemyOnNodeReached;
		m_Enemy.SetState(State.Character.Cutscene);
		m_Enemy.ForceStop(smooth: false);
		m_Enemy.Agent.Agent.enabled = false;
		UpdateAnimationClips("AnchorAttack");
		m_Enemy.OnAnimationEnter -= HandleEnemyAnchorOnAnimationEnter;
		m_Enemy.OnAnimationEnter += HandleEnemyAnchorOnAnimationEnter;
		m_Enemy.OnAnimationEvent -= HandleEnemyAnchorOnAnimationEvent;
		m_Enemy.OnAnimationEvent += HandleEnemyAnchorOnAnimationEvent;
		m_Enemy.OnAnimationComplete -= HandleEnemyAnchorAttackOnAnimationComplete;
		m_Enemy.OnAnimationComplete += HandleEnemyAnchorAttackOnAnimationComplete;
		m_Enemy.Content.SetAnimationTrigger("Interact");
	}

	private void HandleEnemyAnchorOnAnimationEnter(object sender, EventArgs e)
	{
		m_Enemy.OnAnimationEnter -= HandleEnemyAnchorOnAnimationEnter;
		m_AnchorSwingDamage.SetActive(active: true);
	}

	private void HandleEnemyAnchorOnAnimationEvent(object sender, EventArgs e)
	{
		m_Enemy.OnAnimationEvent -= HandleEnemyAnchorOnAnimationEvent;
		m_AnchorSwingDamage.SetActive(active: false);
	}

	private void HandleEnemyAnchorAttackOnAnimationComplete(object sender, EventArgs e)
	{
		m_Enemy.OnAnimationComplete -= HandleEnemyAnchorAttackOnAnimationComplete;
		UpdateAnimationClips("AnchorStuck");
		m_Wilson.SetActive(active: true);
		m_IsStuck = true;
	}

	private void HandleEnemyPunchOnNodeReached(object sender, EventArgs e)
	{
		m_Enemy.OnNodeReached -= HandleEnemyPunchOnNodeReached;
		GroundSlam();
	}

	private void GroundSlam()
	{
		m_Enemy.SetState(State.Character.Cutscene);
		m_Enemy.ForceStop(smooth: false);
		m_Enemy.Agent.Agent.enabled = false;
		UpdateAnimationClips("PunchDown");
		m_Enemy.OnAnimationComplete -= HandleEnemyPunchOnAnimationComplete;
		m_Enemy.OnAnimationComplete += HandleEnemyPunchOnAnimationComplete;
		m_Enemy.Content.SetAnimationTrigger("Interact");
		GroundSlam groundSlam = m_Enemy.GetComponentInChildren<GroundSlam>(includeInactive: true);
		if (!(groundSlam != null))
		{
			return;
		}
		Sequence s = DOTween.Sequence();
		s.InsertCallback(0.95f, delegate
		{
			groundSlam.Enable();
			CharacterAction characterAction = m_Enemy.ActionGroupAttack.Actions[0];
			if (characterAction != null)
			{
				characterAction.Action(m_Enemy);
			}
		});
		s.InsertCallback(4.9f, groundSlam.Disable);
	}

	private void HandleEnemyPunchOnAnimationComplete(object sender, EventArgs e)
	{
		m_Enemy.OnAnimationComplete -= HandleEnemyPunchOnAnimationComplete;
		UpdateAnimationClips("Default");
		m_Enemy.SetNode(null);
		m_Enemy.Agent.Agent.enabled = true;
		m_Enemy.SetTarget(GameManager.Instance.Player.transform);
		m_Enemy.SetState(State.Character.Follow);
		m_Enemy.SetDisableVision(active: false);
		m_Enemy.Movement.SetRunSpeed(4f);
		m_Enemy.SetCanAttack(active: true);
		m_IsAttacking = true;
	}

	private void HandleEnemyOnInitializeOnComplete(object sender, EventArgs e)
	{
		m_Enemy.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
		m_Enemy.Agent.Agent.enabled = true;
		m_Enemy.SetTarget(GameManager.Instance.Player.transform);
		m_Enemy.SetState(State.Character.Follow);
		m_Enemy.SetDisableVision(active: false);
		UpdateAnimationClips("Default");
		m_Enemy.SetCanAttack(active: true);
		m_Wilson.OnHit -= HandleWilsonOnHit;
		m_Wilson.OnHit += HandleWilsonOnHit;
		m_IsActive = true;
		m_IsAttacking = true;
	}

	private void HandleWilsonOnHit(object sender, EventArgs e)
	{
		m_WilsonHitPoints++;
		m_PhaseHits++;
		if (m_WilsonHitPoints >= 9)
		{
			KillShipAhoy();
		}
		else if (m_PhaseHits >= 3)
		{
			NextPhase();
		}
		else
		{
			AnchorStuckHit();
		}
	}

	private void NextPhase()
	{
		if (m_IsStuck)
		{
			m_IsStuck = false;
			m_StuckTimer = 0f;
			m_PhaseHits = 0;
			m_Wilson.SetActive(active: false);
			UpdateAnimationClips("AnchorPickUp");
			m_Enemy.OnAnimationComplete -= HandleEnemyAnchorAttackPickUpOnAnimationComplete;
			m_Enemy.OnAnimationComplete += HandleEnemyAnchorAttackPickUpOnAnimationComplete;
			m_Enemy.Content.SetAnimationTrigger("Interact");
		}
	}

	private void AnchorStuckHit()
	{
		if (m_IsStuck)
		{
			UpdateAnimationClips("AnchorStuckHit");
			m_Enemy.Content.SetAnimationTrigger("Interact");
		}
	}

	private void HandleEnemyAnchorAttackPickUpOnAnimationComplete(object sender, EventArgs e)
	{
		m_Enemy.OnAnimationComplete -= HandleEnemyAnchorAttackPickUpOnAnimationComplete;
		UpdateAnimationClips("Default");
		m_Enemy.SetNode(null);
		m_Enemy.Agent.Agent.enabled = true;
		m_Enemy.SetTarget(GameManager.Instance.Player.transform);
		m_Enemy.SetState(State.Character.Follow);
		m_Enemy.Movement.SetRunSpeed(4f);
		m_Enemy.SetCanAttack(active: true);
		m_IsAttacking = true;
	}

	private void KillShipAhoy()
	{
		m_Wilson.OnHit -= HandleWilsonOnHit;
		m_IsActive = false;
		m_IsStuck = false;
		m_Timer = 0f;
		m_AnchorTimer = 0f;
		m_StuckTimer = 0f;
		m_PhaseHits = 0;
		m_Wilson.SetActive(active: false);
		GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		GameManager.Instance.Player.SetDeathSequence(active: false);
		GameManager.Instance.Player.Heal((int)UpgradeCheck.GetHealth());
		GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.SetStatus(AbilityStatus.Inactive);
		GameManager.Instance.Player.LockAbilities();
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.HideTeleport();
		}
		this.OnComplete.Send(this);
	}

	public void Deactivate()
	{
		if (m_Enemy != null)
		{
			m_Wilson.OnHit -= HandleWilsonOnHit;
			m_Enemy.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
			m_Enemy.OnNodeReached -= HandleEnemyOnNodeReached;
			m_Enemy.Dispose();
			m_Enemy = null;
		}
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		}
	}

	public void UpdateAnimationClips(string name)
	{
		List<AnimationClip> list = new List<AnimationClip>();
		for (int i = 0; i < m_AnimationClipOverrideGroup.Length; i++)
		{
			AnimationClipOverrideGroup animationClipOverrideGroup = m_AnimationClipOverrideGroup[i];
			if (animationClipOverrideGroup.Name == name)
			{
				for (int j = 0; j < animationClipOverrideGroup.AnimationClipGroups.Length; j++)
				{
					list.Add(animationClipOverrideGroup.AnimationClipGroups[j].AnimationClip);
				}
				break;
			}
		}
		m_Enemy.Content.UpdateClipOverrides(list.ToArray());
	}

	public void GameOver()
	{
		GameManager.Instance.ShowGameOver(GameOverType.ShipAhoy);
	}

	protected override void OnDisposed()
	{
		this.OnComplete = null;
		if (m_Enemy != null)
		{
			m_Enemy.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
			m_Wilson.OnHit -= HandleWilsonOnHit;
			m_Enemy.OnNodeReached -= HandleEnemyOnNodeReached;
			m_Enemy.OnAnimationEnter -= HandleEnemyAnchorOnAnimationEnter;
			m_Enemy.OnAnimationEvent -= HandleEnemyAnchorOnAnimationEvent;
			m_Enemy = null;
		}
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		}
		base.OnDisposed();
	}
}
