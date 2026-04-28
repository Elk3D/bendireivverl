using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class InteractableAnimationClip : PlayableAsset, ITimelineClipAsset
{
	public ExposedReference<InteractableAnimation> InteractableAnimation;

	public bool OnEnter;

	public bool OnExit;

	public bool DoLock;

	public bool DoUnlock;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<InteractableAnimationBehaviour> scriptPlayable = ScriptPlayable<InteractableAnimationBehaviour>.Create(graph);
		InteractableAnimationBehaviour behaviour = scriptPlayable.GetBehaviour();
		behaviour.InteractableAnimation = InteractableAnimation.Resolve(graph.GetResolver());
		behaviour.OnEnter = OnEnter;
		behaviour.OnExit = OnExit;
		behaviour.DoLock = DoLock;
		behaviour.DoUnlock = DoUnlock;
		return scriptPlayable;
	}
}
