using DG.Tweening;
using JDS.PostProcessing;
using UnityEngine;

public abstract class PlayerAbilityState : AbilityState<Player, State.PlayerAbility>
{
	protected const string POWER_SHADER_TEXTURE = "_Emission";

	protected const string POWER_SHADER_EMISSION = "_EmitPower";

	protected const string POWER_SHADER_EMISSION_COLOR = "_EmissionColor";

	protected const float POWER_EMISSION_OFF = 0f;

	protected const float POWER_EMISSION = 0.7f;

	protected const float POWER_EMISSION_BOOST = 1.3f;

	protected const float POWER_EMISSION_USE = 2f;

	protected Color POWER_EMISSION_COLOR = new Color32(120, 60, 5, byte.MaxValue);

	protected const float POST_PROCESS_EFFECT_OFF = 0f;

	protected const float POST_PROCESS_EFFECT_MAX = 1f;

	protected Texture2D m_Sigil;

	protected Ability m_AbilityPostProcessEffect;

	protected float m_AbilityUseDelay;

	protected float m_AbilityUseDuration = 0.5f;

	protected float m_AbilityDistanceMin;

	protected float m_AbilityDistanceMax;

	protected float m_Cooldown;

	private Sequence m_HandMaterialSequence;

	protected Color POWER_EMISSION_COLOR_BOOST => POWER_EMISSION_COLOR * 1.25f;

	protected Color POWER_EMISSION_COLOR_USE => POWER_EMISSION_COLOR * 2f;

	protected Material m_HandMaterial => base.Actor.HandMaterial;

	public float _Cooldown => m_Cooldown;

	protected virtual float m_CooldownCount { get; set; }

	public float _CooldownCount => m_CooldownCount;

	public bool IsReady { get; protected set; }

	public bool CanUse { get; protected set; }

	public bool IsUsed { get; protected set; }

	public bool IsBoosting { get; private set; }

	public bool IsCooldown { get; protected set; }

	protected virtual bool UseInput()
	{
		return false;
	}

	public PlayerAbilityState(Player player, State.PlayerAbility id)
		: base(player, id)
	{
	}

	protected sealed override void InternalOnStateEnter()
	{
		InternalAbilityInitializer();
		SetActive(active: false);
		m_CooldownCount = m_Cooldown;
		IsCooldown = false;
		ResetHandSequence();
		m_HandMaterialSequence.OnComplete(StateEnterOnComplete);
	}

	public abstract void InternalAbilityInitializer();

	private void StateEnterOnComplete()
	{
		SetActive(active: true);
		GameManager.Instance.ShowCrosshair();
		ShowAbility();
		InternalStateEnterOnComplete();
	}

	protected virtual void InternalStateEnterOnComplete()
	{
	}

