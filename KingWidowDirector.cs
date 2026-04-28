using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class KingWidowDirector : JMonoBehaviour
{
	[Header("Locations")]
	[SerializeField]
	private Transform m_InitialLocation;

	[SerializeField]
	private CharacterNode m_AnimationLocation;

	[SerializeField]
	private Transform m_ShootLocation;

	[Header("Animation Clips")]
	[SerializeField]
	private AnimationClip m_PlayerDeathAnimationClip;

	[SerializeField]
	private AnimationClip m_KingWidowPlayerDeathAnimationClip;

	[SerializeField]
	private AnimationClipOverrideGroup[] m_AnimationClipOverrideGroup;

	[Header("Prefabs")]
	[SerializeField]
	private Enemy m_EnemyPrefab;

	[SerializeField]
	private InkWidowEgg m_ShotPrefab;

	private Enemy m_Enemy;

	private Transform[] m_ShootLocations;

	private bool m_IsActive;

	private bool m_IsAttacking;

	private int m_ShotIndex = 1;

	private int m_EnemyIndex;

	private int m_HitPoints;

	private int m_PhaseHits;

	public event EventHandler OnJumpUp;

	public event EventHandler OnJumpDown;

	public event EventHandler OnSpit;

	public event EventHandler OnPlayerDeath;

	public event EventHandler OnComplete;

	public void Initialize(Transform startLocation)
	{
		m_ShootLocations = m_AnimationLocation.GetComponentsInChildren<Transform>();
		for (int i = 0; i < m_AnimationClipOverrideGroup.Length; i++)
		{
			m_AnimationClipOverrideGroup[i].Initialize();
		}
		m_Enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>(m_EnemyPrefab);
		m_Enemy.transform.position = startLocation.position;
		m_Enemy.transform.eulerAngles = startLocation.eulerAngles;
		m_Enemy.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
		m_Enemy.OnInitializeOnComplete += HandleEnemyOnInitializeOnComplete;
		m_Enemy.Initialize();
		m_Enemy.SetSection(GetComponentInParent<Section>().SectionID);
	}

	private void Update()
	{
		if (m_IsAttacking && m_IsActive && !base.IsDisposed && !GameManager.Instance.IsPaused && GameManager.Instance.Player.CombatStatus == CombatStatus.None)
		{
			m_IsAttacking = true;
			m_Enemy.SetHitAnimation(active: true);
			m_Enemy.SetTarget(GameManager.Instance.Player.transform);
			m_Enemy.SetState(State.Character.Follow);
		}
	}

	private void HandleEnemyOnInitializeOnComplete(object sender, EventArgs e)
	{
		m_Enemy.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
		SetDeathSequence(active: true);
		m_IsActive = true;
		m_IsAttacking = true;
		m_Enemy.SetHitAnimation(active: true);
		m_Enemy.SetTarget(GameManager.Instance.Player.transform);
		m_Enemy.SetState(State.Character.Follow);
		UpdateAnimationClips("Ground");
		m_Enemy.OnDamageTaken -= HandleEnemyOnDamageTaken;
		m_Enemy.OnDamageTaken += HandleEnemyOnDamageTaken;
	}

	private void HandleEnemyOnDamageTaken(object sender, EventArgs e)
	{
		if (m_IsAttacking)
		{
			m_HitPoints++;
			m_PhaseHits++;
			if (m_HitPoints >= GetHitPointMax())
			{
				KillKingWidow();
			}
			else if (m_PhaseHits >= GetPhaseHitMax())
			{
				NextPhase();
			}
		}
	}

	private int GetHitPointMax()
	{
		int result = 9;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Normal:
			result = 12;
			break;
		case DifficultyLevel.Hard:
			result = 15;
			break;
		case DifficultyLevel.Impossible:
			result = 30;
			break;
		}
		return result;
	}

	private int GetPhaseHitMax()
	{
		int result = 3;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Normal:
			result = 4;
			break;
		case DifficultyLevel.Hard:
			result = 5;
			break;
		case DifficultyLevel.Impossible:
			result = 10;
			break;
		}
		return result;
	}

	private void NextPhase()
	{
		m_IsAttacking = false;
		m_Enemy.SetHitAnimation(active: false);
		m_Enemy.OnDamageTaken -= HandleEnemyOnDamageTaken;
		m_Enemy.OnNodeReached -= HandleEnemyOnNodeReached;
		SetDeathSequence(active: false);
		m_Enemy.SetTarget(null);
		m_Enemy.SetNode(null);
		m_Enemy.SetDisableVision(active: true);
		m_Enemy.Movement.SetMoveSpeed(9f);
		m_Enemy.Movement.SetRunSpeed(9f);
		UpdateAnimationClips("Flee");
		m_ShotIndex = 1;
		m_PhaseHits = 0;
		Vector3 eulerAngles = m_InitialLocation.eulerAngles;
		eulerAngles.y += 45 * UnityEngine.Random.Range(1, 9);
		m_AnimationLocation.transform.eulerAngles = eulerAngles;
		m_Enemy.OnNodeReached += HandleEnemyOnNodeReached;
		m_Enemy.SetNode(m_AnimationLocation);
		m_Enemy.SetState(State.Character.Flee);
	}

	private void KillKingWidow()
	{
		m_IsActive = false;
		m_IsAttacking = false;
		m_Enemy.SetHitAnimation(active: false);
		m_PhaseHits = 0;
		m_Enemy.SetState(State.Character.Death);
		GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		GameManager.Instance.Player.SetDeathSequence(active: false);
		m_Enemy.OnDamageTaken -= HandleEnemyOnDamageTaken;
		m_Enemy.SetTarget(null);
		m_Enemy.ForceStop(smooth: false);
		m_Enemy.Agent.Agent.enabled = false;
		m_Enemy.Controller.enabled = false;
		Collider[] componentsInChildren = m_Enemy.Content.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = true;
		}
		EventTriggerDamage componentInChildren = m_Enemy.GetComponentInChildren<EventTriggerDamage>();
		if (componentInChildren != null)
		{
			componentInChildren.Dispose();
		}
		m_Enemy.Content.Animator.SetTrigger("Death");
		m_Enemy.OnAnimationComplete -= HandleEnemyOnDeathComplete;
		m_Enemy.OnAnimationComplete += HandleEnemyOnDeathComplete;
		this.OnComplete.Send(this);
	}

	private void HandleEnemyOnDeathComplete(object sender, EventArgs e)
	{
		m_Enemy.OnAnimationComplete -= HandleEnemyOnDeathComplete;
		LootableEnemy lootableEnemy = new GameObject("LootableEnemy").AddComponent<LootableEnemy>();
		lootableEnemy.Initialize(m_Enemy.Content.gameObject, m_Enemy.SectionID, m_Enemy.EnemyType);
		lootableEnemy.AddData();
		m_Enemy.Dispose();
	}

	private void HandleEnemyOnNodeReached(object sender, EventArgs e)
	{
		m_Enemy.OnNodeReached -= HandleEnemyOnNodeReached;
		m_Enemy.SetState(State.Character.Cutscene);
		m_Enemy.ForceStop(smooth: false);
		UpdateAnimationClips("JumpUp");
		m_Enemy.OnAnimationComplete -= HandleEnemyJumpUpOnAnimationComplete;
		m_Enemy.OnAnimationComplete += HandleEnemyJumpUpOnAnimationComplete;
		m_Enemy.Content.SetAnimationTrigger("Interact");
		m_Enemy.Controller.enabled = false;
		Collider[] componentsInChildren = m_Enemy.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		m_Enemy.Agent.Agent.enabled = false;
		this.OnJumpUp.Send(this);
	}

	private void HandleEnemyJumpUpOnAnimationComplete(object sender, EventArgs e)
	{
		m_Enemy.OnAnimationComplete -= HandleEnemyJumpUpOnAnimationComplete;
		m_EnemyIndex = 8;
		UpdateAnimationClips("SpitEggs");
		m_Enemy.OnAnimationEvent -= HandleEnemyOnAnimationEvent;
		m_Enemy.OnAnimationEvent += HandleEnemyOnAnimationEvent;
		m_Enemy.Content.SetAnimationTrigger("Interact");
	}

	private void HandleEnemyOnAnimationEvent(object sender, EventArgs e)
	{
		this.OnSpit.Send(this);
		Vector3 position = m_ShootLocations[m_ShotIndex].position;
		InkWidowEgg inkWidowEgg = GameManager.Instance.AssetManager.CreateAsset<InkWidowEgg>(m_ShotPrefab);
		inkWidowEgg.transform.position = m_ShootLocation.position;
		inkWidowEgg.transform.localScale = Vector3.zero;
		inkWidowEgg.transform.DOScale(Vector3.one, 0.1f);
		inkWidowEgg.transform.DOMove(position, 0.35f).SetEase(Ease.Linear);
		ShortcutExtensions.DORotate(endValue: new Vector3(720f, 720f, 720f), target: inkWidowEgg.transform, duration: 0.35f, mode: RotateMode.LocalAxisAdd).SetEase(Ease.Linear).OnComplete(delegate
		{
			inkWidowEgg.OnDeath -= HandleInkWidowOnDeath;
			inkWidowEgg.OnDeath += HandleInkWidowOnDeath;
			inkWidowEgg.Initialize();
		});
		m_ShotIndex++;
	}

	private void HandleInkWidowOnDeath(object sender, EventArgs e)
	{
		((InkWidowEgg)sender).OnDeath -= HandleInkWidowOnDeath;
		m_EnemyIndex--;
		if (m_ShotIndex >= m_ShootLocations.Length && m_EnemyIndex <= 0)
		{
			UpdateAnimationClips("JumpDown");
			m_Enemy.OnAnimationComplete -= HandleEnemyJumpDownOnAnimationComplete;
			m_Enemy.OnAnimationComplete += HandleEnemyJumpDownOnAnimationComplete;
			m_Enemy.Content.SetAnimationTrigger("Interact");
			m_Enemy.Controller.enabled = true;
			Collider[] componentsInChildren = m_Enemy.GetComponentsInChildren<Collider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = true;
			}
			m_Enemy.Agent.Agent.enabled = true;
			Vector3 position = GameManager.Instance.Player.transform.position;
			position.y = m_Enemy.transform.position.y;
			Vector3 normalized = (position - m_Enemy.transform.position).normalized;
			m_Enemy.transform.DORotateQuaternion(Quaternion.LookRotation(normalized), 0.75f).SetDelay(0.25f).SetEase(Ease.Linear);
			this.OnJumpDown.Send(this);
		}
	}

	private void HandleEnemyJumpDownOnAnimationComplete(object sender, EventArgs e)
	{
		m_Enemy.OnAnimationComplete -= HandleEnemyJumpDownOnAnimationComplete;
		UpdateAnimationClips("Ground");
		m_Enemy.SetNode(null);
		m_Enemy.SetDisableVision(active: false);
		m_Enemy.SetTarget(GameManager.Instance.Player.transform);
		m_Enemy.SetState(State.Character.Follow);
		SetDeathSequence(active: true);
		m_Enemy.Movement.SetMoveSpeed(3f);
		m_Enemy.Movement.SetRunSpeed(3f);
		m_Enemy.OnDamageTaken -= HandleEnemyOnDamageTaken;
		m_Enemy.OnDamageTaken += HandleEnemyOnDamageTaken;
		m_IsAttacking = true;
		m_Enemy.SetHitAnimation(active: true);
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		GameManager.Instance.Player.ForceAbilitiesCancel();
		m_Enemy.SetDisableVision(active: true);
		m_Enemy.SetState(State.Character.Cutscene);
		m_Enemy.ForceStop();
		m_Enemy.Controller.enabled = false;
		Collider[] componentsInChildren = m_Enemy.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		m_Enemy.Agent.Agent.enabled = false;
		m_KingWidowPlayerDeathAnimationClip.name = "Death";
		m_Enemy.Content.UpdateClipOverride(m_KingWidowPlayerDeathAnimationClip);
		m_Enemy.Content.SetAnimationTrigger("Death");
		m_Enemy.OnAnimationComplete -= HandleEnemyJumpUpOnAnimationComplete;
		m_Enemy.OnAnimationComplete -= HandleEnemyJumpDownOnAnimationComplete;
		m_Enemy.OnAnimationComplete -= HandleEnemyPlayerDeathOnAnimationComplete;
		m_Enemy.OnAnimationComplete += HandleEnemyPlayerDeathOnAnimationComplete;
		m_PlayerDeathAnimationClip.name = "Cutscene";
		GameManager.Instance.Player.UpdatePlayerContentClipOverrides(m_PlayerDeathAnimationClip);
		GameManager.Instance.Player.Death();
		CameraEffects.Damage();
		GameManager.Instance.Player.transform.DORotate(m_Enemy.transform.eulerAngles + new Vector3(0f, 180f, 0f), 0.3f).SetEase(Ease.InOutSine);
		GameManager.Instance.Player.transform.DOMove(m_Enemy.transform.position + m_Enemy.transform.forward * 5f, 0.3f).SetEase(Ease.InOutSine);
		this.OnPlayerDeath.Send(this);
	}

	private void HandleEnemyPlayerDeathOnAnimationComplete(object sender, EventArgs e)
	{
		m_Enemy.OnAnimationComplete -= HandleEnemyPlayerDeathOnAnimationComplete;
		GameManager.Instance.ShowGameOver(GameOverType.KingWidow);
	}

	private void SetDeathSequence(bool active)
	{
		GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		if (active)
		{
			GameManager.Instance.Player.OnDeath += HandlePlayerOnDeath;
		}
		GameManager.Instance.Player.SetDeathSequence(active);
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

	protected override void OnDisposed()
	{
		this.OnJumpUp = null;
		this.OnJumpDown = null;
		this.OnSpit = null;
		this.OnComplete = null;
		this.OnPlayerDeath = null;
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		}
		if (m_Enemy != null)
		{
			m_Enemy.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
			m_Enemy.OnDamageTaken -= HandleEnemyOnDamageTaken;
			m_Enemy.OnNodeReached -= HandleEnemyOnNodeReached;
			m_Enemy.OnAnimationComplete -= HandleEnemyJumpUpOnAnimationComplete;
			m_Enemy.OnAnimationComplete -= HandleEnemyJumpDownOnAnimationComplete;
			m_Enemy.OnAnimationComplete -= HandleEnemyPlayerDeathOnAnimationComplete;
			m_Enemy.OnAnimationComplete -= HandleEnemyOnDeathComplete;
			m_Enemy = null;
		}
		m_ShootLocations = null;
		base.OnDisposed();
	}
}
