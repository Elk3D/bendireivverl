public abstract class PlayerWeaponState : WeaponState<Player>
{
	protected Weapon m_Weapon;

	public Weapon Weapon => m_Weapon;

	public PlayerWeaponState(Player player, WeaponData data)
		: base(player, data)
	{
	}

	protected sealed override void InternalOnStateEnter()
	{
		OnInitialize();
	}

	public abstract void OnInitialize();

	protected override void InternalUpdateInput()
	{
		if (base.Actor.CurrentState != State.Player.Default || base.Actor.IsWeaponLocked || !base.IsActive)
		{
			return;
		}
		if (PlayerInput.AttackCharge())
		{
			if (!IsCharged)
			{
				Charge();
			}
			else if (!IsOvercharged)
			{
				Overcharge();
			}
		}
		else if (PlayerInput.AttackHold() && Use())
		{
			base.Actor.Attack();
			SetActive(active: false);
		}
	}

	public bool Charge()
	{
		if (!CheckCharge())
		{
			return false;
		}
		return InternalCharge();
	}

	protected abstract bool InternalCharge();

	protected bool CheckCharge()
	{
		if (!base.IsActive)
		{
			return false;
		}
		return InternalCheckCharge();
	}

	protected abstract bool InternalCheckCharge();

	public bool Overcharge()
	{
		if (!CheckOvercharge())
		{
			return false;
		}
		return InternalOvercharge();
	}

	protected abstract bool InternalOvercharge();

	protected bool CheckOvercharge()
	{
		if (!base.IsActive)
		{
			return false;
		}
		return InternalCheckOvercharge();
	}

	protected abstract bool InternalCheckOvercharge();

	public sealed override bool Use()
	{
		if (!CheckUse())
		{
			return Fail();
		}
		return InternalUse();
	}

	protected abstract bool InternalUse();

	protected sealed override bool CheckUse()
	{
		if (!base.IsActive)
		{
			return false;
		}
		return InternalCheckUse();
	}

	protected abstract bool InternalCheckUse();

	protected bool Fail()
	{
		Reset();
		return false;
	}

	public void OnAttack()
	{
		if (CheckOnAttack())
		{
			InternalOnAttack();
		}
	}

	protected virtual bool CheckOnAttack()
	{
		return true;
	}

	protected abstract void InternalOnAttack();

	public void OnAttackComplete()
	{
		InternalOnAttackComplete();
	}

	protected virtual void InternalOnAttackComplete()
	{
	}

	public sealed override void Reset()
	{
		InternalReset();
	}

	protected abstract void InternalReset();

	protected override void InternalOnStateExit()
	{
		Dispose();
	}

	protected override void OnDisposed()
	{
		if (m_Weapon != null)
		{
			m_Weapon.Dispose();
			m_Weapon = null;
		}
		base.OnDisposed();
	}
}
