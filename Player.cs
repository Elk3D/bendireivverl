using System;
using System.Collections.Generic;
using DG.Tweening;
using InControl;
using S13Audio.BATDR;
using UnityEngine;
using UnityEngine.Rendering;

[DefaultExecutionOrder(-90)]
public class Player : PlayerStateMachine
{
	[Header("Camera Transforms")]
	[SerializeField]
	private Transform m_CameraPivot;

	[SerializeField]
	private Transform m_HeadContainer;

	[SerializeField]
	private Transform m_CameraContainer;

	[SerializeField]
	private Transform m_AnimationContainer;

	[Header("Head Tracking")]
	[SerializeField]
	private Transform m_HeadBobTracker;

	[SerializeField]
	private TransformTracker m_HeadTracker;

	[Header("Arm Tracking")]
	[SerializeField]
	private Transform m_ArmPivot;

	[SerializeField]
	private TransformTracker m_WeaponTracker;

	[SerializeField]
	private TransformTracker m_AbilityTracker;

	[Header("Parent Transforms")]
	[SerializeField]
	private Transform m_WeaponParent;

	[SerializeField]
	private Transform m_BodyWeaponParent;

	[Header("Controllers")]
	[SerializeField]
	private PlayerMovement m_PlayerMovement;

	[SerializeField]
	private PlayerLook m_PlayerLook;

	[SerializeField]
	private PlayerLook m_PlayerArmLook;

	[SerializeField]
	private PlayerInteraction m_PlayerInteraction;

	[SerializeField]
	private CameraFOV m_CameraFOV;

	private GameObject m_InternalTracker;

	private float m_RunTimer;

	private float m_RunCooldown = 5f;

	private float m_ShimmyX;

	private float m_Health;

	private bool m_CanStealth = true;

	private bool m_IsDead;

	private bool m_HasDeathSequence;

	private bool m_CanHeadContainerSlerp = true;

	private bool m_CanCameraPivotLerp = true;

	private bool m_IsRunCooldown;

	private bool m_InternalJump;

	private Sequence m_CrouchSequence;

	private bool m_InternalCrouch;

	private List<Character> m_Enemies = new List<Character>();

	private int m_HandWeaponAttackType;

	private Sequence m_WeaponSequence;

	private Sequence m_AbilitySequence;

	private PlayerWeaponGentPipe m_WeaponState;

	private Tweener m_DeathVibration;

	[Header("S13 Section")]
	[SerializeField]
	private PlayerEvents m_PlayerEvents;

	[SerializeField]
	private LayerMask m_FloorMaterialIgnoreLayers;

	public InkDemon test_inkdemon;

	private bool __InkDemonActive;

	public Transform CameraPivot => m_CameraPivot;

	public Transform HeadContainer => m_HeadContainer;

	public Transform CameraParent => m_CameraContainer;

	public Transform AnimationContainer => m_AnimationContainer;

	public Transform WeaponParent => m_WeaponParent;

	public Transform BodyWeaponParent => m_BodyWeaponParent;

	public PlayerMovement PlayerMovement => m_PlayerMovement;

	public PlayerInteraction Interaction => m_PlayerInteraction;

	public CameraFOV CameraFOV => m_CameraFOV;

	protected override bool UseAwake => false;

	public float Health => m_Health;

	public CameraMovements CameraMovement { get; private set; }

	public GameCamera GameCamera { get; private set; }

	public PlayerContent PlayerContent { get; private set; }

	public PlayerModelLayers ModelLayers { get; private set; }

	public Material HandMaterial { get; private set; }

	public CharacterController CharacterController { get; private set; }

	public CapsuleCollider Collider { get; private set; }

	public PlayerMusicController PlayerMusicController { get; private set; }

	public Animator AnimatorBody => PlayerContent.Animator;

	public Animator AnimatorArms => GameCamera.FirstPersonArmsAnimator;

	public bool IsGrounded => CharacterController.isGrounded;

	public bool IsCrouchSequenceActive
	{
		get
		{
			if (m_CrouchSequence != null)
			{
				return m_CrouchSequence.IsPlaying();
			}
			return false;
		}
	}

	public bool IsCrouched => m_InternalCrouch;

	public bool IsWeaponLocked { get; private set; }

	public bool IsAbilityLocked { get; private set; }

	public CombatStatus CombatStatus { get; private set; }

	public CombatStatus PreviousCombatStatus { get; private set; }

	public BattleStatus BattleStatus { get; private set; }

	public SectionID CurrentSectionID { get; private set; }

	public string CurrentZone { get; private set; }

	public GameObject ActiveWeaponDuplicate { get; private set; }

	public GameObject ActiveWeaponMirrorDuplicate { get; private set; }

	public bool IsAttacking { get; private set; }

	public bool IsUnderWanter => GameManager.Instance.GameCamera.IsUnderWater;

	public BATDRPlayerAudioController.FloorMaterials CurrentFloorType { get; private set; }

	public BATDRPlayerAudioController PlayerAudioController { get; private set; }

	public bool _inkDemonActive => __InkDemonActive;

	public event EventHandler OnAbilityUse;

	public event EventHandler OnAnimationComplete;

	public event EventHandler OnAnimationInteract;

	public event EventHandler OnDeath;

	public event EventHandler OnRespawn;

	public event EventHandler OnAttackStart;

	public event EventHandler OnForceDeath;

	public void SetSectionID(SectionID sectionID)
	{
		CurrentSectionID = sectionID;
	}

	public string SetZone(string zone)
	{
		return CurrentZone = zone;
	}

	protected override void InternalInitializeOnComplete()
	{
		UIManager.SetCursor(active: false);
		m_Health = UpgradeCheck.GetHealth();
		CharacterController = GetComponent<CharacterController>();
		Collider = base.gameObject.AddComponent<CapsuleCollider>();
		UpdateCollider();
		CameraMovement = GetComponentInChildren<CameraMovements>(includeInactive: true);
		GameCamera = GetComponentInChildren<GameCamera>(includeInactive: true);
		if (GameCamera != null)
		{
			GameCamera.Initialize(m_HeadContainer, m_CameraContainer);
			m_CameraFOV.Init(GameCamera.Camera, GameCamera.FirstPersonCamera);
		}
		m_PlayerMovement.Initialize(CharacterController);
		m_PlayerLook.Initialize(base.transform, m_HeadContainer);
		m_PlayerArmLook.Initialize(base.transform, m_ArmPivot);
		m_InternalTracker = new GameObject("PlayerInternalTracker");
		m_InternalTracker.transform.SetParent(base.transform);
		m_InternalTracker.transform.position = m_HeadBobTracker.position;
		HandMaterial = GameManager.Instance.AssetManager.GetAsset<Material>("AudreyInkHand");
		if (HandMaterial != null)
		{
			HandMaterial.SetFloat("_EmitPower", 0f);
		}
		PlayerContent = GetComponentInChildren<PlayerContent>();
		ModelLayers = PlayerContent.GetComponentInChildren<PlayerModelLayers>();
		PlayerAudioController = PlayerContent.GetComponentInChildren<BATDRPlayerAudioController>();
		SetCombatStatus(CombatStatus.None);
		SetState(State.Player.Default);
		DisableAllTrackers();
		ModelLayers.EnableFirstPerson();
		PlayerMusicController = GetComponentInChildren<PlayerMusicController>();
		if (PlayerMusicController != null)
		{
			PlayerMusicController.Initialize();
		}
		SetHeadTracker(active: true);
		ShowFirstPersonArms();
	}

