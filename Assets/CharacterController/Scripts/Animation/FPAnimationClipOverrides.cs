using System.Collections.Generic;
using UnityEngine;

public class FPAnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public FPAnimationClipOverrides(int capacity) : base(capacity) { }

    public AnimationClip this[string name]
    {
        get => Find(x => x.Key.name.Equals(name)).Value;
        set
        {
            int i = FindIndex(x => x.Key.name.Equals(name));
            if (i != -1) this[i] = new KeyValuePair<AnimationClip, AnimationClip>(this[i].Key, value);
        }
    }
}
