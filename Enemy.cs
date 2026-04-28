using DG.Tweening;
using UnityEngine;

public class Enemy : Character
{
	private int m_EnemyID;

	[SerializeField]
	private float m_PatrolSpeed = 2f;

	[Header("Enemy Type")]
	[SerializeField]
	private EnemyType m_EnemyType;

	[Header("Health Settings")]
	[Header("Easy")]
	[SerializeField]
	private int m_MaxHitPointsEasy = 9;

	[Header("Normal")]
	[SerializeField]
	private int m_MaxHitPoints = 9;

	[Header("Hard")]
	[SerializeField]
	private int m_MaxHitPointsHard = 9;

	[Header("Hit Settings")]
	[SerializeField]
	private bool m_HasHitAnimation = true;

	[Header("Stun Settings")]
	[SerializeField]
	private AnimationClip m_StunAnimationClip;

	[Header("Loot")]
	[SerializeField]
	private bool m_IsLootable = true;

	[Header("Despawn")]
	[SerializeField]
	private AnimationClip m_DespawnAnimationClip;

	[Header("Banish")]
	[SerializeField]
	private bool m_CanBanish;

	[SerializeField]
	private AnimationClip m_AudreyBanishClip;

	[Header("Initialize Options")]
	[SerializeField]
	private bool m_UseAwake = true;

	private SectionID m_SectionID;

	private Shock m_Shock;

	protected int m_HitPoints;

	private float m_DeathCheck;

	private bool m_IsDeathChecked;

	private bool m_IsImmune;

	public override bool JumpInput => false;

	public override bool CrouchInput => false;

	public override bool RunInput => m_RunInput;

	public override float MoveXInput => m_MoveXSpeed;

	public override float MoveYInput => m_MoveSpeed;

	public override float RotateXInput => 1f;

	public override bool AttackInput => false;

	public float PatrolSpeed => m_PatrolSpeed;

	public EnemyType EnemyType => m_EnemyType;

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

	public bool HasHitAnimation => m_HasHitAnimation;

	public AnimationClip StunAnimationClip => m_StunAnimationClip;

	public bool IsLootable => m_IsLootable;

	public AnimationClip DespawnAnimationClip => m_DespawnAnimationClip;

	public bool CanBanish => m_CanBanish;

	public AnimationClip AudreyBanishClip => m_AudreyBanishClip;

	protected override bool UseAwake => m_UseAwake;

	public SectionID SectionID => m_SectionID;

	public int HitPoints => m_HitPoints;

	public bool IsImmune => m_IsImmune;

	public void SetSection(SectionID sectionID)
	{
		m_SectionID = sectionID;
	}

	protected override void CharacterInitialized()
	{
		m_Shock = GetComponentInChildren<Shock>();
	}

	protected override void InternalUpdate()
	{
		if (base.m_State.ID == State.Character.Death && base.Content.RagdollController == null)
		{
			if (m_DeathCheck >= 5f && !m_IsDeathChecked)
			{
				m_IsDeathChecked = true;
				if (base.Content.Animator != null)
				{
					base.Content.Animator.enabled = true;
					base.Content.Animator.SetTrigger("Death");
				}
			}
			else
			{
				m_DeathCheck += Time.deltaTime;
			}
		}
		else if (!(base.Target == null))
		{
			CharacterNode component = base.Target.GetComponent<CharacterNode>();
			if (component != null && !(component.GetComponent<CharacterInteractionNode>() != null) && Vector3.Distance(base.Target.position, base.transform.position) <= base.Agent.Agent.stoppingDistance)
			{
				ForceNodeOnReached(0.75f);
				SetTarget(null);
			}
		}
	}

	protected override void InternalFixedUpdate()
	{
		if (base.Rotation != null)
		{
			base.Rotation.SetSensitivity(3f);
		}
	}

	public override void OnAttack()
	{
		if (m_ActiveAction != null)
		{
			m_ActiveAction.Action(this);
			m_ActiveAction = null;
		}
	}

	public override void OnAttackRanged()
	{
		if (!(GameManager.Instance.Player == null) && !(GameManager.Instance.GameCamera == null))
		{
			Projectile component = GameManager.Instance.PoolingManager.GetFromPool("Projectiles/Projectile_Ink").GetComponent<Projectile>();
			if (base.Content.EyeSight != null)
			{
				component.transform.position = base.Content.EyeSight.position + base.transform.forward * 2f;
			}
			else
			{
				component.transform.position = base.transform.position + Vector3.up * (base.Agent.Agent.height / 2f) + base.transform.forward * 2f;
			}
			component.transform.LookAt(GameManager.Instance.GameCamera.transform.position);
			component.gameObject.SetActive(value: true);
			component.Initialize();
		}
	}

