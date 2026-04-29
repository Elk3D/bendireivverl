public abstract class FPPlayerState : FPStateBase<FPCharacterController, FPState.Player>
{
    protected FPPlayerState(FPCharacterController player, FPState.Player id) : base(player)
    {
        ID = id;
    }

    public sealed override void OnStateEnter()
    {
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
        InternalOnStateExit();
    }

    protected abstract void InternalOnStateExit();

    protected override void OnDisposed()
    {
        base.OnDisposed();
    }
}
