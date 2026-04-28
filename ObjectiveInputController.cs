using UnityEngine;

public class ObjectiveInputController : Objective
{
	private enum InputType
	{
		Crouch,
		Run
	}

	[SerializeField]
	private InputType m_InputType;

	protected override void InternalEnable()
	{
		Initialize();
	}

	protected override void InternalInitialize()
	{
		if (m_InputType == InputType.Crouch)
		{
			GameManager.Instance.Player.PlayerMovement.UnlockCrouch();
		}
		else if (m_InputType == InputType.Run)
		{
			GameManager.Instance.Player.PlayerMovement.ForceUnlockRun();
		}
		SendOnComplete();
	}

	protected override void InternalDisable()
	{
		if (m_InputType == InputType.Crouch)
		{
			GameManager.Instance.Player.PlayerMovement.LockCrouch();
		}
		else if (m_InputType == InputType.Run)
		{
			GameManager.Instance.Player.PlayerMovement.ForceLockRun();
		}
	}
}
