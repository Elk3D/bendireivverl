using System;
using DG.Tweening;
using JDS.PostProcessing;
using UnityEngine;

public class PlayerAbilityStateFlow : PlayerAbilityState
{
	private LayerMask m_TeleportLayers;

	private RaycastHit m_TargetHit;

	private RaycastHit m_TargetColliderToHit;

	private Vector3 m_TargetPosition;

	private Vector3 m_TargetPositionOffset;

	private Vector3 m_ToTargetPosition;

	private Vector3 m_ToTargetColliderPosition;

	private Sequence m_Sequence;

	private FlowMantle m_FlowMantle;

	private bool m_CanFlow;

	protected override float m_CooldownCount => UpgradeCheck.GetAbility();

	public PlayerAbilityStateFlow(Player player, State.PlayerAbility id)
		: base(player, id)
	{
	}

	protected override void InternalStateEnterOnComplete()
	{
		GameManager.Instance.ShowTeleport();
	}

	public override void InternalAbilityInitializer()
	{
		m_TeleportLayers = ~((1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("AI")));
		m_AbilityPostProcessEffect = GameManager.Instance.GameCamera.BasePostProcess.profile.GetSetting<Teleport>();
		m_Sigil = GameManager.Instance.AssetManager.GetAsset<Texture2D>("Audrey_Sigil_Flow");
		m_AbilityUseDelay = 0.1f;
		m_AbilityDistanceMin = 3f;
		m_AbilityDistanceMax = 25f;
		m_Cooldown = m_CooldownCount;
	}

	protected override void CheckCooldown()
	{
		GameManager.Instance.ShowTeleportCooldown(m_Cooldown, m_CooldownCount);
	}

	protected override void CooldownComplete()
	{
		GameManager.Instance.ShowTeleportCooldownComplete();
	}

	protected override bool UseInput()
	{
		if (base.Actor.IsCrouchSequenceActive)
		{
			return false;
		}
		return PlayerInput.AttackSecondary();
	}

	protected override bool InternalCheckUse()
	{
		bool canUse = base.CanUse;
		if (canUse)
		{
			base.m_HandMaterial.SetTexture("_Emission", m_Sigil);
		}
		return canUse;
	}

	protected override bool InternalUse()
	{
		base.Actor.ShowAbilityInstant();
		base.Actor.Ability();
		if (m_CanFlow)
		{
			base.Actor.SetHeadContainerSlerp(active: false);
			base.Actor.SetState(State.Player.Ability);
		}
		else
		{
			base.Actor.CancelMovement();
			base.Actor.PlayerMovement.StopRun();
		}
		GameManager.Instance.GameCamera.TeleportParticles.Play();
		GameManager.Instance.UseTeleportAbility(use: true);
		ResetSequence();
		float num = 0f;
		float num2 = 0.25f;
		if (m_CanFlow)
		{
			m_Sequence.Insert(num, base.Actor.transform.DOLocalMove(m_ToTargetPosition, num2 * 2f).SetEase(Ease.InSine));
		}
		else
		{
			base.Actor.AddForce(base.Actor.GameCamera.transform.forward * 50f);
		}
		m_Sequence.Insert(num, DOPostProcess(1f, num2, Ease.Linear));
		num += num2;
		m_Sequence.Insert(num, DOPostProcess(0f, num2, Ease.Linear));
		m_Sequence.OnComplete(base.UseOnComplete);
		return true;
	}

	protected override void InternalUseOnComplete()
	{
		GameManager.Instance.GameCamera.TeleportParticles.Stop();
		if (m_FlowMantle != null)
		{
			base.Actor.SetHeadContainerSlerp(active: true);
			m_FlowMantle.InteractableAnimation.OnInteractionComplete -= HandleFlowMantleOnInteractionComplete;
			m_FlowMantle.InteractableAnimation.OnInteractionComplete += HandleFlowMantleOnInteractionComplete;
			m_FlowMantle.Enter();
		}
		else if (m_CanFlow)
		{
			base.Actor.SetHeadContainerSlerp(active: true);
			base.Actor.ResetRotation();
			base.Actor.SetState(State.Player.Default);
		}
		else
		{
			base.Actor.CancelMovement();
		}
	}

	private void HandleFlowMantleOnInteractionComplete(object sender, EventArgs e)
	{
		m_FlowMantle.InteractableAnimation.OnInteractionComplete -= HandleFlowMantleOnInteractionComplete;
		m_FlowMantle = null;
		base.Actor.ResetRotation();
		base.Actor.SetState(State.Player.Default);
	}

	protected override void InternalUpdate()
	{
	}

