using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SeasonalEnemy : Character
{
	private int m_EnemyID;

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

	[Header("Initialize Options")]
	[SerializeField]
	private bool m_UseAwake = true;

	[Header("Alert Mesh")]
	[SerializeField]
	private MeshRenderer m_AlertMesh;

	[Header("Animations")]
	[SerializeField]
	private AnimationClipOverrideGroup[] m_AnimationClipOverrideGroup;

	private SectionID m_SectionID;

	protected int m_HitPoints;

	private bool m_IsCharacterInitialized;

	private bool m_IsSeen;

	private bool m_IsLeaving;

	private CharacterInteractionNode[] m_Nodes;

	public override bool JumpInput => false;

	public override bool CrouchInput => false;

	public override bool RunInput => m_RunInput;

	public override float MoveXInput => m_MoveXSpeed;

	public override float MoveYInput => m_MoveSpeed;

	public override float RotateXInput => 1f;

	public override bool AttackInput => false;

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

	protected override bool UseAwake => m_UseAwake;

	public SectionID SectionID => m_SectionID;

	public int HitPoints => m_HitPoints;

	public void SetSection(SectionID sectionID)
	{
		m_SectionID = sectionID;
	}

	protected override void CharacterInitialized()
	{
		for (int i = 0; i < m_AnimationClipOverrideGroup.Length; i++)
		{
			m_AnimationClipOverrideGroup[i].Initialize();
		}
		m_IsCharacterInitialized = true;
	}

	protected override void InternalUpdate()
	{
		if (m_IsCharacterInitialized && !(base.Target == null))
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

	protected override void InternalLateUpdate()
	{
		if (m_IsCharacterInitialized && !m_IsSeen && m_AlertMesh != null && m_AlertMesh.isVisible)
		{
			m_IsSeen = true;
			UpdateAnimationClips("Jump");
			base.Content.SetAnimationTrigger("Interact");
		}
	}

	public override void OnHit(RaycastHit hit)
	{
		if (base.CurrentState == State.Character.Death)
		{
			return;
		}
		CancelSlide();
		if (m_IsLeaving)
		{
			m_IsLeaving = false;
			base.Content.transform.DOKill();
		}
		string prefab = "Effects/Effects_Hit_Ink";
		GameManager.Instance.PoolingManager.GetFromPool(prefab, 6f).transform.position = hit.point;
		bool flag = false;
		bool flag2 = false;
		if (GameManager.Instance.Player != null && GameManager.Instance.Player.CurrentWeapon != null && GameManager.Instance.Player.CurrentWeapon.IsCharged)
		{
			GentPipeChargeEffects componentInChildren = GameManager.Instance.Player.CurrentWeapon.Weapon.GetComponentInChildren<GentPipeChargeEffects>();
			if (componentInChildren != null)
			{
				flag = componentInChildren.IsCharged;
				flag2 = componentInChildren.IsOvercharged;
				componentInChildren.Hit();
				GameManager.Instance.PoolingManager.GetFromPool("Impacts/Impact_Electric", 1f).GetComponent<Impact>().Initialize(hit, isPooled: true);
			}
		}
		m_HitPoints += ((!flag) ? 1 : (flag2 ? 3 : 2));
		SendOnDamageTaken();
		if (m_HitPoints >= MaxHitPoints)
		{
			SetState(State.Character.Death);
			ForceStop(smooth: false);
			base.Agent.Agent.enabled = false;
			base.Controller.enabled = false;
			base.Content.UpdateMaterials();
			ClearAnimationTriggers();
			base.Content.Animator.SetTrigger("Death");
			LootableEnemy lootableEnemy = new GameObject("LootableEnemy").AddComponent<LootableEnemy>();
			lootableEnemy.Initialize(base.Content.gameObject, m_SectionID, m_EnemyType, LostOneType.NONE, isColor: false, base.transform.parent);
			lootableEnemy.AddData();
			CharacterDeath();
		}
		else
		{
			HitAnimation();
			HitGlowFader();
		}
	}

	private void HitAnimation()
	{
		if (base.m_State.ID != State.Character.SeasonalHit)
		{
			SetState(State.Character.SeasonalHit);
		}
	}

	private void HitGlowFader()
	{
		object[] modelRenderers = base.Content.ModelRenderers;
		ShaderEffects.Fade("_PostHitGlow", 2f, 0.3f, 0.75f, modelRenderers);
	}

	public void Leave()
	{
		m_IsLeaving = true;
		UpdateAnimationClips("Jump");
		base.Content.SetAnimationTrigger("Interact");
	}

	public void EnterLeave()
	{
		if (m_IsLeaving)
		{
			base.Controller.enabled = false;
			base.Content.transform.DOKill();
			base.Content.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear).OnComplete(delegate
			{
				GameManager.Instance.PoolingManager.GetFromPool("Effects/Effects_Looted_Enemy", 10f).GetComponent<LootedEnemy>().Initialize(base.transform.position);
				GameManager.Instance.PoolingManager.GetFromPool("Effects/Effects_Hit_Ink", 6f).transform.position = base.transform.position;
				Dispose();
			});
		}
	}

	public void JumpComplete()
	{
		if (!m_IsLeaving)
		{
			GetRandomNode();
		}
	}

	public void GetRandomNode()
	{
		CharacterNode node = m_Nodes[Random.Range(0, m_Nodes.Length)];
		SetNode(node);
	}

	public void SetNodes(CharacterInteractionNode[] nodes)
	{
		m_Nodes = nodes;
	}

	public override void OnDeath()
	{
		Dispose();
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
		base.Content.UpdateClipOverrides(list.ToArray());
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
