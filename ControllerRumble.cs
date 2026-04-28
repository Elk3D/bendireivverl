using DG.Tweening;
using InControl;
using UnityEngine;

public class ControllerRumble
{
	public enum RUMBLE_PRESETS
	{
		UI_Blip,
		Strong_Jolt,
		Soft_Drag,
		Long_Pop,
		Note_Pickup,
		Desk_Drawers,
		Food_Pickup,
		Inv_Pickup,
		Cart_Drag,
		NONE
	}

	private const float PS_RUMBLE_INTENSITY_MODIFIER = 1.15f;

	private const float XBOX_RUMBLE_INTENSITY_MODIFIER = 1f;

	private const float SWITCH_RUMBLE_INTENSITY_MODIFIER = 1f;

	public static readonly Vector2[] rumblePresets = new Vector2[9]
	{
		new Vector2(0.1f, 1f),
		new Vector2(0.3f, 3.5f),
		new Vector2(1f, 0.7f),
		new Vector2(0.5f, 2f),
		new Vector2(0.1f, 0.5f),
		new Vector2(0.6f, 0.6f),
		new Vector2(0.35f, 0.5f),
		new Vector2(0.45f, 0.3f),
		new Vector2(1.2f, 0.5f)
	};

	private Tweener m_ControllerVibrationTweener;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetVibration(float duration, float strength = 10f, int vibrato = 10, float randomness = 90f, bool fadeOut = true, bool isIndependentUpdate = false)
	{
		float num = strength;
		switch (Application.platform)
		{
		case RuntimePlatform.PS4:
		case RuntimePlatform.PS5:
			num *= 1.15f;
			break;
		case RuntimePlatform.GameCoreXboxSeries:
		case RuntimePlatform.GameCoreXboxOne:
			num *= 1f;
			break;
		case RuntimePlatform.Switch:
			num *= 1f;
			break;
		}
		num += 0.5f;
		num *= GameManager.Instance.PlayerSettings.Rumble;
		if (duration == 0f || strength == 0f)
		{
			return;
		}
		m_ControllerVibrationTweener.Kill();
		if (!GameManager.Instance.HasController)
		{
			return;
		}
		float vibration = num;
		m_ControllerVibrationTweener = DOTween.To(() => vibration, delegate(float x)
		{
			vibration = x;
		}, 0f, duration).SetUpdate(UpdateType.Normal, isIndependentUpdate).OnUpdate(delegate
		{
			if (GameManager.Instance.HasController)
			{
				InputManager.ActiveDevice.Vibrate(vibration * 0.5f);
			}
		})
			.OnComplete(delegate
			{
				InputManager.ActiveDevice.Vibrate(0f);
			});
	}

	public void SetVibration(Vector2 DurationAndStrength, bool isIndependentUpdate = false)
	{
		DurationAndStrength.y *= GameManager.Instance.PlayerSettings.Rumble;
		if (DurationAndStrength.y == 0f)
		{
			return;
		}
		m_ControllerVibrationTweener.Kill();
		if (!GameManager.Instance.HasController)
		{
			return;
		}
		float vibration = DurationAndStrength.y + 0.5f;
		m_ControllerVibrationTweener = DOTween.To(() => vibration, delegate(float x)
		{
			vibration = x;
		}, 0f, DurationAndStrength.x).SetUpdate(UpdateType.Normal, isIndependentUpdate).OnUpdate(delegate
		{
			if (GameManager.Instance.HasController)
			{
				InputManager.ActiveDevice.Vibrate(vibration * 0.5f);
			}
		})
			.OnComplete(delegate
			{
				InputManager.ActiveDevice.Vibrate(0f);
			});
	}

	public void RumbleEvent(AnimationEvent animationEvent)
	{
		float strength = float.Parse(animationEvent.stringParameter) * GameManager.Instance.PlayerSettings.Rumble;
		GameManager.Instance.TriggerRumble(animationEvent.floatParameter, strength);
	}
}