	public void UpdatePlayerContentClipOverrides(params AnimationClip[] clips)
	{
		PlayerContent.UpdateClipOverrides(clips);
	}

	private void UpdateCollider(float height)
	{
		CharacterController.height = height;
		CharacterController.center = new Vector3(0f, height / 2f, 0f);
		UpdateCollider();
	}

	private void UpdateCollider()
	{
		Collider.center = CharacterController.center;
		Collider.radius = CharacterController.radius;
		Collider.height = CharacterController.height;
	}

	public void ResetRunTimer()
	{
		m_RunTimer = 0f;
		m_PlayerMovement.UnlockRun();
		m_IsRunCooldown = false;
		GameManager.Instance.RefillSprintBar(isFilling: false);
	}

	protected override void InternalUpdate()
	{
		CheckEnemies();
		if (base.Disabled)
		{
			m_CameraFOV.UpdateVOD(m_PlayerMovement.IsRunning);
			return;
		}
		float stamina = UpgradeCheck.GetStamina();
		if (m_PlayerMovement.CanRun)
		{
			if (!m_PlayerMovement.IsRunLocked && m_PlayerMovement.CanJump && CharacterController.isGrounded && PlayerInput.Jump() && base.CurrentState == State.Player.Default)
			{
				m_RunTimer += stamina * 0.15f;
			}
			if (m_PlayerMovement.IsRunning && !m_PlayerMovement.IsRunLocked)
			{
				m_RunTimer += Time.deltaTime;
			}
			else if (!m_PlayerMovement.IsRunning && m_PlayerMovement.IsRunLocked)
			{
				m_RunTimer -= Time.deltaTime;
				if (m_RunTimer <= 0f)
				{
					ResetRunTimer();
				}
			}
			else if (!m_PlayerMovement.IsRunning && !m_PlayerMovement.IsRunLocked && m_RunTimer > 0f)
			{
				m_RunTimer -= Time.deltaTime * 2f;
				if (m_RunTimer < 0f)
				{
					m_RunTimer = 0f;
				}
			}
			if (m_RunTimer > stamina)
			{
				m_PlayerMovement.StopRun();
				m_PlayerMovement.LockRun();
				m_RunTimer = m_RunCooldown;
				m_IsRunCooldown = true;
				GameManager.Instance.RefillSprintBar(isFilling: true);
			}
			GameManager.Instance.ShowSprintBar(m_RunTimer, m_IsRunCooldown ? m_RunCooldown : stamina);
		}
		m_CameraFOV.UpdateVOD(m_PlayerMovement.IsRunning);
		CheckFloorMaterial();
	}

	protected override void InternalFixedUpdate()
	{
		m_PlayerLook.UpdateCursorLock();
		GetHeadTracker();
	}

	protected override void InternalLateUpdate()
	{
	}

	public void UpdateLookInput()
	{
		m_PlayerLook.GetInput();
		m_PlayerArmLook.GetInput();
	}

	public void UpdateMovementInput()
	{
		m_PlayerMovement.UpdateMovementInput();
		m_ShimmyX = PlayerInput.LookX();
		float num = (m_PlayerMovement.IsCrouched ? 1f : 2f);
		m_ShimmyX = Mathf.Clamp(m_ShimmyX, -1f, 1f) * num;
	}

	public void UpdateInteractionInput()
	{
		m_PlayerInteraction.Update(m_CameraContainer);
	}

	public void UpdateAbilitiesInput()
	{
		if (base.Abilities != null)
		{
			for (int i = 0; i < base.Abilities.Count; i++)
			{
				base.Abilities[i]?.Update();
			}
		}
	}

	public void UpdateWeaponInput()
	{
		base.CurrentWeapon?.Update();
	}

	public void UpdateMovement()
	{
		m_PlayerMovement.UpdateMovement(base.transform);
	}

	public void UpdateRotations()
	{
		m_PlayerLook.Rotation(base.transform, m_HeadContainer);
		m_PlayerArmLook.Rotation(base.transform, m_ArmPivot);
	}

	public void UpdateAbilities()
	{
		if (base.Abilities != null)
		{
			for (int i = 0; i < base.Abilities.Count; i++)
			{
				base.Abilities[i]?.FixedUpdate();
			}
		}
	}

	public void UpdateWeapon()
	{
		base.CurrentWeapon?.FixedUpdate();
	}

	public void UpdateAnimationRotations()
	{
		m_PlayerLook.Rotation(m_AnimationContainer, m_HeadContainer);
	}

	public void UpdateAnimations()
	{
		InternalUpdateAnimations();
	}

	public void UpdateAbilitiesLateUpdate()
	{
		if (base.Abilities != null)
		{
			for (int i = 0; i < base.Abilities.Count; i++)
			{
				base.Abilities[i]?.LateUpdate();
			}
		}
	}

	public void UpdateWeaponLateUpdate()
	{
		base.CurrentWeapon?.LateUpdate();
	}

