public abstract class AbilityState<TActor, TIdentifier> : State<TActor, TIdentifier>
{
	protected AbilityState(TActor actor, TIdentifier id)
		: base(actor)
	{
		base.ID = id;
	}

	protected abstract bool CheckReady();

	protected abstract bool CheckUse();

	protected abstract void ClearCheck();

	public abstract bool Use();

	public abstract void Reset();

	public sealed override void OnStateEnter()
	{
		InternalOnStateEnter();
	}

	protected abstract void InternalOnStateEnter();

	public sealed override void OnStateExit()
	{
		ClearCheck();
		InternalOnStateExit();
	}

	protected abstract void InternalOnStateExit();

	public sealed override void Update()
	{
		GetActions(InternalUpdate, InternalUpdateInput);
	}

	protected abstract void InternalUpdate();

	protected abstract void InternalUpdateInput();

	public sealed override void FixedUpdate()
	{
		GetActions(CheckFixedUpdate);
	}

	private void CheckFixedUpdate()
	{
		if (CheckReady())
		{
			InternalFixedUpdate();
		}
	}

	protected abstract void InternalFixedUpdate();

	public sealed override void LateUpdate()
	{
		GetActions(InternalLateUpdate);
	}

	protected abstract void InternalLateUpdate();

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