	public void SetHitPoints(int hitPoints)
	{
		m_HitPoints = hitPoints;
	}

	public void SetImmune(bool active)
	{
		m_IsImmune = active;
	}

	public override void OnHit(RaycastHit hit)
	{
		if (base.CurrentState == State.Character.Death)
		{
			return;
		}
		bool flag = m_EnemyType == EnemyType.LostOneColor;
		string prefab = "Effects/Effects_Hit_Ink";
		if (flag)
		{
			prefab = "Effects/Effects_Hit_Ink_Color";
		}
		if (m_EnemyType == EnemyType.ShipAhoy)
		{
			prefab = "Effects/Effects_Hit_Smoke";
		}
		else if (EnemyType == EnemyType.Keeper)
		{
			prefab = "Impacts/Impact_Sparks";
		}
		GameManager.Instance.PoolingManager.GetFromPool(prefab, 6f).transform.position = hit.point;
		bool flag2 = false;
		bool flag3 = false;
		if (GameManager.Instance.Player != null && GameManager.Instance.Player.CurrentWeapon != null && GameManager.Instance.Player.CurrentWeapon.IsCharged)
		{
			GentPipeChargeEffects componentInChildren = GameManager.Instance.Player.CurrentWeapon.Weapon.GetComponentInChildren<GentPipeChargeEffects>();
			if (componentInChildren != null)
			{
				flag2 = componentInChildren.IsCharged;
				flag3 = componentInChildren.IsOvercharged;
				componentInChildren.Hit();
				GameManager.Instance.PoolingManager.GetFromPool("Impacts/Impact_Electric", 1f).GetComponent<Impact>().Initialize(hit, isPooled: true);
			}
		}
		if (!m_IsImmune && (MaxHitPoints >= 99 || MaxHitPoints == -1))
		{
			SetImmune(active: true);
		}
		if (!m_IsImmune)
		{
			m_HitPoints += ((!flag2) ? 1 : (flag3 ? 3 : 2));
		}
		SendOnDamageTaken();
		if (!m_IsImmune)
		{
			if (m_HitPoints >= MaxHitPoints)
			{
				SetState(State.Character.Death);
				ForceStop(smooth: false);
				base.Agent.Agent.enabled = false;
				base.Controller.enabled = false;
				base.Content.UpdateMaterials();
				if (base.Content.RagdollController != null)
				{
					base.Content.Animator.enabled = false;
					base.Content.RagdollController.Activate(hit, flag);
				}
				else
				{
					if (m_EnemyType == EnemyType.InkWidow)
					{
						base.Content.Animator.speed = 1f * Random.Range(1f, 1.5f);
					}
					else if (m_EnemyType == EnemyType.Searcher)
					{
						base.Content.Animator.speed = 1f * Random.Range(1f, 1.25f);
					}
					ClearAnimationTriggers();
					base.Content.Animator.SetTrigger("Death");
					Collider[] componentsInChildren = base.Content.GetComponentsInChildren<Collider>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].enabled = true;
					}
				}
				if (m_IsLootable)
				{
					LostOneType lostOneType = LostOneType.NONE;
					if (m_EnemyType == EnemyType.LostOne || m_EnemyType == EnemyType.LostOneColor)
					{
						LostOneCharacterContent lostOneCharacterContent = base.Content as LostOneCharacterContent;
						if (lostOneCharacterContent != null)
						{
							lostOneType = lostOneCharacterContent.LostOneType;
						}
					}
					LootableEnemy lootableEnemy = new GameObject("LootableEnemy").AddComponent<LootableEnemy>();
					lootableEnemy.Initialize(base.Content.gameObject, m_SectionID, m_EnemyType, lostOneType, flag);
					lootableEnemy.AddData();
				}
				CharacterDeath();
				if (flag2)
				{
					GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddShockKill();
				}
				else
				{
					GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddKill();
				}
				if (m_EnemyType == EnemyType.Searcher || m_EnemyType == EnemyType.LostOne || m_EnemyType == EnemyType.LostOneColor)
				{
					SectionDataObject sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(m_SectionID);
					if (sectionDataObject != null && sectionDataObject.EnemyData != null)
					{
						EnemyDataObject enemyDataObject = (EnemyDataObject)sectionDataObject.EnemyData.GetValue(m_EnemyID);
						if (enemyDataObject != null)
						{
							sectionDataObject.EnemyData.Remove(enemyDataObject);
						}
					}
				}
			}
			else
			{
				HitAnimation();
				HitGlowFader();
			}
		}
		else
		{
			if (m_EnemyType == EnemyType.Keeper)
			{
				if (flag3 && m_StunAnimationClip != null)
				{
					OnShock();
					SetState(State.Character.Stun);
				}
			}
			else if (m_EnemyType == EnemyType.KingWidow)
			{
				HitAnimation();
			}
			HitGlowFader();
		}
		if (flag2)
		{
			OnShock();
		}
		if (base.Content.RagdollController != null && base.CurrentState == State.Character.Death)
		{
			Dispose();
		}
	}

	private void HitAnimation()
	{
		if (base.m_State.ID != State.Character.Hit && base.m_State.ID != State.Character.Attack && base.m_State.ID != State.Character.Flee && base.m_State.ID != State.Character.Evade && m_HasHitAnimation)
		{
			SetState(State.Character.Hit);
		}
	}

	public void SetHitAnimation(bool active)
	{
		m_HasHitAnimation = active;
	}

	private void HitGlowFader()
	{
		object[] modelRenderers = base.Content.ModelRenderers;
		ShaderEffects.Fade("_PostHitGlow", 2f, 0.3f, 0.75f, modelRenderers);
	}

	public void OnShock()
	{
		m_Shock?.Play();
	}

	public void AutoAttack()
	{
		if (base.CurrentState != State.Character.Flee && base.m_PreviousState != State.Character.Flee)
		{
			if (base.Target != GameManager.Instance.Player.transform)
			{
				SetTarget(GameManager.Instance.Player.transform);
			}
			SetState(State.Character.Follow);
		}
	}

	public void Takedown()
	{
		if (base.Content.Animator != null)
		{
			base.Content.Animator.SetTrigger("Banish");
		}
		SetState(State.Character.Death);
		ForceStop(smooth: false);
		base.Agent.Agent.enabled = false;
		base.Controller.enabled = false;
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		Sequence sequence = DOTween.Sequence();
		float duration = 5.25f;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material material = componentsInChildren[i].material;
			material.SetFloat("_DeathPower", 0.25f);
			sequence.Insert(0f, material.DOFloat(1f, "_DeathPower", duration).SetEase(Ease.InSine));
		}
		sequence.OnComplete(TakedownOnComplete);
	}

	private void TakedownOnComplete()
	{
		CharacterDeath();
		Dispose();
	}

	public override void OnDeath()
	{
		Dispose();
	}

	public void AddData()
	{
		SectionDataObject sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(m_SectionID);
		m_EnemyID = base.gameObject.GetInstanceID();
		while (sectionDataObject.EnemyData.ContainsKey(m_EnemyID))
		{
			m_EnemyID += Random.Range(-100, 100);
		}
		EnemyDataObject enemyDataObject = DataObject<int, EnemyDataObject>.Create(m_EnemyID);
		enemyDataObject.SetEnemyType(m_EnemyType);
		if (base.Content is LostOneCharacterContent lostOneCharacterContent)
		{
			enemyDataObject.SetLostOneType(lostOneCharacterContent.LostOneType);
		}
		if (base.CurrentNode != null)
		{
			enemyDataObject.SetCurrentNode(base.CurrentNode.ID);
		}
		enemyDataObject.SetGameObjectData(base.gameObject);
		sectionDataObject.EnemyData.Add(m_EnemyID, enemyDataObject);
	}

	public void UpdateData()
	{
		if (!base.IsDisposed)
		{
			EnemyDataObject enemyDataObject = (EnemyDataObject)((SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(m_SectionID)).EnemyData.GetValue(m_EnemyID);
			enemyDataObject.SetEnemyType(m_EnemyType);
			if (base.Content is LostOneCharacterContent lostOneCharacterContent)
			{
				enemyDataObject.SetLostOneType(lostOneCharacterContent.LostOneType);
			}
			if (base.CurrentNode != null)
			{
				enemyDataObject.SetCurrentNode(base.CurrentNode.ID);
			}
			if (GameManager.Instance.Player != null)
			{
				enemyDataObject.SetCombatState(base.Target == GameManager.Instance.Player.transform);
			}
			else
			{
				enemyDataObject.SetCombatState(inCombat: false);
			}
			enemyDataObject.SetGameObjectData(base.gameObject);
		}
	}

	public void SetData(int id)
	{
		m_EnemyID = id;
		EnemyDataObject enemyDataObject = (EnemyDataObject)((SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(m_SectionID)).EnemyData.GetValue(m_EnemyID);
		if (enemyDataObject != null)
		{
			base.gameObject.SetData(enemyDataObject.GameObjectDatas);
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
