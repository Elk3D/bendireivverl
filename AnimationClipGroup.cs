using System;
using UnityEngine;

[Serializable]
public class AnimationClipGroup
{
	public string Name;

	public AnimationClip AnimationClip;

	public void Initialize()
	{
		AnimationClip.name = Name;
	}
}
