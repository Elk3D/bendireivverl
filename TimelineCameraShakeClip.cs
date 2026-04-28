using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineCameraShakeClip : PlayableAsset, ITimelineClipAsset
{
	public float Duration;

	public float Strength;

	public int Vibrato;

	public float Randomness;

	public bool FadeOut;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<TimelineCameraShakeBehaviour> scriptPlayable = ScriptPlayable<TimelineCameraShakeBehaviour>.Create(graph);
		TimelineCameraShakeBehaviour behaviour = scriptPlayable.GetBehaviour();
		behaviour.Duration = Duration;
		behaviour.Strength = Strength;
		behaviour.Vibrato = Vibrato;
		behaviour.Randomness = Randomness;
		behaviour.FadeOut = FadeOut;
		return scriptPlayable;
	}
}
