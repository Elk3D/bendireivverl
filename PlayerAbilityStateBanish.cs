using System;
using DG.Tweening;
using JDS.PostProcessing;
using UnityEngine;

public class PlayerAbilityStateBanish : PlayerAbilityState
{
	private LayerMask m_TargetCheckLayer;

	private LayerMask m_TargetColliderCheckLayer;

	private RaycastHit m_TargetHit;

	private RaycastHit m_TargetColliderHit;

	private Vector3 m_TargetPosition;

	private Vector3 m_TargetPositionOffset;

	private Vector3 m_ToTargetPosition;

	private Vector3 m_ToTargetRotation;

	private Sequence m_Sequence;

	public Enemy Target { get; private set; }

	protected override bool UseInput()
	{
		return PlayerInput.InteractOnPressed();
	}

	public PlayerAbilityStateBanish(Player player, State.PlayerAbility id)
		: base(player, id)
	{
	}

	public override void InternalAbilityInitializer()
	{
		m_TargetCheckLayer = ~((1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Audio")) | (1 << LayerMask.NameToLayer("EventTrigger")));
		m_TargetColliderCheckLayer = ~(1 << LayerMask.NameToLayer("Player"));
		m_AbilityPostProcessEffect = GameManager.Instance.GameCamera.BasePostProcess.profile.GetSetting<Takedown>();
		m_Sigil = GameManager.Instance.AssetManager.GetAsset<Texture2D>("Audrey_Sigil_Banish");
		m_AbilityUseDelay = 1.5f;
		m_AbilityUseDuration = 1.5f;
		m_AbilityDistanceMax = 5f;
	}

	protected override void InternalNotReady()
	{
		Target = null;
	}

	protected override bool InternalCheckUse()
	{
		bool num = Target != null;
		if (num)
		{
			base.m_HandMaterial.SetTexture("_Emission", m_Sigil);
		}
		return num;
	}

	protected override bool InternalUse()
	{
		Target.AudreyBanishClip.name = "Interact";
		base.Actor.UpdatePlayerContentClipOverrides(Target.AudreyBanishClip);
		base.Actor.OnAnimationComplete -= HandleEnterOnAnimationComplete;
		base.Actor.OnAnimationComplete += HandleEnterOnAnimationComplete;
		base.Actor.EnterInteraction("Interact");
		base.Actor.SetState(State.Player.Ability);
		Target.Takedown();
		ResetSequence();
		float num = 0f;
		m_Sequence.Insert(num, base.Actor.transform.DOMove(m_ToTargetPosition, 0.25f).SetEase(Ease.InSine));
		m_Sequence.Insert(num, base.Actor.transform.DORotate(m_ToTargetRotation, 0.25f).SetEase(Ease.InSine));
		GameManager.Instance.GameCamera.BanishParticles.Play();
		if (m_AbilityPostProcessEffect != null)
		{
			m_Sequence.Insert(num, DOTween.To(() => m_AbilityPostProcessEffect._Power.value, delegate(float x)
			{
				m_AbilityPostProcessEffect._Power.value = x;
			}, 1f, 0.5f).SetEase(Ease.InSine));
			num += 0.5f;
			m_Sequence.Insert(num, DOTween.To(() => m_AbilityPostProcessEffect._Power.value, delegate(float x)
			{
				m_AbilityPostProcessEffect._Power.value = x;
			}, 0.925f, 0.05f).SetLoops(17, LoopType.Yoyo).SetEase(Ease.Linear));
			num += 0.75f;
			m_Sequence.Insert(num, DOTween.To(() => m_AbilityPostProcessEffect._Power.value, delegate(float x)
			{
				m_AbilityPostProcessEffect._Power.value = x;
			}, 0f, 1.5f).SetEase(Ease.InSine));
		}
		int healAmount = GetHealAmount();
		m_Sequence.InsertCallback(2.25f, delegate
		{
			GameManager.Instance.Player.Heal(healAmount);
		});
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddBanish();
		return true;
	}

	private int GetHealAmount()
	{
		int num = (int)UpgradeCheck.GetHealth();
		int result = num / 4;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Normal:
			result = num / 3;
			break;
		case DifficultyLevel.Easy:
			result = num / 2;
			break;
		}
		return result;
	}

	private void HandleEnterOnAnimationComplete(object sender, EventArgs e)
	{
		base.Actor.OnAnimationComplete -= HandleEnterOnAnimationComplete;
		GameManager.Instance.Player.ExitAnimation();
		GameManager.Instance.GameCamera.BanishParticles.Stop();
		UseOnComplete();
	}

	protected override void InternalUpdate()
	{
	}

	protected override void InternalFixedUpdate()
	{
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus != AbilityStatus.Active)
		{
			return;
		}
		if (base.Actor.IsCrouched && (base.Actor.CombatStatus == CombatStatus.Stealth || base.Actor.CombatStatus == CombatStatus.Hide) && Physics.SphereCast(base.Actor.HeadContainer.position, 0.1f, base.Actor.HeadContainer.forward, out m_TargetHit, m_AbilityDistanceMax, m_TargetCheckLayer))
		{
			Enemy component = m_TargetHit.transform.GetComponent<Enemy>();
			if (component != null && component.CanBanish)
			{
				if (Mathf.Abs(Vector3.Angle(component.transform.forward, base.Actor.transform.position - component.transform.position)) > 120f)
				{
					if (Target != null && Target != component)
					{
						ClearCheck();
					}
					CheckTarget(component);
				}
				else
				{
					ClearCheck();
				}
			}
			else
			{
				ClearCheck();
			}
		}
		else
		{
			ClearCheck();
		}
	}

