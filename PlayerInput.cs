using InControl;
using UnityEngine;

public class PlayerInput
{
	public static bool IsInverted;

	public static float Sensitivity = 1f;

	public static bool HasController { get; private set; }

	public static bool CheckController()
	{
		if (!HasController && InputManager.ActiveDevice.IsActive)
		{
			HasController = true;
		}
		else if (!InputManager.ActiveDevice.IsActive && (LookXRaw() != 0f || LookYRaw() != 0f))
		{
			HasController = false;
		}
		return HasController;
	}

	public static bool Any()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.Action1) || GamepadInput.GetButtonDown(InputControlType.Action2) || GamepadInput.GetButtonDown(InputControlType.Action3) || GamepadInput.GetButtonDown(InputControlType.Action4), Input.anyKeyDown);
	}

	public static float MoveX()
	{
		return GetInput(GamepadInput.GetAxis(InputControlType.LeftStickX), Input.GetAxis("Horizontal"));
	}

	public static float MoveY()
	{
		return GetInput(GamepadInput.GetAxis(InputControlType.LeftStickY), Input.GetAxis("Vertical"));
	}

	public static float MoveXRaw()
	{
		return GetInput(GamepadInput.GetAxisRaw(InputControlType.LeftStickX), Input.GetAxisRaw("Horizontal"));
	}

	public static float MoveYRaw()
	{
		return GetInput(GamepadInput.GetAxisRaw(InputControlType.LeftStickY), Input.GetAxisRaw("Vertical"));
	}

	public static float LookX(float TSpeed = 1f)
	{
		float num = 3.5f;
		int num2 = (int)(Sensitivity * 10f);
		if (num2 <= 0)
		{
			num2 = 0;
		}
		else if (num2 >= 10)
		{
			num2 = 10;
		}
		float num3 = 0f;
		num3 = ((num2 > 5) ? ((float)(num2 - 5) * 0.5f) : (0f - (float)(5 - num2) * 0.5f));
		num += num3;
		return GetInput(GamepadInput.GetAxis(InputControlType.RightStickX) * (num * 1.5f + TSpeed), Input.GetAxis("Mouse X") * (num / 4f));
	}

	public static float LookY()
	{
		float num = 3f;
		int num2 = (int)(Sensitivity * 10f);
		if (num2 <= 0)
		{
			num2 = 0;
		}
		else if (num2 >= 10)
		{
			num2 = 10;
		}
		float num3 = 0f;
		num3 = ((num2 > 5) ? ((float)(num2 - 5) * 0.4f) : (0f - (float)(5 - num2) * 0.4f));
		num += num3;
		float num4 = ((!IsInverted) ? 1 : (-1));
		return GetInput(GamepadInput.GetAxis(InputControlType.RightStickY) * (num * 1.5f), Input.GetAxis("Mouse Y") * (num / 4f)) * num4;
	}

	public static float LookXRaw()
	{
		return GetInput(GamepadInput.GetAxisRaw(InputControlType.RightStickX), Input.GetAxisRaw("Mouse X"));
	}

	public static float LookYRaw()
	{
		float num = ((!IsInverted) ? 1 : (-1));
		return GetInput(GamepadInput.GetAxisRaw(InputControlType.RightStickY), Input.GetAxisRaw("Mouse Y")) * num;
	}

	public static bool Cancel()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.Action2), Input.GetKeyDown(KeyCode.F));
	}

	public static bool Attack()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.RightTrigger), Input.GetMouseButtonDown(0));
	}

	public static bool AttackHold()
	{
		return GetInput(GamepadInput.GetButton(InputControlType.RightTrigger), Input.GetMouseButton(0));
	}

	public static bool AttackUp()
	{
		return GetInput(GamepadInput.GetButtonUp(InputControlType.RightTrigger), Input.GetMouseButtonUp(0));
	}

	public static bool AttackSecondary()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.LeftTrigger), Input.GetMouseButtonDown(1));
	}

	public static bool AttackSecondaryHold()
	{
		return GetInput(GamepadInput.GetButton(InputControlType.LeftTrigger), Input.GetMouseButton(1));
	}

	public static bool AttackSecondaryRelease()
	{
		return GetInput(GamepadInput.GetButtonUp(InputControlType.LeftTrigger), Input.GetMouseButtonUp(1));
	}

	public static bool AttackCharge()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.RightBumper), Input.GetKeyDown(KeyCode.Tab));
	}

	public static bool Run()
	{
		return GetInput(GamepadInput.GetButton(InputControlType.LeftStickButton) || GamepadInput.GetButton(InputControlType.LeftBumper), Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
	}

	public static bool Jump()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.Action1), Input.GetKeyDown(KeyCode.Space));
	}

	public static bool JumpButtonUp()
	{
		return GetInput(GamepadInput.GetButtonUp(InputControlType.Action1), Input.GetKeyUp(KeyCode.Space));
	}

	public static bool JumpHold()
	{
		return GetInput(GamepadInput.GetButton(InputControlType.Action1), Input.GetKey(KeyCode.Space));
	}

	public static bool Crouch()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.RightStickButton), Input.GetKeyDown(KeyCode.C));
	}

	public static bool CrouchButtonUp()
	{
		return GetInput(GamepadInput.GetButtonUp(InputControlType.RightStickButton), Input.GetKeyUp(KeyCode.C));
	}

	public static bool Pause()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.Start) || GamepadInput.GetButtonDown(InputControlType.Options) || GamepadInput.GetButtonDown(InputControlType.Menu), Input.GetKeyDown(KeyCode.Escape));
	}

	public static bool SpecialActionM()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.Action4), Input.GetKeyDown(KeyCode.M));
	}

	public static bool SpecialAction()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.Action4), Input.GetKeyDown(KeyCode.Y));
	}

	public static bool SpecialActionReleased()
	{
		return GetInput(GamepadInput.GetButtonUp(InputControlType.Action4), Input.GetKeyUp(KeyCode.Y));
	}

	public static bool SpecialActionHold()
	{
		return GetInput(GamepadInput.GetButton(InputControlType.Action4), Input.GetKey(KeyCode.Y));
	}

	public static bool InteractOnPressed()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.Action3), Input.GetKeyDown(KeyCode.E));
	}

	public static bool InteractOnReleased()
	{
		return GetInput(GamepadInput.GetButtonUp(InputControlType.Action3), Input.GetKeyUp(KeyCode.E));
	}

	public static bool InteractOnHold()
	{
		return GetInput(GamepadInput.GetButton(InputControlType.Action3), Input.GetKey(KeyCode.E));
	}

	public static float ControllerY()
	{
		float axisRaw = GamepadInput.GetAxisRaw(InputControlType.LeftStickY);
		if (axisRaw == 0f)
		{
			axisRaw = GamepadInput.GetAxisRaw(InputControlType.DPadY);
		}
		return GetInput(axisRaw, 0f);
	}

	public static float ControllerX()
	{
		float axisRaw = GamepadInput.GetAxisRaw(InputControlType.LeftStickX);
		if (axisRaw == 0f)
		{
			axisRaw = GamepadInput.GetAxisRaw(InputControlType.DPadX);
		}
		return GetInput(axisRaw, 0f);
	}

	public static bool ControllerAction1()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.Action1), inputKeyboard: false);
	}

	public static bool ControllerAction1Hold()
	{
		return GetInput(GamepadInput.GetButton(InputControlType.Action1), inputKeyboard: false);
	}

	public static bool ControllerAction1Up()
	{
		return GetInput(GamepadInput.GetButtonUp(InputControlType.Action1), inputKeyboard: false);
	}

	public static bool ControllerAction2()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.Action2), inputKeyboard: false);
	}

	public static bool ControllerAction3()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.Action3), inputKeyboard: false);
	}

	public static bool ControllerStart()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.Start) || GamepadInput.GetButtonDown(InputControlType.Options) || GamepadInput.GetButtonDown(InputControlType.Menu), inputKeyboard: false);
	}

	public static bool ControllerLeftBumper()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.LeftBumper), inputKeyboard: false);
	}

	public static bool ControllerRightBumper()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.RightBumper), inputKeyboard: false);
	}

	public static bool ControllerLeftTrigger()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.LeftTrigger), inputKeyboard: false);
	}

	public static bool ControllerRightTrigger()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.RightTrigger), inputKeyboard: false);
	}

	public static bool VirtualMouseLeftOnPressed()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.Action1), Input.GetMouseButtonDown(0));
	}

	public static bool VirtualMouseLeftOnReleased()
	{
		return GetInput(GamepadInput.GetButtonUp(InputControlType.Action1), Input.GetMouseButtonUp(0));
	}

	public static bool VirtualMouseRightOnPressed()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.Action2), Input.GetMouseButtonDown(1));
	}

	public static bool VirtualMouseRightOnReleased()
	{
		return GetInput(GamepadInput.GetButtonUp(InputControlType.Action2), Input.GetMouseButtonUp(1));
	}

	public static bool VirtualMouseMiddleOnPressed()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.Action3), Input.GetMouseButtonDown(2));
	}

	public static bool VirtualMouseMiddleOnReleased()
	{
		return GetInput(GamepadInput.GetButtonUp(InputControlType.Action3), Input.GetMouseButtonUp(2));
	}

	private static T GetInput<T>(T inputGamepad, T inputKeyboard)
	{
		T result = default(T);
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
		return result;
	}
}
