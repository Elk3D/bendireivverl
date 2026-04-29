using System;
using UnityEngine;

[Serializable]
public class FPAnimationClipGroup
{
    public string Name;
    public AnimationClip AnimationClip;

    public void Initialize() => AnimationClip.name = Name;
}