	protected override void InternalUpdateInput()
	{
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus != AbilityStatus.Active)
		{
			return;
		}
		if (IsCooldown && m_Cooldown <= m_CooldownCount)
		{
			m_Cooldown += Time.deltaTime;
			CheckCooldown();
			if (m_Cooldown >= m_CooldownCount)
			{
				m_Cooldown = m_CooldownCount;
				IsCooldown = false;
				CooldownComplete();
			}
		}
		if ((base.Actor.CombatStatus != CombatStatus.Hide || base.Actor.CharacterController.enabled) && base.Actor.CurrentState == State.Player.Default && m_Cooldown >= m_CooldownCount && !base.Actor.IsAbilityLocked && IsReady && IsUsed && CanUse && UseInput())
		{
			UseAbility();
		}
	}

	protected virtual void CheckCooldown()
	{
	}

	protected virtual void CooldownComplete()
	{
	}

	public void ForceCooldown()
	{
		ForceSetCooldown(0f);
		IsCooldown = true;
	}

	public void ForceSetCooldown(float cooldown)
	{
		m_Cooldown = cooldown;
		if (m_Cooldown > m_CooldownCount)
		{
			m_Cooldown = m_CooldownCount;
		}
	}

	private void ShowAbility()
	{
		IsUsed = true;
		SetActive(active: false);
		ShowSigil();
		IsReady = true;
		SetActive(active: true);
	}

	private void ShowSigil()
	{
		ResetHandSequence();
		float atPosition = 0f;
		float duration = 0f;
		m_HandMaterial.SetTexture("_Emission", m_Sigil);
		m_HandMaterialSequence.Insert(atPosition, DOEmission("_EmitPower", 0.7f, duration, Ease.OutSine));
		m_HandMaterialSequence.Insert(atPosition, DOColor("_EmissionColor", POWER_EMISSION_COLOR, duration, Ease.OutSine));
	}

	private void UseAbility()
	{
		if (Use())
		{
			base.Actor.Ability();
			IsCooldown = true;
			m_Cooldown = 0f;
		}
		else
		{
			IsCooldown = false;
			m_Cooldown = m_CooldownCount;
		}
		IsUsed = false;
		IsReady = false;
	}

	protected sealed override bool CheckReady()
	{
		if (base.Actor.CurrentState != State.Player.Default)
		{
			CanUse = false;
			return false;
		}
		if (!IsReady)
		{
			CanUse = false;
			InternalNotReady();
		}
		return IsReady;
	}

	protected virtual void InternalNotReady()
	{
	}

	protected sealed override bool CheckUse()
	{
		return InternalCheckUse();
	}

	protected abstract bool InternalCheckUse();

	public sealed override bool Use()
	{
		if (CheckUse())
		{
			SetActive(active: false);
			GameManager.Instance.Player.Interaction.ResetInteraction();
			GameManager.Instance.ClearInteraction();
			ResetPostProcessEffect();
			ResetHandSequence();
			m_HandMaterial.SetTexture("_Emission", m_Sigil);
			m_HandMaterial.SetFloat("_EmitPower", 0.7f);
			m_HandMaterial.SetColor("_EmissionColor", POWER_EMISSION_COLOR);
			float abilityUseDelay = m_AbilityUseDelay;
			float duration = 0.1f;
			m_HandMaterialSequence.Insert(abilityUseDelay, DOEmission("_EmitPower", 2f, duration, Ease.InSine));
			m_HandMaterialSequence.Insert(abilityUseDelay, DOColor("_EmissionColor", POWER_EMISSION_COLOR_USE, duration, Ease.InSine));
			abilityUseDelay += 0.1f;
			duration = m_AbilityUseDuration;
			m_HandMaterialSequence.Insert(abilityUseDelay, DOEmission("_EmitPower", 0f, duration, Ease.InSine));
			m_HandMaterialSequence.Insert(abilityUseDelay, DOColor("_EmissionColor", POWER_EMISSION_COLOR, duration, Ease.InSine));
			return InternalUseClearCheck();
		}
		return false;
	}

	private bool InternalUseClearCheck()
	{
		bool result = InternalUse();
		ClearCheck();
		return result;
	}

	protected abstract bool InternalUse();

	protected void UseOnComplete()
	{
		InternalUseOnComplete();
		ResetPostProcessEffect(active: false);
		Reset();
	}

	protected virtual void InternalUseOnComplete()
	{
	}

	public sealed override void Reset()
	{
		ResetHandSequence();
		float atPosition = 0f;
		float duration = 0.4f;
		m_HandMaterialSequence.Insert(atPosition, DOEmission("_EmitPower", 0f, duration, Ease.InSine));
		m_HandMaterialSequence.Insert(atPosition, DOColor("_EmissionColor", POWER_EMISSION_COLOR, duration, Ease.InSine));
		m_HandMaterialSequence.OnComplete(RestOnComplete);
	}

	private void RestOnComplete()
	{
		base.Actor.HideAbility(HideAbilityOnComplete);
		ShowAbility();
		InternalResetOnComplete();
	}

	protected virtual void InternalResetOnComplete()
	{
	}

	private void HideAbilityOnComplete()
	{
		SetActive(active: true);
	}

	protected void BoostAbilityEmission()
	{
		if (!IsBoosting)
		{
			IsBoosting = true;
			ResetHandSequence();
			float atPosition = 0f;
			float duration = 0.4f;
			m_HandMaterialSequence.Insert(atPosition, DOEmission("_EmitPower", 1.3f, duration, Ease.InOutSine));
			m_HandMaterialSequence.Insert(atPosition, DOColor("_EmissionColor", POWER_EMISSION_COLOR_BOOST, duration, Ease.InOutSine));
		}
	}

	protected void ResetAbilityEmission()
	{
		if (IsBoosting)
		{
			IsBoosting = false;
			ResetHandSequence();
			float atPosition = 0f;
			float duration = 0.4f;
			m_HandMaterialSequence.Insert(atPosition, DOEmission("_EmitPower", 0.7f, duration, Ease.InOutSine));
			m_HandMaterialSequence.Insert(atPosition, DOColor("_EmissionColor", POWER_EMISSION_COLOR, duration, Ease.InOutSine));
		}
	}

	protected void DOKillHandMaterial()
	{
		m_HandMaterial.DOKill();
	}

	protected Tween DOEmission(string name, float endValue, float duration, Ease ease)
	{
		return m_HandMaterial.DOFloat(endValue, name, duration).SetEase(ease);
	}

	protected Tween DOColor(string name, Color endValue, float duration, Ease ease)
	{
		return m_HandMaterial.DOColor(endValue, name, duration).SetEase(ease);
	}

	protected Tween DOPostProcess(float endValue, float duration, Ease ease)
	{
		int hashCode = m_AbilityPostProcessEffect._Power.value.GetHashCode();
		DOTween.Kill(hashCode);
		return DOTween.To(() => m_AbilityPostProcessEffect._Power.value, delegate(float x)
		{
			m_AbilityPostProcessEffect._Power.value = x;
		}, endValue, duration).SetEase(ease).SetId(hashCode);
	}

	public void ResetReady()
	{
		IsReady = false;
		IsUsed = false;
	}

	protected void ResetPostProcessEffect(bool active = true)
	{
		if (m_AbilityPostProcessEffect != null)
		{
			m_AbilityPostProcessEffect.active = active;
			m_AbilityPostProcessEffect._Power.value = 0f;
		}
	}

	public void ForceCancel()
	{
		IsCooldown = false;
		m_Cooldown = m_CooldownCount;
		if (m_AbilityPostProcessEffect != null && m_AbilityPostProcessEffect._Power != null)
		{
			m_AbilityPostProcessEffect._Power.value = 0f;
		}
		Reset();
		InternalForceCancel();
	}

	protected virtual void InternalForceCancel()
	{
	}

	private void ResetHandSequence()
	{
		KillHandSequence();
		DOKillHandMaterial();
		m_HandMaterialSequence = DOTween.Sequence();
	}

	private void KillHandSequence()
	{
		if (m_HandMaterialSequence != null)
		{
			m_HandMaterialSequence.Kill();
			m_HandMaterialSequence = null;
		}
	}

	protected override void OnDisposed()
	{
		KillHandSequence();
		DOKillHandMaterial();
		base.OnDisposed();
	}
}
