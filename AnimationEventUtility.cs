using System;
using UnityEngine;

public static class AnimationEventUtility
{
	public static void AddAnimationEvent(ref Animator animator, string clipName, string functionName, int frame, string stringParameter)
	{
		SetupNewEvent(GetAnimationClip(ref animator, clipName), functionName, (float)frame / 30f, stringParameter);
	}

	public static void AddAnimationEvent(ref Animator animator, string clipName, string functionName, float time, string stringParameter)
	{
		SetupNewEvent(GetAnimationClip(ref animator, clipName), functionName, time, stringParameter);
	}

	private static AnimationClip GetAnimationClip(ref Animator animator, string clipName)
	{
		AnimationClip result = null;
		for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
		{
			if (animator.runtimeAnimatorController.animationClips[i].name == clipName)
			{
				result = animator.runtimeAnimatorController.animationClips[i];
				break;
			}
		}
		return result;
	}

	private static void SetupNewEvent(AnimationClip clip, string functionName, float time, string stringParameter)
	{
		if (clip != null)
		{
			time = (float)Math.Round(time, 3);
			if (!ContainsEvent(clip, functionName, time, stringParameter))
			{
				AnimationEvent evt = AddEvent(functionName, time, stringParameter);
				clip.AddEvent(evt);
			}
		}
	}

	private static bool ContainsEvent(AnimationClip clip, string functionName, float time, string stringParameter)
	{
		bool result = false;
		AnimationEvent[] events = clip.events;
		foreach (AnimationEvent animationEvent in events)
		{
			if (animationEvent.functionName == functionName && animationEvent.time == time && animationEvent.stringParameter == stringParameter)
			{
				result = true;
			}
		}
		return result;
	}

	private static AnimationEvent AddEvent(string functionName, float time, string stringParameter)
	{
		return new AnimationEvent
		{
			functionName = functionName,
			stringParameter = stringParameter,
			time = time
		};
	}
}
