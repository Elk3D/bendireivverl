using UnityEngine;

public class PlayerAbilityStateFastTravel : PlayerAbilityState
{
	public PlayerAbilityStateFastTravel(Player player, State.PlayerAbility id)
		: base(player, id)
	{
	}

	public override void InternalAbilityInitializer()
	{
		m_Sigil = GameManager.Instance.AssetManager.GetAsset<Texture2D>("Audrey_Sigil_FastTravel");
		m_AbilityUseDelay = 0f;
		m_AbilityDistanceMax = 6f;
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
		return true;
	}

	protected override void InternalUpdate()
	{
	}

	protected override void InternalFixedUpdate()
	{
	}

	protected override void ClearCheck()
	{
		if (base.CanUse)
		{
			base.CanUse = false;
			ResetAbilityEmission();
		}
	}

	protected override void InternalLateUpdate()
	{
	}

	protected override void InternalOnStateExit()
	{
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
