using InControl;

public static class FPGamepadInput
{
    public static bool GetButton(InputControlType t) => GetControl(t)?.IsPressed ?? false;
    public static bool GetButtonDown(InputControlType t) => GetControl(t)?.WasPressed ?? false;
    public static bool GetButtonUp(InputControlType t) => GetControl(t)?.WasReleased ?? false;
    public static float GetAxis(InputControlType t) => GetControl(t)?.Value ?? 0f;
    public static float GetAxisRaw(InputControlType t) => GetControl(t)?.RawValue ?? 0f;

    private static InputControl GetControl(InputControlType t) => InputManager.ActiveDevice?.GetControl(t);
}
