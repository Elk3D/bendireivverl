public class CharacterState : State<Character, State.Character>
{
	public CharacterState(Character character, State.Character id)
		: base(character)
	{
		base.ID = id;
	}

	public sealed override void OnStateEnter()
	{
		JDebug.Log("[" + base.Actor.name + "] - OnStateEnter :: " + base.ID, base.Actor, JDebug.JDebugType.AI);
		InternalOnStateEnter();
	}

	public virtual void InternalOnStateEnter()
	{
	}

	public sealed override void Update()
	{
		GetActions(InternalUpdate);
	}

	protected virtual void InternalUpdate()
	{
	}

	public sealed override void FixedUpdate()
	{
		GetActions(InternalFixedUpdate);
	}

	protected virtual void InternalFixedUpdate()
	{
	}

	public sealed override void LateUpdate()
	{
		GetActions(InternalLateUpdate);
	}

	protected virtual void InternalLateUpdate()
	{
	}

	public override void OnStateExit()
	{
		JDebug.Log("[" + base.Actor.name + "] - OnStateExit :: " + base.ID, base.Actor, JDebug.JDebugType.AI);
		InternalOnStateExit();
	}

	public virtual void InternalOnStateExit()
	{
	}

	protected bool CheckTarget()
	{
		if (base.Actor.Target != null)
		{
			return true;
		}
		return false;
	}
}
