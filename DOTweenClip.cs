using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class DOTweenClip : PlayableAsset, ITimelineClipAsset
{
	public ExposedReference<DOTweenAnimation> DOTweenAnimation;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<DOTweenBehaviour> scriptPlayable = ScriptPlayable<DOTweenBehaviour>.Create(graph);
		scriptPlayable.GetBehaviour().DOTweenAnimation = DOTweenAnimation.Resolve(graph.GetResolver());
		return scriptPlayable;
	}
}
