using System;
using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRPlayerHealth : MonoBehaviour
{
	private const string healthAccessorID = "Player_Health";

	[SerializeField]
	[Tooltip("Percentage at which health events occur")]
	[Range(0f, 100f)]
	private int threshold = 50;

	[SerializeField]
	private float maxTimerSpeed = 4f;

	[SerializeField]
	private float minTimerSpeed = 1f;

	[SerializeField]
	private float recoverySpeed = 3.5f;

	[Header("Actions")]
	[SerializeField]
	private S13LocalAction enterHurtState;

	[SerializeField]
	private S13LocalAction exitHurtState;

	[SerializeField]
	private S13LocalAction timerTick;

	[Header("Debug")]
	[SerializeField]
	[S13ReadOnlyField]
	private float timerSpeed;

	[SerializeField]
	[S13ReadOnlyField]
	private float heartbeatTimer;

	[SerializeField]
	[S13ReadOnlyField]
	private bool isPlayerHurt;

	private float lastValue;

	private float healthThreshold => (float)threshold / 100f;

	private void OnEnable()
	{
		BATDRPlayerHealthAudioController.OnHealthChanged = (Action<float>)Delegate.Combine(BATDRPlayerHealthAudioController.OnHealthChanged, new Action<float>(OnHealthChanged));
	}

	private void Update()
	{
		if (isPlayerHurt)
		{
			heartbeatTimer -= Time.deltaTime;
			if (heartbeatTimer <= 0f)
			{
				timerTick.Execute();
				heartbeatTimer = timerSpeed;
			}
			timerSpeed += recoverySpeed * Time.deltaTime;
			if (timerSpeed > maxTimerSpeed)
			{
				ExitHurtState();
			}
		}
	}

	private void OnDisable()
	{
		BATDRPlayerHealthAudioController.OnHealthChanged = (Action<float>)Delegate.Remove(BATDRPlayerHealthAudioController.OnHealthChanged, new Action<float>(OnHealthChanged));
	}

	private void SetTimer(float newHealthValue)
	{
		timerSpeed = (heartbeatTimer = ScaleHealthValue(newHealthValue));
	}

	private void OnHealthChanged(float newHealthValue)
	{
		if (newHealthValue <= healthThreshold && newHealthValue < lastValue)
		{
			EnterHurtState();
		}
		else if (isPlayerHurt && newHealthValue > healthThreshold)
		{
			ExitHurtState();
		}
		lastValue = newHealthValue;
		SetTimer(newHealthValue);
		S13Manager.SetAccessor("Player_Health", newHealthValue);
	}

	private void EnterHurtState()
	{
		if (!isPlayerHurt)
		{
			isPlayerHurt = true;
			enterHurtState.Execute();
		}
	}

	private void ExitHurtState()
	{
		if (isPlayerHurt)
		{
			isPlayerHurt = false;
			exitHurtState.Execute();
			S13Manager.SetAccessor("Player_Health", 1f);
		}
	}

	private float ScaleHealthValue(float newHealthValue)
	{
		float num = maxTimerSpeed - minTimerSpeed + minTimerSpeed;
		return newHealthValue / 10f * num;
	}
}
