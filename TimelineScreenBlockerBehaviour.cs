using UnityEngine;
using UnityEngine.Playables;

public class TimelineScreenBlockerBehaviour : PlayableBehaviour
{
	public float Duration;

	public bool Show = true;

	public bool IsWhite;

	private bool m_IsPlayed;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (Application.isPlaying && !m_IsPlayed)
		{
			m_IsPlayed = true;
			if (Show)
			{
				GameManager.Instance.ShowScreenBlocker(Duration, 0f, null, "BLOCKER", isIndependent: false, IsWhite);
			}
			else
			{
				GameManager.Instance.HideScreenBlocker(Duration, 0f, null, "BLOCKER", isIndependent: false, IsWhite);
			}
		}
	}
}
