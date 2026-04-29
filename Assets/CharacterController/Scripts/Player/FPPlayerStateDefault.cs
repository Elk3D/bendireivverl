public class FPPlayerStateDefault : FPPlayerState
{
    public FPPlayerStateDefault(FPCharacterController player, FPState.Player id)
        : base(player, id) { }

    protected override void InternalOnStateEnter() { }

    protected override void InternalUpdate()
    {
        base.Actor.UpdateLookInput();
        if (!base.Actor.Settings.SmoothCamera)
            base.Actor.UpdateRotations();
        base.Actor.UpdateMovementInput();
        base.Actor.UpdateInteractionInput();
    }

    protected override void InternalFixedUpdate()
    {
        if (base.Actor.Settings.SmoothCamera)
            base.Actor.UpdateRotations();
        base.Actor.UpdateMovement();
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
