using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;

[Serializable]
public class TimelineCutsceneVideoClip : PlayableAsset, ITimelineClipAsset
{
	public ExposedReference<VideoPlayer> VideoPlayer;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<TimelineCutsceneVideoBehaviour> scriptPlayable = ScriptPlayable<TimelineCutsceneVideoBehaviour>.Create(graph);
		scriptPlayable.GetBehaviour().VideoPlayer = VideoPlayer.Resolve(graph.GetResolver());
		return scriptPlayable;
	}
}
