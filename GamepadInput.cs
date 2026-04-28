using InControl;

public static class GamepadInput
{
	public static bool GetButton(InputControlType inputControlType)
	{
		return GetControl(inputControlType)?.IsPressed ?? false;
	}

	public static bool GetButtonDown(InputControlType inputControlType)
	{
		return GetControl(inputControlType)?.WasPressed ?? false;
	}

	public static bool GetButtonUp(InputControlType inputControlType)
	{
		return GetControl(inputControlType)?.WasReleased ?? false;
	}

	public static float GetAxis(InputControlType inputControlType)
	{
		return GetControl(inputControlType)?.Value ?? 0f;
	}

	public static float GetAxisRaw(InputControlType inputControlType)
	{
		return GetControl(inputControlType)?.RawValue ?? 0f;
	}

	private static InputControl GetControl(InputControlType inputControlType)
	{
		return InputManager.ActiveDevice?.GetControl(inputControlType);
	}
}
