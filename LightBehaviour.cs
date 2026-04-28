using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class LightBehaviour : PlayableBehaviour
{
	public GameObject Target;

	public float FadeValue;

	public float FadeDuration;

	private bool m_IsPlayed;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (Application.isPlaying && !(Target == null) && !m_IsPlayed)
		{
			m_IsPlayed = true;
			Light component = Target.GetComponent<Light>();
			if (component != null)
			{
				component.DOKill();
				component.DOIntensity(FadeValue, FadeDuration).SetEase(Ease.Linear);
			}
		}
	}
}