	private void InternalUpdateAnimations()
	{
		float num = (m_PlayerMovement.IsRunning ? (m_PlayerMovement.MovementInput.x * 2f) : m_PlayerMovement.MovementInput.x);
		float num2 = (m_PlayerMovement.IsRunning ? (m_PlayerMovement.MovementInput.y * 2f) : m_PlayerMovement.MovementInput.y);
		if ((double)Vector3.Distance(m_PlayerMovement.CurrentPosition, m_PlayerMovement.PreviousPosition) <= 0.01)
		{
			if (num2 != 0f)
			{
				num2 = 0f;
			}
			if (num != 0f)
			{
				num = 0f;
			}
		}
		if (m_PlayerMovement.IsSlowed)
		{
			if (num != 0f)
			{
				num = m_PlayerMovement.MovementInput.x;
			}
			if (num2 != 0f)
			{
				num2 = m_PlayerMovement.MovementInput.y;
			}
			float num3 = 0.25f;
			if (num > 0f)
			{
				num -= num3;
			}
			else if (num < 0f)
			{
				num += num3;
			}
			if (num2 > 0f)
			{
				num2 -= num3;
			}
			else if (num2 < 0f)
			{
				num2 += num3;
			}
		}
		if (!IsGrounded && !m_PlayerMovement.PreviouslyGrounded && !m_InternalJump)
		{
			Vector3 end = base.transform.position + Vector3.down * 0.3f;
			if (!Physics.Linecast(base.transform.position, end, out var _, ~(int)m_FloorMaterialIgnoreLayers, QueryTriggerInteraction.Ignore))
			{
				m_InternalJump = true;
			}
		}
		else if (IsGrounded && m_PlayerMovement.PreviouslyGrounded && m_InternalJump)
		{
			m_InternalJump = false;
			bool flag = false;
			if (CharacterController.transform.position.y < m_PlayerMovement.InitialAirPosition.y)
			{
				Vector3 initialAirPosition = m_PlayerMovement.InitialAirPosition;
				initialAirPosition.x = CharacterController.transform.position.x;
				initialAirPosition.z = CharacterController.transform.position.z;
				m_PlayerMovement.InitialAirPosition = initialAirPosition;
				if (Vector3.Distance(m_PlayerMovement.InitialAirPosition, CharacterController.transform.position) > 15f)
				{
					int damage = 2;
					if (GameManager.Instance.Player.Health <= 2f)
					{
						damage = 0;
					}
					Damage(damage);
					ShowHealthBar();
					CameraEffects.Damage();
					CameraEffects.ShakeRotation(0.5f, 2f, 10, 90f, fadeOut: false);
					S13SetPlayerReaction(BATDRPlayerAudioController.PlayerReaction.Hit);
					flag = true;
				}
			}
			if (!flag)
			{
				CameraEffects.ShakeRotation(0.2f, 0.8f, 2, 90f, fadeOut: true, vibrate: false);
				S13SetPlayerReaction(BATDRPlayerAudioController.PlayerReaction.Land);
			}
		}
		if (IsGrounded && m_PlayerMovement.IsCrouched && !m_InternalCrouch)
		{
			m_InternalCrouch = true;
			SetCanStealth(active: true);
			if (m_CanStealth && CombatStatus != CombatStatus.Combat && CombatStatus != CombatStatus.Hide)
			{
				GameManager.Instance.ShowStealth();
				SetCombatStatus(CombatStatus.Stealth);
			}
			float characterControllerHeight = CharacterController.height;
			m_CrouchSequence?.Kill();
			m_CrouchSequence = DOTween.Sequence();
			m_CrouchSequence.Insert(0f, DOTween.To(() => characterControllerHeight, delegate(float value)
			{
				characterControllerHeight = value;
			}, 4.5f, 0.5f).SetUpdate(UpdateType.Fixed).SetEase(Ease.InOutSine)
				.OnUpdate(delegate
				{
					UpdateCollider(characterControllerHeight);
				}));
			m_CrouchSequence.OnComplete(delegate
			{
				UpdateCollider();
				m_CrouchSequence = null;
			});
			S13SetCrouching(m_InternalCrouch);
		}
		else if (IsGrounded && !m_PlayerMovement.IsCrouched && m_InternalCrouch)
		{
			if (!Physics.SphereCast(base.transform.position, CharacterController.radius, Vector3.up, out var _, 6.5f - CharacterController.radius, ~(1 << LayerMask.NameToLayer("Player")), QueryTriggerInteraction.Ignore))
			{
				m_InternalCrouch = false;
				if (m_CanStealth && CombatStatus != CombatStatus.Combat && CombatStatus != CombatStatus.Hide)
				{
					GameManager.Instance.HideStealth();
					SetCombatStatus(CombatStatus.None);
				}
				float characterControllerHeight2 = CharacterController.height;
				m_CrouchSequence?.Kill();
				m_CrouchSequence = DOTween.Sequence();
				m_CrouchSequence.Insert(0f, DOTween.To(() => characterControllerHeight2, delegate(float value)
				{
					characterControllerHeight2 = value;
				}, 6.5f, 0.25f).SetUpdate(UpdateType.Fixed).SetEase(Ease.InOutSine)
					.OnUpdate(delegate
					{
						UpdateCollider(characterControllerHeight2);
					}));
				m_CrouchSequence.OnComplete(delegate
				{
					UpdateCollider();
					m_CrouchSequence = null;
				});
				S13SetCrouching(m_InternalCrouch);
			}
			else
			{
				m_PlayerMovement.SetCrouchInput(isCrouched: true);
			}
		}
		if (num2 > 1f)
		{
			SetAnimationMovementState(2f);
		}
		else if (num2 > 0f)
		{
			SetAnimationMovementState(1f);
		}
		else if (num2 < 0f)
		{
			SetAnimationMovementState(-1f);
		}
		else
		{
			SetAnimationMovementState(0f);
		}
		if (IsGrounded)
		{
			if (!m_PlayerMovement.IsCrouched)
			{
				SetAnimationCrouchState(0f);
			}
			else
			{
				SetAnimationCrouchState(1f);
			}
		}
		else
		{
			SetAnimationCrouchState(-1f);
		}
		if (num == 0f && num2 == 0f)
		{
			SetAnimationShimmySpeed(m_ShimmyX);
		}
		else
		{
			SetAnimationShimmySpeed(0f);
		}
		SetAnimationStrafeSpeed(num);
		SetAnimationSpeed(num2);
		SetAnimationCrouchSpeed(num2);
	}

	public void CheckEnemies()
	{
		if (CombatStatus != CombatStatus.Combat)
		{
			return;
		}
		if (m_Enemies.Count > 0)
		{
			for (int num = m_Enemies.Count - 1; num >= 0; num--)
			{
				Character character = m_Enemies[num];
				if (character != null)
				{
					if (character.Target != base.transform || character.CurrentState == State.Character.Patrol)
					{
						RemoveEnemy(character);
					}
				}
				else
				{
					m_Enemies.RemoveAt(num);
				}
			}
		}
		else if (BattleStatus == BattleStatus.None)
		{
			if (IsCrouched)
			{
				SetCombatStatus(CombatStatus.Stealth);
			}
			else
			{
				SetCombatStatus(CombatStatus.None);
			}
		}
	}

	public void AddEnemy(Character character)
	{
		if (!(character == null) && !m_Enemies.Contains(character))
		{
			m_Enemies.Add(character);
			SetCombatStatus(CombatStatus.Combat);
		}
	}

	public void RemoveEnemy(Character character)
	{
		for (int num = m_Enemies.Count - 1; num >= 0; num--)
		{
			if (m_Enemies[num] == null)
			{
				m_Enemies.RemoveAt(num);
			}
		}
		if (m_Enemies.Contains(character))
		{
			m_Enemies.Remove(character);
		}
		if (m_Enemies.Count <= 0)
		{
			ClearEnemies();
		}
	}

	public void ClearEnemies()
	{
		m_Enemies.Clear();
		if (CombatStatus != CombatStatus.Hide && CombatStatus != CombatStatus.Stealth)
		{
			SetCombatStatus(CombatStatus.None);
		}
	}

	public void ForceEnemiesFlee()
	{
		if (m_Enemies.Count <= 0)
		{
			return;
		}
		for (int num = m_Enemies.Count - 1; num >= 0; num--)
		{
			Character character = m_Enemies[num];
			if (character != null)
			{
				if (character.CurrentNode != null)
				{
					character.SetNode(character.CurrentNode);
				}
				else if (character.PreviousNode != null)
				{
					character.SetNode(character.PreviousNode);
				}
				character.SetState(State.Character.Flee);
			}
			else
			{
				m_Enemies.RemoveAt(num);
			}
		}
	}

	public void Hit(RaycastHit hit, Character character = null, int damage = 1)
	{
		Damage(damage);
		if (m_Health <= 0f)
		{
			if (m_HasDeathSequence)
			{
				if (!m_IsDead)
				{
					m_IsDead = true;
				}
				this.OnDeath.Send(this);
			}
			else
			{
				ForceRespawn();
			}
			return;
		}
		ShowHealthBar();
		CameraEffects.Damage();
		CameraEffects.ShakeRotation(0.35f, 2f, 10, 90f, fadeOut: false);
		if (damage > 0)
		{
			Vector3 point = hit.point;
			point.y = base.transform.position.y;
			AddForce((base.transform.position - point).normalized * (2f * (float)damage));
		}
		S13SetPlayerReaction(BATDRPlayerAudioController.PlayerReaction.Hit);
	}

	public void SetHealth(int health, bool isSilent = false)
	{
		m_Health = health;
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.SetHealth((int)m_Health);
		GameManager.Instance.ShowHealthBar(m_Health, UpgradeCheck.GetHealth());
		if (!isSilent)
		{
			BATDRPlayerHealthAudioController.SetPlayerHealth(m_Health);
		}
	}

