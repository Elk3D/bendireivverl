public abstract class FPPlayerStateMachine : FPStateMachineBase<FPCharacterController, FPState.Player>
{
    protected sealed override void Update()
    {
        GetActions(base.Update, InternalUpdate);
    }

    protected virtual void InternalUpdate() { }

    protected sealed override void FixedUpdate()
    {
        GetActions(base.FixedUpdate, InternalFixedUpdate);
    }

    protected virtual void InternalFixedUpdate() { }

    protected sealed override void LateUpdate()
    {
        GetActions(base.LateUpdate, InternalLateUpdate);
    }

    protected virtual void InternalLateUpdate() { }

    protected override void OnDisposed()
    {
        base.OnDisposed();
    }
}
