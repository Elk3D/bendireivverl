using System;
using UnityEngine;

namespace S13Audio.BATDR;

[DefaultExecutionOrder(1)]
public class S13Speedometer : MonoBehaviour
{
	private enum SpeedCalcMethod
	{
		Static,
		PlayerMovement
	}

	private enum SpeedScaleMethod
	{
		Ignore,
		Static,
		ScaleMagnitude
	}

	[S13LockableField]
	[SerializeField]
	private string accessorID = string.Empty;

	[SerializeField]
	private SpeedCalcMethod speedCalcMethod = SpeedCalcMethod.PlayerMovement;

	[SerializeField]
	private SpeedScaleMethod speedScaleMethod = SpeedScaleMethod.Static;

	[SerializeField]
	private Player player;

	[Header("Falling Settings")]
	[SerializeField]
	private bool enableFallspeedCalculation = true;

	[SerializeField]
	[Tooltip("If Y movement is less than this value, begin falling")]
	private float beginFallingYMovement = -0.4f;

	[SerializeField]
	[Tooltip("The Y movement that is considered to be max falling speed")]
	private float maxFallingYMovement = -0.7f;

	[SerializeField]
	[S13CurveRange(0f, 0f, 1f, 1f)]
	private AnimationCurve fallSpeedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[ReadOnly]
	[SerializeField]
	private float curSpeed;

	private float lastSpeed;

	private void Start()
	{
		if (!player)
		{
			S13Debug.LogWarning("Player not assigned to S13SpeedParameter. Disabling");
			base.enabled = false;
		}
	}

	private void Update()
	{
		curSpeed = 0f;
		float num = 1f;
		switch (speedCalcMethod)
		{
		case SpeedCalcMethod.Static:
			if (player.PlayerMovement.IsCrouched)
			{
				curSpeed = 3.75f;
			}
			else if (player.PlayerMovement.IsRunning)
			{
				curSpeed = 14.5f;
			}
			else
			{
				curSpeed = 7.75f;
			}
			break;
		case SpeedCalcMethod.PlayerMovement:
			curSpeed = player.PlayerMovement.CurrentSpeed * 3.33f;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		switch (speedScaleMethod)
		{
		case SpeedScaleMethod.Static:
			if (player.PlayerMovement.MovementInput.magnitude <= 0f)
			{
				num = 0f;
			}
			break;
		case SpeedScaleMethod.ScaleMagnitude:
			num = Mathf.Clamp01(player.PlayerMovement.MovementInput.magnitude);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case SpeedScaleMethod.Ignore:
			break;
		}
		curSpeed *= num;
		float num2 = 0f;
		if (enableFallspeedCalculation)
		{
			if (player.PlayerMovement.MoveDirection.y < beginFallingYMovement)
			{
				float time = Normalize01(player.PlayerMovement.MoveDirection.y, beginFallingYMovement, maxFallingYMovement);
				num2 = fallSpeedCurve.Evaluate(time);
			}
			curSpeed += num2;
		}
		curSpeed = Mathf.Clamp01(curSpeed);
		if (!Mathf.Approximately(curSpeed, lastSpeed))
		{
			lastSpeed = curSpeed;
			S13Manager.SetAccessor(accessorID, curSpeed);
		}
	}

	private float Normalize01(float value, float minInput, float maxInput)
	{
		return Mathf.Abs(value - minInput) / Mathf.Abs(maxInput - minInput);
	}
}
