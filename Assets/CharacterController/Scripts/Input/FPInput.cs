using InControl;
using UnityEngine;

public static class FPInput
{
    public static bool IsInverted;
    public static float Sensitivity = 1f;

    public static bool HasController { get; private set; }

    public static bool CheckController()
    {
        if (!HasController && InputManager.ActiveDevice.IsActive) HasController = true;
        else if (!InputManager.ActiveDevice.IsActive && (LookXRaw() != 0f || LookYRaw() != 0f)) HasController = false;
        return HasController;
    }

    // ── Movement ──────────────────────────────────────────────────────────
    public static float MoveX() => Get(FPGamepadInput.GetAxis(InputControlType.LeftStickX), Input.GetAxis("Horizontal"));
    public static float MoveY() => Get(FPGamepadInput.GetAxis(InputControlType.LeftStickY), Input.GetAxis("Vertical"));
    public static float MoveXRaw() => Get(FPGamepadInput.GetAxisRaw(InputControlType.LeftStickX), Input.GetAxisRaw("Horizontal"));
    public static float MoveYRaw() => Get(FPGamepadInput.GetAxisRaw(InputControlType.LeftStickY), Input.GetAxisRaw("Vertical"));

    // ── Look ──────────────────────────────────────────────────────────────
    public static float LookX(float tSpeed = 1f)
    {
        float base_ = 3.5f;
        int s = (int)(Sensitivity * 10f);
        s = Mathf.Clamp(s, 0, 10);
        float adj = s > 5 ? (s - 5) * 0.5f : -(5 - s) * 0.5f;
        base_ += adj;
        return Get(FPGamepadInput.GetAxis(InputControlType.RightStickX) * (base_ * 1.5f + tSpeed),
                   Input.GetAxis("Mouse X") * (base_ / 4f));
    }

    public static float LookY()
    {
        float base_ = 3f;
        int s = (int)(Sensitivity * 10f);
        s = Mathf.Clamp(s, 0, 10);
        float adj = s > 5 ? (s - 5) * 0.4f : -(5 - s) * 0.4f;
        base_ += adj;
        float inv = IsInverted ? -1f : 1f;
        return Get(FPGamepadInput.GetAxis(InputControlType.RightStickY) * (base_ * 1.5f),
                   Input.GetAxis("Mouse Y") * (base_ / 4f)) * inv;
    }

    public static float LookXRaw() => Get(FPGamepadInput.GetAxisRaw(InputControlType.RightStickX), Input.GetAxisRaw("Mouse X"));
    public static float LookYRaw()
    {
        float inv = IsInverted ? -1f : 1f;
        return Get(FPGamepadInput.GetAxisRaw(InputControlType.RightStickY), Input.GetAxisRaw("Mouse Y")) * inv;
    }

    // ── Actions ───────────────────────────────────────────────────────────
    public static bool Run() => Get(
        FPGamepadInput.GetButton(InputControlType.LeftStickButton) || FPGamepadInput.GetButton(InputControlType.LeftBumper),
        Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));

    public static bool Jump() => Get(FPGamepadInput.GetButtonDown(InputControlType.Action1), Input.GetKeyDown(KeyCode.Space));
    public static bool JumpButtonUp() => Get(FPGamepadInput.GetButtonUp(InputControlType.Action1), Input.GetKeyUp(KeyCode.Space));
    public static bool JumpHold() => Get(FPGamepadInput.GetButton(InputControlType.Action1), Input.GetKey(KeyCode.Space));

    public static bool Crouch() => Get(FPGamepadInput.GetButtonDown(InputControlType.RightStickButton), Input.GetKeyDown(KeyCode.C));
    public static bool CrouchButtonUp() => Get(FPGamepadInput.GetButtonUp(InputControlType.RightStickButton), Input.GetKeyUp(KeyCode.C));

    public static bool Pause() => Get(
        FPGamepadInput.GetButtonDown(InputControlType.Start) || FPGamepadInput.GetButtonDown(InputControlType.Options) || FPGamepadInput.GetButtonDown(InputControlType.Menu),
        Input.GetKeyDown(KeyCode.Escape));

    public static bool InteractOnPressed() => Get(FPGamepadInput.GetButtonDown(InputControlType.Action3), Input.GetKeyDown(KeyCode.E));
    public static bool InteractOnReleased() => Get(FPGamepadInput.GetButtonUp(InputControlType.Action3), Input.GetKeyUp(KeyCode.E));
    public static bool InteractOnHold() => Get(FPGamepadInput.GetButton(InputControlType.Action3), Input.GetKey(KeyCode.E));

    // ── Internal ──────────────────────────────────────────────────────────
    private static T Get<T>(T gamepad, T keyboard)
    {
        if (!gamepad.Equals(default(T))) { HasController = true; return gamepad; }
        if (!keyboard.Equals(default(T))) { HasController = false; return keyboard; }
        return default;
    }
}
