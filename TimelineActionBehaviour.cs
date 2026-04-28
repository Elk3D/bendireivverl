using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineActionBehaviour : PlayableBehaviour
{
	public List<TimelineActions> Actions;

	private bool m_IsPlayed;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (Application.isPlaying && !m_IsPlayed)
		{
			m_IsPlayed = true;
			for (int i = 0; i < Actions.Count; i++)
			{
				Actions[i].Action();
			}
		}
	}
}
