public abstract class PlayerState : State<Player, State.Player>
{
	protected PlayerState(Player player, State.Player id)
		: base(player)
	{
		base.ID = id;
	}

	public sealed override void OnStateEnter()
	{
		DebugState("OnStateEnter");
		InternalOnStateEnter();
	}

	protected abstract void InternalOnStateEnter();

	public sealed override void Update()
	{
		GetActions(InternalUpdate);
	}

	protected abstract void InternalUpdate();

	public sealed override void FixedUpdate()
	{
		GetActions(InternalFixedUpdate);
	}

	protected abstract void InternalFixedUpdate();

	public sealed override void LateUpdate()
	{
		GetActions(InternalLateUpdate);
	}

	protected abstract void InternalLateUpdate();

	public sealed override void OnStateExit()
	{
		DebugState("OnStateExit");
		InternalOnStateExit();
	}

	protected abstract void InternalOnStateExit();

	protected void DebugState(string method)
	{
		JDebug.Log("[" + base.Actor.name + "] - " + method + " :: " + base.ID, base.Actor, JDebug.JDebugType.Player);
	}
}