	protected override void ClearCheck()
	{
		if (!base.CanUse)
		{
			return;
		}
		base.CanUse = false;
		if (Target != null)
		{
			SkinnedMeshRenderer[] componentsInChildren = Target.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				ShaderEffects.SetFloat("_PostHitGlow", 0.3f, skinnedMeshRenderer);
				ShaderEffects.SetFloat("_Gradient", 0.1f, skinnedMeshRenderer);
				ShaderEffects.SetColor("_GradientColor", ShaderConstants.GRADIENT_DEFAULT, skinnedMeshRenderer);
			}
		}
		GameManager.Instance.HideInteraction();
		Target = null;
		m_TargetHit = default(RaycastHit);
		m_TargetColliderHit = default(RaycastHit);
		m_TargetPosition = Vector3.zero;
		m_TargetPositionOffset = Vector3.zero;
		m_ToTargetPosition = base.Actor.transform.position;
		m_ToTargetRotation = base.Actor.transform.eulerAngles;
	}

	private void CheckTarget(Enemy _target)
	{
		float num = 2.5f;
		Target = _target;
		m_TargetPosition = Target.transform.position;
		m_TargetPositionOffset = m_TargetPosition - Target.transform.forward * num;
		float height = base.Actor.CharacterController.height;
		float radius = base.Actor.CharacterController.radius;
		if (Physics.SphereCast(m_TargetPositionOffset, radius + 0.01f, Vector3.up, out m_TargetColliderHit, height - radius, m_TargetColliderCheckLayer, QueryTriggerInteraction.Ignore) || Physics.SphereCast(m_TargetPosition + Vector3.up * radius, radius + 0.01f, -Target.transform.forward, out m_TargetColliderHit, num, m_TargetColliderCheckLayer, QueryTriggerInteraction.Ignore))
		{
			ClearCheck();
			return;
		}
		m_ToTargetPosition = m_TargetPositionOffset;
		m_ToTargetRotation = Target.transform.eulerAngles;
		if (base.CanUse)
		{
			return;
		}
		base.CanUse = true;
		if (Target != null)
		{
			SkinnedMeshRenderer[] componentsInChildren = Target.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				ShaderEffects.SetFloat("_PostHitGlow", 2f, skinnedMeshRenderer);
				ShaderEffects.SetFloat("_Gradient", 1f, skinnedMeshRenderer);
				ShaderEffects.SetColor("_GradientColor", ShaderConstants.GRADIENT_OUTLINE, skinnedMeshRenderer);
			}
		}
		GameManager.Instance.ShowInteraction(TextUtility.GetKey(InteractionType.INTERACTION_BANISH.ToString()));
		BoostAbilityEmission();
	}

	protected override void InternalLateUpdate()
	{
	}

	protected override void InternalOnStateExit()
	{
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
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
