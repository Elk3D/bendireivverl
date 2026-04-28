using UnityEngine;
using UnityEngine.Playables;

public class TimelineUnityEventBehaviour : PlayableBehaviour
{
	public TimelineUnityEvent UnityEvent;

	private bool m_IsPlayed;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (Application.isPlaying && !m_IsPlayed)
		{
			m_IsPlayed = true;
			if (UnityEvent != null)
			{
				UnityEvent.Action();
			}
		}
	}
}
