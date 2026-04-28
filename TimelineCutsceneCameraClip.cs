using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineCutsceneCameraClip : PlayableAsset, ITimelineClipAsset
{
	public TimelineCutsceneCameraType CutsceneType;

	public ExposedReference<Transform> Target;

	public ExposedReference<Transform> StartLocation;

	public ExposedReference<Transform> EndLocation;

	public bool UnlockWeapons = true;

	public bool EndCrouching;

	public bool IsReal;

	public bool IsSnap;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<TimelineCutsceneCameraBehaviour> scriptPlayable = ScriptPlayable<TimelineCutsceneCameraBehaviour>.Create(graph);
		TimelineCutsceneCameraBehaviour behaviour = scriptPlayable.GetBehaviour();
		behaviour.SetOwner(go);
		behaviour.Target = Target.Resolve(graph.GetResolver());
		behaviour.StartLocation = StartLocation.Resolve(graph.GetResolver());
		behaviour.EndLocation = EndLocation.Resolve(graph.GetResolver());
		behaviour.CutsceneType = CutsceneType;
		behaviour.UnlockWeapons = UnlockWeapons;
		behaviour.EndCrouching = EndCrouching;
		behaviour.IsReal = IsReal;
		behaviour.IsSnap = IsSnap;
		return scriptPlayable;
	}
}
