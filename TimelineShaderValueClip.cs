using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineShaderValueClip : PlayableAsset, ITimelineClipAsset
{
	public ExposedReference<GameObject> Target;

	public int MaterialIndex;

	public string PropertyName;

	public float Value;

	public float Duration = 1f;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<TimelineShaderValueBehaviour> scriptPlayable = ScriptPlayable<TimelineShaderValueBehaviour>.Create(graph);
		TimelineShaderValueBehaviour behaviour = scriptPlayable.GetBehaviour();
		behaviour.Target = Target.Resolve(graph.GetResolver());
		behaviour.MaterialIndex = MaterialIndex;
		behaviour.PropertyName = PropertyName;
		behaviour.Value = Value;
		behaviour.Duration = Duration;
		return scriptPlayable;
	}
}
