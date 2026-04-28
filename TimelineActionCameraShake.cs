using System;

[Serializable]
public class TimelineActionCameraShake
{
	public float duration;

	public float strength = 90f;

	public int vibrato = 10;

	public float randomness = 90f;

	public bool fadeOut = true;

	public void Action()
	{
		if (!(duration <= 0f))
		{
			CameraEffects.ShakeRotation(duration, strength, vibrato, randomness, fadeOut);
		}
	}
}
