using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineCutsceneBarsClip : PlayableAsset, ITimelineClipAsset
{
	[Header("IsEnd")]
	public bool IsEnd;

	public bool ShowCrosshair = true;

	[Header("Player")]
	public bool SlowPlayer;

	public bool LockPlayer;

	public bool SkipPlayIn;

	public bool DisableSkip;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<TimelineCutsceneBarsBehaviour> scriptPlayable = ScriptPlayable<TimelineCutsceneBarsBehaviour>.Create(graph);
		TimelineCutsceneBarsBehaviour behaviour = scriptPlayable.GetBehaviour();
		behaviour.SetOwner(go);
		behaviour.IsEnd = IsEnd;
		behaviour.ShowCrosshair = ShowCrosshair;
		behaviour.SlowPlayer = SlowPlayer;
		behaviour.LockPlayer = LockPlayer;
		behaviour.SkipPlayIn = SkipPlayIn;
		behaviour.DisableSkip = DisableSkip;
		return scriptPlayable;
	}
}
