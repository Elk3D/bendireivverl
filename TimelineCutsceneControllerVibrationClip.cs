using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineCutsceneControllerVibrationClip : PlayableAsset, ITimelineClipAsset
{
	public bool IsEnd;

	public bool SlowPlayer;

	public bool LockPlayer;

	public bool SkipPlayIn;

	public bool DisableSkip;

	public float Vibration_Duration;

	public float Vibration_Strength;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<TimelineCutsceneControllerVibrationBehaviour> scriptPlayable = ScriptPlayable<TimelineCutsceneControllerVibrationBehaviour>.Create(graph);
		TimelineCutsceneControllerVibrationBehaviour behaviour = scriptPlayable.GetBehaviour();
		behaviour.SetOwner(go);
		behaviour.IsEnd = IsEnd;
		behaviour.SlowPlayer = SlowPlayer;
		behaviour.LockPlayer = LockPlayer;
		behaviour.SkipPlayIn = SkipPlayIn;
		behaviour.DisableSkip = DisableSkip;
		behaviour.m_Duration = Vibration_Duration;
		behaviour.m_Strength = Vibration_Strength;
		return scriptPlayable;
	}
}
