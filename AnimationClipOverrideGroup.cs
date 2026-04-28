using System;

[Serializable]
public class AnimationClipOverrideGroup
{
	public string Name;

	public AnimationClipGroup[] AnimationClipGroups;

	public void Initialize()
	{
		for (int i = 0; i < AnimationClipGroups.Length; i++)
		{
			AnimationClipGroups[i].Initialize();
		}
	}
}
