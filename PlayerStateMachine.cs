using System;
using System.Collections.Generic;

public abstract class PlayerStateMachine : StateMachine<Player, State.Player>
{
	private Dictionary<State.PlayerAbility, PlayerAbilityState> m_ActiveAbilities = new Dictionary<State.PlayerAbility, PlayerAbilityState>();

	public PlayerWeaponState CurrentWeapon { get; private set; }

	public List<PlayerAbilityState> Abilities { get; private set; }

	protected sealed override void Update()
	{
		GetActions(base.Update, InternalUpdate);
	}

	protected virtual void InternalUpdate()
	{
	}

	protected sealed override void FixedUpdate()
	{
		GetActions(base.FixedUpdate, InternalFixedUpdate);
	}

	protected virtual void InternalFixedUpdate()
	{
	}

	protected sealed override void LateUpdate()
	{
		GetActions(base.LateUpdate, InternalLateUpdate);
	}

	protected virtual void InternalLateUpdate()
	{
	}

	public void SetWeapon(PlayerWeaponState weapon)
	{
		CurrentWeapon?.OnStateExit();
		CurrentWeapon = weapon;
		CurrentWeapon?.OnStateEnter();
	}

	protected void ClearWeapon()
	{
		if (CurrentWeapon != null)
		{
			CurrentWeapon.Dispose();
			CurrentWeapon = null;
		}
	}

	public void SetAbility(State.PlayerAbility ability)
	{
		string text = typeof(State.PlayerAbility).ToString();
		text = text.RemoveSymbols().Replace("State", "") + "State" + ability;
		if (m_ActiveAbilities.ContainsKey(ability))
		{
			return;
		}
		PlayerAbilityState playerAbilityState = (PlayerAbilityState)Activator.CreateInstance(Type.GetType(text), this, ability);
		m_ActiveAbilities.Add(ability, playerAbilityState);
		if (!GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.ContainsKey(ability))
		{
			AbilityDataObject value = DataObject<State.PlayerAbility, AbilityDataObject>.Create(ability);
			GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.Add(ability, value);
		}
		if (Abilities == null)
		{
			Abilities = new List<PlayerAbilityState>();
		}
		if (!Abilities.Contains(playerAbilityState))
		{
			Abilities.Add(playerAbilityState);
		}
		for (int i = 0; i < Abilities.Count; i++)
		{
			PlayerAbilityState playerAbilityState2 = Abilities[i];
			if (playerAbilityState2.ID != playerAbilityState.ID)
			{
				playerAbilityState2.OnStateExit();
			}
		}
		playerAbilityState.OnStateEnter();
	}

	public bool HasTeleport()
	{
		bool result = false;
		if (Abilities != null)
		{
			for (int i = 0; i < Abilities.Count; i++)
			{
				if (Abilities[i] is PlayerAbilityStateFlow)
				{
					result = true;
				}
			}
		}
		return result;
	}

	protected override void OnDisposed()
	{
		ClearWeapon();
		if (m_ActiveAbilities != null)
		{
			m_ActiveAbilities.Clear();
			m_ActiveAbilities = null;
		}
		if (Abilities != null)
		{
			Abilities.Clear();
			Abilities = null;
		}
		base.OnDisposed();
	}
}
