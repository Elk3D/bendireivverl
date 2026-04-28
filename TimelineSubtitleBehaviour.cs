using UnityEngine;
using UnityEngine.Playables;

public class TimelineSubtitleBehaviour : PlayableBehaviour
{
	public string Subtitles;

	public int Index;

	private bool m_IsPlayed;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (!Application.isPlaying || m_IsPlayed)
		{
			return;
		}
		m_IsPlayed = true;
		if (GameManager.Instance.PlayerSettings.Subtitles)
		{
			string[] array = TextUtility.GetKey(Subtitles).Split('{');
			if (Index < array.Length)
			{
				GameManager.Instance.ShowSubtitles(array[Index], (float)playable.GetDuration(), isTrimmed: true);
			}
		}
	}
}
