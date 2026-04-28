using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineCutsceneWeaponClip : PlayableAsset, ITimelineClipAsset
{
	public ExposedReference<Transform> Target;

	public bool UseData;

	public bool UsePower;

	public int PowerCost;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<TimelineCutsceneWeaponBehaviour> scriptPlayable = ScriptPlayable<TimelineCutsceneWeaponBehaviour>.Create(graph);
		TimelineCutsceneWeaponBehaviour behaviour = scriptPlayable.GetBehaviour();
		behaviour.Target = Target.Resolve(graph.GetResolver());
		behaviour.UseData = UseData;
		behaviour.UsePower = UsePower;
		behaviour.PowerCost = PowerCost;
		return scriptPlayable;
	}
}
