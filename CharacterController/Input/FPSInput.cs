using InControl;
using UnityEngine;

namespace CharacterController
{
    // Static input abstraction supporting keyboard/mouse and InControl gamepads.
    // Stripped to movement, look, and interaction inputs only.
    // Attack / ability / UI-virtual-mouse inputs have been removed.
    public static class FPSInput
    {
        public static bool IsInverted;
        public static float Sensitivity = 1f;

        public static bool HasController { get; private set; }

        public static bool CheckController()
        {
            if (!HasController && InputManager.ActiveDevice.IsActive)
                HasController = true;
            else if (!InputManager.ActiveDevice.IsActive && (LookXRaw() != 0f || LookYRaw() != 0f))
                HasController = false;
            return HasController;
        }

        public static bool Any()
        {
            return GetInput(
                GamepadInput.GetButtonDown(InputControlType.Action1) ||
                GamepadInput.GetButtonDown(InputControlType.Action2) ||
                GamepadInput.GetButtonDown(InputControlType.Action3) ||
                GamepadInput.GetButtonDown(InputControlType.Action4),
                Input.anyKeyDown);
        }

        // ── Movement ────────────────────────────────────────────────────────────

        public static float MoveX()    => GetInput(GamepadInput.GetAxis(InputControlType.LeftStickX),    Input.GetAxis("Horizontal"));
        public static float MoveY()    => GetInput(GamepadInput.GetAxis(InputControlType.LeftStickY),    Input.GetAxis("Vertical"));
        public static float MoveXRaw() => GetInput(GamepadInput.GetAxisRaw(InputControlType.LeftStickX), Input.GetAxisRaw("Horizontal"));
        public static float MoveYRaw() => GetInput(GamepadInput.GetAxisRaw(InputControlType.LeftStickY), Input.GetAxisRaw("Vertical"));

        public static bool Run()
        {
            return GetInput(
                GamepadInput.GetButton(InputControlType.LeftStickButton) || GamepadInput.GetButton(InputControlType.LeftBumper),
                Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        }

        public static bool Jump()          => GetInput(GamepadInput.GetButtonDown(InputControlType.Action1), Input.GetKeyDown(KeyCode.Space));
        public static bool JumpButtonUp()  => GetInput(GamepadInput.GetButtonUp(InputControlType.Action1),   Input.GetKeyUp(KeyCode.Space));
        public static bool JumpHold()      => GetInput(GamepadInput.GetButton(InputControlType.Action1),     Input.GetKey(KeyCode.Space));

        public static bool Crouch()        => GetInput(GamepadInput.GetButtonDown(InputControlType.RightStickButton), Input.GetKeyDown(KeyCode.C));
        public static bool CrouchButtonUp()=> GetInput(GamepadInput.GetButtonUp(InputControlType.RightStickButton),  Input.GetKeyUp(KeyCode.C));

        // ── Look ────────────────────────────────────────────────────────────────

        public static float LookX(float TSpeed = 1f)
        {
            float base_ = 3.5f;
            int s = (int)(Sensitivity * 10f);
            s = Mathf.Clamp(s, 0, 10);
            float offset = s > 5 ? (s - 5) * 0.5f : -(5 - s) * 0.5f;
            base_ += offset;
            return GetInput(GamepadInput.GetAxis(InputControlType.RightStickX) * (base_ * 1.5f + TSpeed),
                            Input.GetAxis("Mouse X") * (base_ / 4f));
        }

        public static float LookY()
        {
            float base_ = 3f;
            int s = (int)(Sensitivity * 10f);
            s = Mathf.Clamp(s, 0, 10);
            float offset = s > 5 ? (s - 5) * 0.4f : -(5 - s) * 0.4f;
            base_ += offset;
            float invert = IsInverted ? -1f : 1f;
            return GetInput(GamepadInput.GetAxis(InputControlType.RightStickY) * (base_ * 1.5f),
                            Input.GetAxis("Mouse Y") * (base_ / 4f)) * invert;
        }

        public static float LookXRaw() => GetInput(GamepadInput.GetAxisRaw(InputControlType.RightStickX), Input.GetAxisRaw("Mouse X"));

        public static float LookYRaw()
        {
            float invert = IsInverted ? -1f : 1f;
            return GetInput(GamepadInput.GetAxisRaw(InputControlType.RightStickY), Input.GetAxisRaw("Mouse Y")) * invert;
        }

        // ── Misc / UI ───────────────────────────────────────────────────────────

        public static bool Cancel() => GetInput(GamepadInput.GetButtonDown(InputControlType.Action2), Input.GetKeyDown(KeyCode.F));
        public static bool Pause()
        {
            return GetInput(
                GamepadInput.GetButtonDown(InputControlType.Start) ||
                GamepadInput.GetButtonDown(InputControlType.Options) ||
                GamepadInput.GetButtonDown(InputControlType.Menu),
                Input.GetKeyDown(KeyCode.Escape));
        }

        // ── Interaction ─────────────────────────────────────────────────────────

        public static bool InteractOnPressed()  => GetInput(GamepadInput.GetButtonDown(InputControlType.Action3), Input.GetKeyDown(KeyCode.E));
        public static bool InteractOnReleased() => GetInput(GamepadInput.GetButtonUp(InputControlType.Action3),   Input.GetKeyUp(KeyCode.E));
        public static bool InteractOnHold()     => GetInput(GamepadInput.GetButton(InputControlType.Action3),     Input.GetKey(KeyCode.E));

        // ── Private helper ──────────────────────────────────────────────────────

        private static T GetInput<T>(T inputGamepad, T inputKeyboard)
        {
            if (!inputGamepad.Equals(default(T)))
            {
                HasController = true;
                return inputGamepad;
            }
            if (!inputKeyboard.Equals(default(T)))
            {
                HasController = false;
                return inputKeyboard;
            }
            return default(T);
        }
    }
}
