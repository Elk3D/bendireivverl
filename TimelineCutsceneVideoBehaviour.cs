using UnityEngine.Playables;
using UnityEngine.Video;

public class TimelineCutsceneVideoBehaviour : PlayableBehaviour
{
	public VideoPlayer VideoPlayer;

	public override void PrepareFrame(Playable playable, FrameData info)
	{
		if (VideoPlayer != null)
		{
			if (!VideoPlayer.isPlaying)
			{
				VideoPlayer.Play();
			}
			if (VideoPlayer.isPlaying)
			{
				double time = playable.GetTime();
				VideoPlayer.time = time;
			}
		}
	}

	public override void OnBehaviourPause(Playable playable, FrameData info)
	{
		if (VideoPlayer.isPlaying)
		{
			VideoPlayer.Pause();
		}
	}
}
