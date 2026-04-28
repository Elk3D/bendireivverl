using S13Audio;
using UnityEngine;

public class S13HandlerDynamic : S13HandlerPrimitive<S13ObjectDynamic, S13HandlerDynamic>
{
	private AudioClip nextClip;

	public void SetNextClip(AudioClip clip)
	{
		nextClip = clip;
	}

	protected override void PlayHandler(Transform parent)
	{
		base.PlayHandler(parent);
		VoiceClip(nextClip, base.targetObject.loop, parent);
	}

	protected override void StopHandler(bool ignoreFades)
	{
		base.StopHandler(ignoreFades);
	}
}
