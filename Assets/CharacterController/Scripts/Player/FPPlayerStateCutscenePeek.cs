public class FPPlayerStateCutscenePeek : FPPlayerState
{
    public FPPlayerStateCutscenePeek(FPCharacterController player, FPState.Player id)
        : base(player, id) { }

    protected override void InternalOnStateEnter()
    {
        base.Actor.CancelMovement();
        base.Actor.Movement.StopRun();
    }

    protected override void InternalUpdate()
    {
        base.Actor.UpdateLookInput();
        if (!base.Actor.Settings.SmoothCamera)
            base.Actor.UpdateAnimationRotations();
    }

    protected override void InternalFixedUpdate()
    {
        if (base.Actor.Settings.SmoothCamera)
            base.Actor.UpdateAnimationRotations();
    }

    protected override void InternalLateUpdate()
    {
        base.Actor.UpdateAnimations();
    }

    protected override void InternalOnStateExit() { }

    protected override void OnDisposed()
    {
        base.OnDisposed();
    }
}