	public void Damage(int damage)
	{
		if (damage > 0)
		{
			m_Health -= damage;
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.SetHealth((int)m_Health);
			BATDRPlayerHealthAudioController.SetPlayerHealth(m_Health);
		}
	}

	public void Heal(int amount)
	{
		float health = UpgradeCheck.GetHealth();
		if (!(m_Health >= health))
		{
			float num = m_Health + (float)amount;
			if (num > health)
			{
				num = health;
			}
			m_Health = num;
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.SetHealth((int)m_Health);
			GameManager.Instance.ShowHealthBar(m_Health, health);
			BATDRPlayerHealthAudioController.SetPlayerHealth(m_Health);
		}
	}

	public void ShowHealthBar()
	{
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.SetHealth((int)m_Health);
		GameManager.Instance.ShowHealthBar(m_Health, UpgradeCheck.GetHealth());
	}

	public void Death()
	{
		GameCamera.SetFirstPersonArmsActive(active: false);
		ModelLayers.EnableCutscene();
		EnterInternalAnimation();
		DisableAllTrackers();
		SetState(State.Player.Cutscene);
		SetAnimationTrigger("Cutscene");
		GameManager.Instance.HideCrosshair();
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == AbilityStatus.Active && GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.HideTeleport();
		}
		GameManager.Instance.HideHealthBar();
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpdateLife();
	}

	public void ForceRespawn(float blockerDuration = 0f)
	{
		m_DeathVibration.Kill();
		if (GameManager.Instance.HasController)
		{
			float vibration = 2.5f;
			m_DeathVibration = DOTween.To(() => vibration, delegate(float x)
			{
				vibration = x;
			}, 0f, 1.5f).OnUpdate(delegate
			{
				if (GameManager.Instance.HasController)
				{
					InputManager.ActiveDevice.Vibrate(vibration * 0.5f);
				}
			}).OnComplete(delegate
			{
				InputManager.ActiveDevice.Vibrate(0f);
			});
		}
		GameManager.Instance.ShowScreenBlocker(blockerDuration);
		ForceDeath();
	}

	public void ForceDeath()
	{
		if (BattleStatus == BattleStatus.None)
		{
			SetBattleStatus(BattleStatus.Dead);
		}
		S13SetPlayerReaction(BATDRPlayerAudioController.PlayerReaction.Dead);
		this.OnForceDeath.Send(this);
		AnimatorBody.SetTrigger("Cancel");
		AnimatorArms.SetTrigger("Cancel");
		GameManager.Instance.GameCamera.SetUnderWater(active: false);
		ForceAbilitiesCancel();
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpdateLife();
		this.OnDeath.Send(this);
		SetRespawnState(GameManager.Instance.RespawnManager.GetClosest(base.transform.position));
	}

	private void SetRespawnState(PlayerRespawn playerRespawn)
	{
		if (!(playerRespawn == null))
		{
			playerRespawn.Respawn();
			float health = UpgradeCheck.GetHealth();
			m_Health = health / 2f;
			BATDRPlayerHealthAudioController.SetPlayerHealth(m_Health);
			m_IsDead = false;
		}
	}

	public void Respawn()
	{
		GameManager.Instance.ShowHealthBar(m_Health, UpgradeCheck.GetHealth());
		if (base.Abilities != null)
		{
			foreach (PlayerAbilityState ability in base.Abilities)
			{
				if (ability != null && ability is PlayerAbilityStateFlow)
				{
					ability.ForceCooldown();
					break;
				}
			}
		}
		this.OnRespawn.Send(this);
		if (BattleStatus == BattleStatus.Dead)
		{
			SetBattleStatus(BattleStatus.None);
		}
		S13SetPlayerReaction(BATDRPlayerAudioController.PlayerReaction.Respawn);
	}

	public void OnAttack()
	{
		IsAttacking = false;
		base.CurrentWeapon?.OnAttack();
	}

	public void Attack()
	{
		IsAttacking = true;
		int num = UnityEngine.Random.Range(1, 4);
		while (m_HandWeaponAttackType == num)
		{
			num = UnityEngine.Random.Range(1, 4);
		}
		m_HandWeaponAttackType = num;
		AnimatorBody.SetInteger("HandWeaponAttackType", m_HandWeaponAttackType);
		if (AnimatorArms != null)
		{
			AnimatorArms.SetInteger("HandWeaponAttackType", m_HandWeaponAttackType);
		}
		AnimatorBody.SetTrigger("HandWeaponAttack");
		if (AnimatorArms != null)
		{
			AnimatorArms.SetTrigger("HandWeaponAttack");
		}
		this.OnAttackStart.Send(this);
	}

	public void OnAttackComplete()
	{
		IsAttacking = false;
		base.CurrentWeapon?.OnAttackComplete();
	}

	public void AttackComplete()
	{
		IsAttacking = false;
		AnimatorBody.SetInteger("HandWeaponAttackType", 0);
		if (AnimatorArms != null)
		{
			AnimatorArms.SetInteger("HandWeaponAttackType", 0);
		}
		AnimatorBody.ResetTrigger("HandWeaponAttack");
		if (AnimatorArms != null)
		{
			AnimatorArms.ResetTrigger("HandWeaponAttack");
		}
	}

	public void Ability()
	{
		AnimatorBody.SetTrigger("AbilityUse");
		if (AnimatorArms != null)
		{
			AnimatorArms.SetTrigger("AbilityUse");
		}
		this.OnAbilityUse.Send(this);
	}

	public void ForceAbilitiesCancel()
	{
		if (base.Abilities != null)
		{
			for (int i = 0; i < base.Abilities.Count; i++)
			{
				base.Abilities[i].ForceCancel();
			}
		}
	}

	public void ForceAbilitiesCooldown()
	{
		if (base.Abilities != null)
		{
			for (int i = 0; i < base.Abilities.Count; i++)
			{
				base.Abilities[i].ForceCooldown();
			}
		}
	}

	public void LockWeapon()
	{
		IsWeaponLocked = true;
	}

	public void UnlockWeapon()
	{
		IsWeaponLocked = false;
	}

	public void LockAbilities(bool playAudio = false)
	{
		if (playAudio && m_PlayerEvents != null)
		{
			m_PlayerEvents.AbilitiesLose();
		}
		IsAbilityLocked = true;
	}

	public void UnlockAbilities(bool playAudio = false)
	{
		if (playAudio && m_PlayerEvents != null)
		{
			m_PlayerEvents.AbilitiesReturn();
		}
		IsAbilityLocked = false;
	}

	public void SendOnUseAbility()
	{
		this.OnAbilityUse.Send(this);
	}

	public void SetInitialLocation(Transform _transform)
	{
		base.transform.position = _transform.position;
		base.transform.eulerAngles = _transform.eulerAngles;
		ForcePlayerRotation(_transform.rotation);
		ResetRotation();
	}

	public void SetInitialLocation(PlayerTransform playerTransform)
	{
		base.transform.position = playerTransform.Position;
		base.transform.eulerAngles = playerTransform.Rotation;
		ResetRotation();
		ForceRotation(Quaternion.Euler(playerTransform.Rotation), Quaternion.Euler(playerTransform.HeadRotation));
	}

	public void ResetRotation()
	{
		m_PlayerLook.Initialize(base.transform, m_HeadContainer);
		m_PlayerArmLook.Initialize(base.transform, m_ArmPivot);
	}

	public void ForceRotation(Quaternion playerRotation, Quaternion cameraRotation)
	{
		ForcePlayerRotation(playerRotation);
		ForceCameraRotation(cameraRotation);
	}

	public void ForcePlayerRotation(Quaternion playerRotation)
	{
		m_PlayerLook.ForceRotation(playerRotation);
	}

	public void ForceCameraRotation(Quaternion cameraRotation)
	{
		m_PlayerLook.ForceCameraRotation(cameraRotation);
	}

	public void LockRotation(float x, float y)
	{
		m_PlayerLook.HorizontalClampSetActive(active: true);
		m_PlayerLook.SetHorizontalClamp(x);
		m_PlayerLook.SetVerticalClamp(y);
	}

	public void UnlockRotation()
	{
		m_PlayerLook.HorizontalClampSetActive(active: false);
		m_PlayerLook.ResetVerticalClamp();
	}

	public void AddForce(Vector3 force)
	{
		m_PlayerMovement.AddForce(force);
	}

	public void CancelMovement()
	{
		m_PlayerMovement.CancelMovement();
	}

	public void SetCanStealth(bool active)
	{
		m_CanStealth = active;
	}

	public void SetInteraction(bool active)
	{
		m_PlayerInteraction.SetActive(active);
	}

	public void SetCollision(bool active)
	{
		CharacterController.enabled = active;
		Collider.enabled = active;
	}

	public void ResetAnimation()
	{
		SetAnimationMovementState(0f);
		SetAnimationSpeed(0f);
		SetAnimationStrafeSpeed(0f);
		SetAnimationCrouchSpeed(0f);
		m_ShimmyX = 0f;
	}

	public void ForceResetAnimation()
	{
		AnimatorBody.SetMovementState(0f, smooth: false);
		AnimatorBody.SetMovementSpeed(0f, smooth: false);
		AnimatorBody.SetStrafeSpeed(0f, smooth: false);
		AnimatorBody.SetCrouchSpeed(0f, smooth: false);
		m_ShimmyX = 0f;
	}

	public void SetAnimationInt(string trigger, int value)
	{
		if (!(trigger == ""))
		{
			AnimatorBody.SetInteger(trigger, value);
		}
	}

	public void SetAnimationTrigger(string trigger)
	{
		SetBodyAnimationTrigger(trigger);
		SetArmAnimationTrigger(trigger);
	}

	public void ResetAnimationTrigger(string trigger)
	{
		if (!(trigger == ""))
		{
			AnimatorBody.ResetTrigger(trigger);
			if (AnimatorArms != null)
			{
				AnimatorArms.ResetTrigger(trigger);
			}
		}
	}

	public void SetBodyAnimationTrigger(string trigger)
	{
		if (!(trigger == ""))
		{
			AnimatorBody.SetTrigger(trigger);
		}
	}

	public void SetArmAnimationTrigger(string trigger)
	{
		if (!(trigger == "") && AnimatorArms != null)
		{
			AnimatorArms.SetTrigger(trigger);
		}
	}

	public void SetAnimationType(string name, int value)
	{
		AnimatorBody.SetInteger(name, value);
	}

	private void SetAnimationMovementState(float state)
	{
		AnimatorBody.SetMovementState(state);
	}

	private void SetAnimationCrouchState(float state)
	{
		AnimatorBody.SetCrouchState(state, smooth: true, 0.3f);
	}

	private void SetAnimationShimmySpeed(float speed, bool isSmooth = true)
	{
		AnimatorBody.SetShimmySpeed(speed, isSmooth);
	}

	private void SetAnimationSpeed(float speed)
	{
		bool smooth = speed == 0f;
		AnimatorBody.SetMovementSpeed(speed, smooth);
	}

	private void SetAnimationStrafeSpeed(float speed)
	{
		bool smooth = speed == 0f;
		AnimatorBody.SetStrafeSpeed(speed, smooth);
	}

	private void SetAnimationCrouchSpeed(float speed)
	{
		bool smooth = speed == 0f;
		AnimatorBody.SetCrouchSpeed(speed, smooth);
	}

	private void SetAnimationLayerWeight(int layer, float weight)
	{
		if (AnimatorBody != null)
		{
			AnimatorBody.SetLayerWeight(layer, weight);
		}
		if (AnimatorArms != null)
		{
			AnimatorArms.SetLayerWeight(layer, weight);
		}
	}

	private void UpdateClipOverrides(AnimationClip clip = null)
	{
		if (clip != null)
		{
			clip.name = "Cutscene";
			PlayerContent.UpdateClipOverrides(clip);
		}
	}

	public void SetArmTrackersActive(bool active)
	{
		m_AbilityTracker.SetActive(active: false);
		if (base.CurrentWeapon == null)
		{
			m_WeaponTracker.SetActive(active: false);
		}
		else
		{
			m_WeaponTracker.SetActive(active);
		}
	}

	public void ForceStand(bool isOutOfCombat = true)
	{
		m_CrouchSequence.Kill();
		AnimatorBody.SetCrouchState(0f, smooth: false);
		SetAnimationShimmySpeed(0f, isSmooth: false);
		m_InternalCrouch = false;
		m_PlayerMovement.ForceStand();
		UpdateCollider(6.5f);
		if (isOutOfCombat)
		{
			SetCombatStatus(CombatStatus.None);
		}
		S13SetCrouching(m_InternalCrouch);
	}

	public void ForceCrouch(bool isInternalCrouch = true)
	{
		m_CrouchSequence.Kill();
		AnimatorBody.SetCrouchState(1f, smooth: false);
		SetAnimationShimmySpeed(0f, isSmooth: false);
		m_InternalCrouch = isInternalCrouch;
		if (isInternalCrouch)
		{
			SetCanStealth(active: true);
			m_PlayerMovement.ForceCrouch();
			UpdateCollider(4.5f);
			SetCombatStatus(CombatStatus.Stealth);
		}
		S13SetCrouching(m_InternalCrouch);
	}

	public void EnableAnimationRotation(float x = 15f, float y = 20f, bool isCutscene = false)
	{
		State.Player state = (isCutscene ? State.Player.CutscenePeek : State.Player.Peek);
		SetState(state);
		m_PlayerLook.Initialize(m_AnimationContainer, m_HeadContainer);
		LockRotation(x, y);
	}

	public void EnterInteraction(string trigger)
	{
		JDebug.Log("Player :: EnterInteraction :: (trigger = " + trigger + ")", this, JDebug.JDebugType.Player);
		SetState(State.Player.Cutscene);
		CancelMovement();
		ResetAnimation();
		ModelLayers.EnableCutscene();
		SetHeadTracker(active: false);
		HideFirstPersonArms();
		SetBodyAnimationTrigger(trigger);
		DOTween.Sequence().InsertCallback(0.05f, EnterInternalAnimation);
	}

	private void EnterInternalAnimation()
	{
		m_HeadContainer.SetParent(m_AnimationContainer);
	}

	public void EnterInteractionInstant(string trigger)
	{
		JDebug.Log("Player :: EnterInteractionInstant :: (trigger = " + trigger + ")", this, JDebug.JDebugType.Player);
		SetState(State.Player.Cutscene);
		CancelMovement();
		ForceResetAnimation();
		ResetAnimation();
		ModelLayers.EnableCutscene();
		SetHeadTracker(active: false);
		if (trigger.ToLower().Contains("instant"))
		{
			SetAnimationLayerWeight(1, 0f);
			SetAnimationLayerWeight(2, 0f);
		}
		HideFirstPersonArms();
		SetBodyAnimationTrigger(trigger);
		m_HeadContainer.SetParent(m_AnimationContainer);
		m_HeadContainer.localPosition = Vector3.zero;
		m_HeadContainer.localEulerAngles = Vector3.zero;
	}

	public void ExitInteraction()
	{
		JDebug.Log("Player :: ExitInteraction", this, JDebug.JDebugType.Player);
		SetState(State.Player.Cutscene);
		SetAnimationTrigger("ExitInteraction");
		if (base.gameObject.scene.buildIndex != -1 && base.gameObject.transform.parent == null)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		UnlockRotation();
	}

	public void ExitAnimation()
	{
		AnimationComplete();
		ResetRotation();
		DOTween.Sequence().InsertCallback(0.05f, ExitInternalAnimation);
	}

	private void ExitInternalAnimation()
	{
		EnableAllTrackers();
		ShowFirstPersonArms();
		ExitAnimationCamera();
		ResetRotation();
		SetState(State.Player.Default);
	}

	public void ExitAnimationCamera()
	{
		m_HeadContainer.SetParent(m_CameraPivot);
		m_AnimationContainer.localPosition = Vector3.zero;
		m_AnimationContainer.localEulerAngles = Vector3.zero;
	}

	public void EnterCutscene(AnimationClip animationClip)
	{
		animationClip.name = "Cutscene";
		UpdateClipOverrides(animationClip);
		EnterInteraction("Cutscene");
	}

	public void AnimationInteract()
	{
		this.OnAnimationInteract.Send(this);
	}

	public void AnimationComplete()
	{
		this.OnAnimationComplete.Send(this);
	}

	public void AnimationClear()
	{
		m_AnimationContainer.DOLocalMove(m_HeadBobTracker.localPosition, 0.25f).SetEase(Ease.InOutSine);
	}

	public void SlideToLocation(Transform location, float duration = 0.25f, Ease ease = Ease.InOutSine)
	{
		SlideToLocation(location.position, location.eulerAngles, duration, ease);
	}

	public void SlideToLocation(Vector3 position, Vector3 rotation, float duration = 0.25f, Ease ease = Ease.InOutSine)
	{
		base.transform.DOMove(position, duration).SetEase(ease);
		base.transform.DORotate(rotation, duration).SetEase(ease).OnComplete(delegate
		{
			GameCamera.SetFirstPersonArmsActive(active: false);
		});
	}

	private void EnableAllTrackers()
	{
		SetAllTrackers(active: true);
	}

	private void DisableAllTrackers()
	{
		SetAllTrackers(active: false);
	}

	public void SetHeadTracker(bool active)
	{
		m_HeadTracker.SetActive(active);
	}

	public void SetAllTrackers(bool active)
	{
		m_HeadTracker.SetActive(active);
		SetArmTrackersActive(active);
		SetAnimationLayerWeight(1, 0f);
		if (m_WeaponState != null && GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.WeaponStatus == WeaponStatus.Active)
		{
			SetAnimationLayerWeight(2, 1f);
		}
		else
		{
			SetAnimationLayerWeight(2, 0f);
		}
	}

	public void SetCombatStatus(CombatStatus combatStatus)
	{
		if (CombatStatus != combatStatus)
		{
			PreviousCombatStatus = CombatStatus;
			CombatStatus = combatStatus;
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.SetCombatStatus(CombatStatus);
			JDebug.Log("[Player] - SetCombatStatus ::  [CombatStatus: " + CombatStatus.ToString() + "] | [PreviousCombatStatus: " + PreviousCombatStatus.ToString() + "]", this, JDebug.JDebugType.Player);
		}
	}

	public void SetBattleStatus(BattleStatus battleStatus)
	{
		BattleStatus = battleStatus;
		JDebug.Log("[Player] - SetBattleStatus ::  [BattleStatus: " + BattleStatus.ToString() + "]", this, JDebug.JDebugType.Player);
	}

	public void SetDeathSequence(bool active)
	{
		m_HasDeathSequence = active;
	}

	public void SetCameraPivotLerp(bool active)
	{
		m_CanCameraPivotLerp = active;
	}

	public void SetHeadContainerSlerp(bool active)
	{
		m_CanHeadContainerSlerp = active;
	}

	private void GetHeadTracker()
	{
		m_InternalTracker.transform.position = m_HeadBobTracker.position;
		if (m_CanCameraPivotLerp)
		{
			if (GameManager.Instance.PlayerSettings.SmoothCamera)
			{
				Vector3 localPosition = m_InternalTracker.transform.localPosition;
				Vector3 localPosition2 = m_CameraPivot.localPosition;
				localPosition2.x = Mathf.Lerp(localPosition2.x, localPosition.x, 5f * Time.deltaTime);
				if (m_PlayerMovement.IsCrouched)
				{
					localPosition2.y = Mathf.Lerp(localPosition2.y, localPosition.y, 5f * Time.deltaTime);
				}
				else
				{
					localPosition2.y = Mathf.Lerp(localPosition2.y, localPosition.y, 5f * Time.deltaTime);
				}
				localPosition2.z = Mathf.Lerp(localPosition2.z, localPosition.z, 5f * Time.deltaTime);
				m_CameraPivot.localPosition = localPosition2;
				if (GameManager.Instance.GameCamera.ArmsContainer != null)
				{
					Vector3 localPosition3 = GameManager.Instance.GameCamera.ArmsContainer.localPosition;
					float a = m_CameraPivot.localPosition.x * 1.5f;
					a = Mathf.Lerp(a, localPosition.x * 1.5f, 1.5f * Time.deltaTime);
					localPosition3.x = 0f - a;
					GameManager.Instance.GameCamera.ArmsContainer.localPosition = localPosition3;
				}
			}
			else
			{
				m_CameraPivot.localPosition = m_InternalTracker.transform.localPosition;
			}
		}
		else
		{
			m_CameraPivot.localPosition = m_InternalTracker.transform.localPosition;
		}
		if (m_CanHeadContainerSlerp)
		{
			if (m_HeadContainer.localPosition != Vector3.zero)
			{
				m_HeadContainer.localPosition = Vector3.Slerp(m_HeadContainer.localPosition, Vector3.zero, 5f * Time.deltaTime);
			}
			if (m_HeadContainer.localRotation != Quaternion.identity)
			{
				m_HeadContainer.localRotation = Quaternion.Slerp(m_HeadContainer.localRotation, Quaternion.identity, 5f * Time.deltaTime);
			}
		}
	}

	private void CheckFloorMaterial()
	{
		Vector3 start = base.transform.position + Vector3.up * CharacterController.height;
		Vector3 vector = base.transform.position + Vector3.down;
		if (Physics.Linecast(start, vector, out var hitInfo, ~(int)m_FloorMaterialIgnoreLayers, QueryTriggerInteraction.Collide))
		{
			Debug.DrawLine(start, hitInfo.point, Color.green);
			FloorMaterial component = hitInfo.transform.GetComponent<FloorMaterial>();
			if (S13CheckIfSecondCastRequired(component))
			{
				S13StepInInkPuddle();
				Vector3 end = vector + Vector3.down * 0.1f;
				if (Physics.Linecast(hitInfo.point, end, out var hitInfo2, ~(int)m_FloorMaterialIgnoreLayers, QueryTriggerInteraction.Collide))
				{
					component = hitInfo2.transform.GetComponent<FloorMaterial>();
				}
			}
			S13SetFloorMaterial(component);
		}
		else
		{
			Debug.DrawLine(start, vector, Color.red);
		}
	}

	public void UnlockPhotoMode()
	{
	}

	public void ShowFirstPersonArms()
	{
		if (base.Abilities == null)
		{
			ModelLayers.DisableAbility();
		}
		if (m_WeaponState == null)
		{
			ModelLayers.DisableWeapon();
		}
		else
		{
			ShowWeapon();
		}
	}

	public void HideFirstPersonArms()
	{
		HideAbility();
		HideWeapon();
	}

	public void ShowAbility(Action onComplete = null)
	{
		if (base.Abilities == null)
		{
			return;
		}
		GameCamera.SetFirstPersonArmsActive(active: true);
		m_AbilityTracker.SetActive(active: true);
		if (m_AbilitySequence != null)
		{
			m_AbilitySequence.Kill();
			m_AbilitySequence = null;
		}
		m_AbilitySequence = DOTween.Sequence();
		float animatorLayer = AnimatorBody.GetLayerWeight(1);
		m_AbilitySequence.Insert(0.01f, DOTween.To(() => animatorLayer, delegate(float x)
		{
			AnimatorBody.SetLayerWeight(1, x);
		}, 1f, 0.5f).SetEase(Ease.Linear));
		if (AnimatorArms != null)
		{
			float armAnimatorLayer = AnimatorArms.GetLayerWeight(1);
			m_AbilitySequence.Insert(0.01f, DOTween.To(() => armAnimatorLayer, delegate(float x)
			{
				AnimatorArms.SetLayerWeight(1, x);
			}, 1f, 0.5f).SetEase(Ease.Linear));
		}
		m_AbilitySequence.OnComplete(delegate
		{
			onComplete?.Invoke();
		});
		ModelLayers.EnableAbility();
	}

	public void ShowAbilityInstant(Action onComplete = null)
	{
		if (base.Abilities != null)
		{
			GameCamera.SetFirstPersonArmsActive(active: true);
			AnimatorBody.SetLayerWeight(1, 1f);
			if (AnimatorArms != null)
			{
				AnimatorArms.SetLayerWeight(1, 1f);
			}
			onComplete?.Invoke();
			ModelLayers.EnableAbility();
		}
	}

	public void HideAbility(Action onComplete = null)
	{
		if (m_AbilitySequence != null)
		{
			m_AbilitySequence.Kill();
			m_AbilitySequence = null;
		}
		m_AbilitySequence = DOTween.Sequence();
		if (AnimatorBody != null)
		{
			float animatorLayer = AnimatorBody.GetLayerWeight(1);
			m_AbilitySequence.Insert(0f, DOTween.To(() => animatorLayer, delegate(float x)
			{
				AnimatorBody.SetLayerWeight(1, x);
			}, 0f, 0.5f).SetEase(Ease.Linear));
		}
		if (AnimatorArms != null)
		{
			float armAnimatorLayer = AnimatorArms.GetLayerWeight(1);
			m_AbilitySequence.Insert(0f, DOTween.To(() => armAnimatorLayer, delegate(float x)
			{
				AnimatorArms.SetLayerWeight(1, x);
			}, 0f, 0.5f).SetEase(Ease.Linear));
		}
		m_AbilitySequence.OnComplete(delegate
		{
			ModelLayers.DisableAbility();
			onComplete?.Invoke();
		});
		m_AbilityTracker.SetActive(active: false);
	}

	public void SetNewWeapon(WeaponData weaponData)
	{
		DestroyWeapon();
		SetWeapon(new PlayerWeaponGentPipe(this, weaponData));
		m_WeaponState = (PlayerWeaponGentPipe)base.CurrentWeapon;
		GentPipePowerIndicator[] componentsInChildren = base.gameObject.GetComponentsInChildren<GentPipePowerIndicator>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SetPowerLevel(GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.Power);
		}
		MeshRenderer[] componentsInChildren2 = m_WeaponState.Weapon.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		ActiveWeaponMirrorDuplicate = DuplicateWeapon(componentsInChildren2, "Mirror", shadowsOnly: false);
		ActiveWeaponDuplicate = DuplicateWeapon(componentsInChildren2, "Player", shadowsOnly: true);
		m_WeaponState.SwapShaderToFirstPerson();
		GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetWeapon(m_WeaponState.Weapon.Data.WeaponType);
	}

	public void EquipWeapon(WeaponData weaponData)
	{
		if (weaponData.WeaponType != WeaponType.NONE)
		{
			SetNewWeapon(weaponData);
			GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetWeapon(weaponData.WeaponType);
			GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetStatus(WeaponStatus.Active);
			ShowWeapon();
		}
	}

	public void ShowWeapon()
	{
		if (m_WeaponState == null || GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.WeaponStatus != WeaponStatus.Active)
		{
			return;
		}
		m_WeaponState.SetFirstPersonFlag(active: true);
		GameCamera.SetFirstPersonArmsActive(active: true);
		m_BodyWeaponParent.gameObject.SetActive(value: true);
		m_WeaponTracker.SetActive(active: true);
		AnimatorBody.SetInteger("HandWeaponStatus", 1);
		if (AnimatorArms != null)
		{
			AnimatorArms.SetInteger("HandWeaponStatus", 1);
		}
		ModelLayers.EnableWeapon();
		if (m_WeaponSequence != null)
		{
			m_WeaponSequence.Kill();
			m_WeaponSequence = null;
		}
		m_WeaponSequence = DOTween.Sequence();
		float animatorLayer = AnimatorBody.GetLayerWeight(2);
		m_WeaponSequence.Insert(0.01f, DOTween.To(() => animatorLayer, delegate(float x)
		{
			AnimatorBody.SetLayerWeight(2, x);
		}, 1f, 0.5f).SetEase(Ease.Linear));
		if (AnimatorArms != null)
		{
			float armAnimatorLayer = AnimatorArms.GetLayerWeight(2);
			m_WeaponSequence.Insert(0.01f, DOTween.To(() => armAnimatorLayer, delegate(float x)
			{
				AnimatorArms.SetLayerWeight(2, x);
			}, 1f, 0.5f).SetEase(Ease.Linear));
		}
		m_WeaponSequence.OnComplete(m_WeaponState.Enable);
	}

	private void DestroyWeapon()
	{
		if (m_WeaponState != null)
		{
			m_WeaponState.Dispose();
			m_WeaponState = null;
		}
		ClearWeapon();
		UnityEngine.Object.Destroy(ActiveWeaponDuplicate);
		UnityEngine.Object.Destroy(ActiveWeaponMirrorDuplicate);
	}

	public void RemoveWeapon()
	{
		HideWeapon();
		if (m_WeaponState != null)
		{
			m_WeaponState.Dispose();
			m_WeaponState = null;
		}
		ClearWeapon();
		UnityEngine.Object.Destroy(ActiveWeaponDuplicate, 0.5f);
		UnityEngine.Object.Destroy(ActiveWeaponMirrorDuplicate, 0.5f);
	}

	public void HideWeapon()
	{
		if (m_WeaponState != null)
		{
			m_WeaponState.SetFirstPersonFlag(active: false);
		}
		if (m_WeaponSequence != null)
		{
			m_WeaponSequence.Kill();
			m_WeaponSequence = null;
		}
		m_WeaponSequence = DOTween.Sequence();
		if (AnimatorBody != null)
		{
			float animatorLayer = AnimatorBody.GetLayerWeight(2);
			m_WeaponSequence.Insert(0f, DOTween.To(() => animatorLayer, delegate(float x)
			{
				AnimatorBody.SetLayerWeight(2, x);
			}, 0f, 0.3f).SetEase(Ease.Linear));
		}
		if (AnimatorArms != null)
		{
			float armAnimatorLayer = AnimatorArms.GetLayerWeight(2);
			m_WeaponSequence.Insert(0f, DOTween.To(() => armAnimatorLayer, delegate(float x)
			{
				AnimatorArms.SetLayerWeight(2, x);
			}, 0f, 0.3f).SetEase(Ease.Linear));
		}
		m_WeaponSequence.OnComplete(delegate
		{
			ModelLayers.DisableWeapon();
			GameCamera.SetFirstPersonArmsActive(active: false);
			if (m_BodyWeaponParent != null)
			{
				m_BodyWeaponParent.gameObject.SetActive(value: false);
			}
		});
		if (m_WeaponTracker != null)
		{
			m_WeaponTracker.SetActive(active: false);
		}
		AnimatorBody.SetInteger("HandWeaponStatus", 0);
		if (AnimatorArms != null)
		{
			AnimatorArms.SetInteger("HandWeaponStatus", 0);
		}
	}

	private GameObject DuplicateWeapon(MeshRenderer[] weaponRenderers, string layer, bool shadowsOnly)
	{
		GameObject gameObject = new GameObject("DuplicateWeapon");
		gameObject.transform.SetParent(m_WeaponParent);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localEulerAngles = Vector3.zero;
		GentPipePowerIndicator gentPipePowerIndicator = new GameObject("Weapon_GentPipe_PowerIndicator").AddComponent<GentPipePowerIndicator>();
		gentPipePowerIndicator.transform.SetParent(gameObject.transform);
		gentPipePowerIndicator.transform.localPosition = Vector3.zero;
		List<GameObject> list = new List<GameObject>();
		GentPipeChargeEffects gentPipeChargeEffects = new GameObject("ChargeEffects").AddComponent<GentPipeChargeEffects>();
		gentPipeChargeEffects.transform.SetParent(gameObject.transform);
		gentPipeChargeEffects.transform.localPosition = Vector3.zero;
		gentPipeChargeEffects.transform.localEulerAngles = Vector3.zero;
		gentPipeChargeEffects.transform.localScale = Vector3.one;
		GameObject gameObject2 = new GameObject("ChargeLevel");
		gameObject2.transform.SetParent(gentPipeChargeEffects.transform);
		gameObject2.transform.localPosition = new Vector3(-0.409f, 0.6395195f, -0.1221f);
		gameObject2.transform.localEulerAngles = Vector3.zero;
		gameObject2.transform.localScale = Vector3.one;
		foreach (MeshRenderer meshRenderer in weaponRenderers)
		{
			GameObject gameObject3 = UnityEngine.Object.Instantiate(meshRenderer.gameObject);
			foreach (Transform item in gameObject3.transform)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
			gameObject3.name = meshRenderer.gameObject.name;
			gameObject3.transform.SetParent(gameObject.transform);
			gameObject3.transform.position = meshRenderer.transform.position;
			gameObject3.transform.eulerAngles = meshRenderer.transform.eulerAngles;
			gameObject3.layer = LayerMask.NameToLayer(layer);
			if (shadowsOnly)
			{
				gameObject3.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
			}
			if (meshRenderer.gameObject.name.ToLower().Contains("powerlvl"))
			{
				list.Add(gameObject3);
			}
			else if (meshRenderer.gameObject.name.ToLower().Contains("powerindicator"))
			{
				gameObject3.transform.SetParent(gentPipePowerIndicator.transform);
			}
			else if (meshRenderer.gameObject.name.ToLower().Contains("filler"))
			{
				gameObject3.transform.SetParent(gameObject2.transform);
			}
		}
		gentPipePowerIndicator.Initialize(list.ToArray());
		for (int j = 0; j < list.Count; j++)
		{
			list[j].transform.SetParent(gentPipePowerIndicator.transform);
		}
		gentPipeChargeEffects.SetChargeLevel(gameObject2.transform);
		gameObject.transform.SetParent(m_BodyWeaponParent);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localEulerAngles = Vector3.zero;
		return gameObject;
	}

	private void S13SetFloorMaterial(FloorMaterial floorMaterial)
	{
		if (floorMaterial != null)
		{
			CurrentFloorType = floorMaterial.FloorType;
			if (PlayerAudioController != null)
			{
				PlayerAudioController.SetFloorMaterial(CurrentFloorType);
			}
		}
	}

	private bool S13CheckIfSecondCastRequired(FloorMaterial floorMaterial)
	{
		if (floorMaterial != null)
		{
			return floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.InkPuddle;
		}
		return false;
	}

	private void S13StepInInkPuddle()
	{
		if (PlayerAudioController != null)
		{
			PlayerAudioController.StepInInkPuddle();
		}
	}

	public void S13SetPlayerReaction(BATDRPlayerAudioController.PlayerReaction reaction)
	{
		if (PlayerAudioController != null)
		{
			PlayerAudioController.React(reaction);
		}
	}

	public void S13SetCartMove(bool isMoving)
	{
		if (PlayerAudioController != null)
		{
			PlayerAudioController.SetCartMove(isMoving);
		}
	}

	public void S13SetClimbing(bool isClimbing)
	{
		if (PlayerAudioController != null)
		{
			PlayerAudioController.SetClimbing(isClimbing);
		}
	}

	public void S13SetHiding(bool isHiding)
	{
		if (PlayerAudioController != null)
		{
			PlayerAudioController.SetHiding(isHiding);
		}
	}

	public void S13SetCrouching(bool isCrouching)
	{
		if (PlayerAudioController != null)
		{
			PlayerAudioController.SetCrouching(isCrouching);
		}
	}

	protected override void OnDisposed()
	{
		this.OnAbilityUse = null;
		this.OnAnimationComplete = null;
		this.OnAnimationInteract = null;
		this.OnDeath = null;
		this.OnRespawn = null;
		this.OnAttackStart = null;
		this.OnForceDeath = null;
		m_CrouchSequence?.Kill();
		m_CrouchSequence = null;
		if (HandMaterial != null)
		{
			HandMaterial.SetTexture("_Emission", null);
			HandMaterial.SetFloat("_EmitPower", 0f);
		}
		if (test_inkdemon != null)
		{
			test_inkdemon.Dispose();
		}
		GameManager.Instance.Player = null;
		CharacterController = null;
		base.OnDisposed();
	}

	public void InkDemonTest()
	{
		if (__InkDemonActive)
		{
			return;
		}
		if (test_inkdemon != null)
		{
			test_inkdemon.Enable();
		}
		__InkDemonActive = true;
		GameManager.Instance.ShowNotificationText(TextUtility.GetKey("NOTIFICATION_INK_DEMON"), isInkDemon: true);
		TestShakeLoop();
		CameraEffects.InkDemon(10f);
		DOTween.Sequence().InsertCallback(10f, delegate
		{
			if ((bool)test_inkdemon)
			{
				test_inkdemon.SetActive(active: true);
			}
			__InkDemonActive = false;
		});
	}

	private void TestShakeLoop()
	{
		if (__InkDemonActive)
		{
			CameraEffects.ShakeRotation(1.5f, 0.5f, 10, 90f, fadeOut: false);
			DOTween.Sequence().InsertCallback(1.5f, TestShakeLoop);
		}
	}
}