	protected override void InternalFixedUpdate()
	{
		Vector3 vector = base.Actor.HeadContainer.position - base.Actor.HeadContainer.forward;
		Vector3 forward = base.Actor.HeadContainer.forward;
		Vector3 hitPoint = vector + forward * m_AbilityDistanceMax;
		if (Physics.SphereCast(vector, 0.2f, forward, out m_TargetHit, m_AbilityDistanceMax, m_TeleportLayers, QueryTriggerInteraction.Ignore))
		{
			m_FlowMantle = m_TargetHit.transform.GetComponent<FlowMantle>();
			hitPoint = m_TargetHit.point;
		}
		if (m_FlowMantle != null)
		{
			m_ToTargetPosition = m_FlowMantle.StartPosition;
			m_CanFlow = true;
			BoostAbilityEmission();
		}
		else if (CheckTarget(vector, hitPoint))
		{
			m_CanFlow = true;
			BoostAbilityEmission();
		}
		else
		{
			m_CanFlow = false;
			BoostAbilityEmission();
		}
	}

	protected override void ClearCheck()
	{
		if (base.CanUse)
		{
			base.CanUse = false;
		}
	}

	private bool CheckTarget(Vector3 origin, Vector3 hitPoint)
	{
		float height = base.Actor.CharacterController.height;
		float radius = base.Actor.CharacterController.radius;
		m_TargetPosition = hitPoint;
		Vector3 vector = ((m_TargetPosition.y > origin.y + radius) ? Vector3.down : Vector3.up);
		Vector3 vector2 = origin - m_TargetPosition;
		m_TargetPositionOffset = m_TargetPosition + (vector2 / vector2.magnitude).normalized * (radius * 2f);
		m_TargetPositionOffset.y = m_TargetPosition.y;
		m_TargetPositionOffset += vector * radius * 1.1f;
		float maxDistance = height;
		if (vector.y < 0f)
		{
			m_TargetPositionOffset += vector * radius;
			maxDistance = radius * 1.5f;
		}
		if (Physics.SphereCast(m_TargetPositionOffset, radius, -vector, out m_TargetColliderToHit, maxDistance, m_TeleportLayers, QueryTriggerInteraction.Ignore))
		{
			m_ToTargetColliderPosition = m_TargetColliderToHit.point;
			if (vector.y < 0f)
			{
				m_ToTargetColliderPosition.y -= height + radius / 2f;
			}
			m_ToTargetPosition = m_ToTargetColliderPosition;
		}
		else
		{
			m_ToTargetPosition = (m_ToTargetColliderPosition = ((vector.y < 0f) ? (m_TargetPositionOffset + vector * (height - radius * 2f)) : (m_TargetPositionOffset - vector * (height + radius))));
		}
		if (Physics.SphereCast(m_ToTargetPosition + Vector3.up, radius, Vector3.up * height, out var _, height - 1.1f - radius, m_TeleportLayers, QueryTriggerInteraction.Ignore))
		{
			m_ToTargetPosition = base.Actor.transform.position;
		}
		if (m_ToTargetPosition == Vector3.zero || m_ToTargetPosition == base.Actor.transform.position)
		{
			return false;
		}
		Vector3 toTargetPosition = m_ToTargetPosition;
		bool flag = Vector3.Distance(base.Actor.transform.position, toTargetPosition) > m_AbilityDistanceMin;
		if (!base.CanUse)
		{
			base.CanUse = true;
		}
		if (flag)
		{
			Vector3 vector3 = toTargetPosition + Vector3.up * height;
			if (Physics.Linecast(toTargetPosition, vector3, m_TeleportLayers, QueryTriggerInteraction.Ignore))
			{
				flag = false;
			}
			toTargetPosition += Vector3.up * 0.01f;
			if (Physics.Linecast(vector3, toTargetPosition, m_TeleportLayers, QueryTriggerInteraction.Ignore))
			{
				flag = false;
			}
		}
		return flag;
	}

	protected override void InternalLateUpdate()
	{
	}

	protected override void InternalResetOnComplete()
	{
		m_TargetHit = default(RaycastHit);
		m_TargetPosition = Vector3.zero;
		m_TargetPositionOffset = Vector3.zero;
		m_ToTargetPosition = base.Actor.transform.position;
	}

	protected override void InternalOnStateExit()
	{
	}

	protected override void InternalForceCancel()
	{
		KillSequence();
		GameManager.Instance.GameCamera.TeleportParticles.Stop();
		base.Actor.SetHeadContainerSlerp(active: true);
		base.Actor.ResetRotation();
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
		m_Sequence.SetUpdate(UpdateType.Fixed);
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			m_Sequence.Kill();
			m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		KillSequence();
		base.OnDisposed();
	}
}
