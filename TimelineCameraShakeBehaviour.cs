using UnityEngine;
using UnityEngine.Playables;

public class TimelineCameraShakeBehaviour : PlayableBehaviour
{
	public float Duration;

	public float Strength = 90f;

	public int Vibrato = 10;

	public float Randomness = 90f;

	public bool FadeOut = true;

	private bool m_IsPlayed;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (Application.isPlaying && !m_IsPlayed)
		{
			m_IsPlayed = true;
			if (!(Duration <= 0f))
			{
				CameraEffects.ShakeRotation(Duration, Strength, Vibrato, Randomness, FadeOut);
			}
		}
	}
}
