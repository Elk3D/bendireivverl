using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class LightClip : PlayableAsset, ITimelineClipAsset
{
	public ExposedReference<GameObject> Target;

	public float FadeValue;

	public float FadeDuration;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<LightBehaviour> scriptPlayable = ScriptPlayable<LightBehaviour>.Create(graph);
		LightBehaviour behaviour = scriptPlayable.GetBehaviour();
		behaviour.Target = Target.Resolve(graph.GetResolver());
		behaviour.FadeValue = FadeValue;
		behaviour.FadeDuration = FadeDuration;
		return scriptPlayable;
	}
}
