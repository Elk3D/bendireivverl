public class FPPlayerStateCutscene : FPPlayerState
{
    public FPPlayerStateCutscene(FPCharacterController player, FPState.Player id)
        : base(player, id) { }

    protected override void InternalOnStateEnter()
    {
        base.Actor.CancelMovement();
        base.Actor.Movement.StopRun();
    }

    protected override void InternalUpdate() { }
    protected override void InternalFixedUpdate() { }
    protected override void InternalLateUpdate() { }
    protected override void InternalOnStateExit() { }

    protected override void OnDisposed()
    {
        base.OnDisposed();
    }
}
