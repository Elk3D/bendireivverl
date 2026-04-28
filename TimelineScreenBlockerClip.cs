using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineScreenBlockerClip : PlayableAsset, ITimelineClipAsset
{
	public float Duration;

	public bool Show = true;

	public bool IsWhite;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<TimelineScreenBlockerBehaviour> scriptPlayable = ScriptPlayable<TimelineScreenBlockerBehaviour>.Create(graph);
		TimelineScreenBlockerBehaviour behaviour = scriptPlayable.GetBehaviour();
		behaviour.Duration = Duration;
		behaviour.Show = Show;
		behaviour.IsWhite = IsWhite;
		return scriptPlayable;
	}
}
