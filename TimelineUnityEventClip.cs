using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineUnityEventClip : PlayableAsset, ITimelineClipAsset
{
	public ExposedReference<TimelineUnityEvent> UnityEvent;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<TimelineUnityEventBehaviour> scriptPlayable = ScriptPlayable<TimelineUnityEventBehaviour>.Create(graph);
		scriptPlayable.GetBehaviour().UnityEvent = UnityEvent.Resolve(graph.GetResolver());
		return scriptPlayable;
	}
}
