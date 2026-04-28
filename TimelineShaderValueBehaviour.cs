using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineShaderValueBehaviour : PlayableBehaviour
{
	public GameObject Target;

	public int MaterialIndex;

	public string PropertyName;

	public float Value;

	public float Duration = 1f;

	private bool m_IsPlayed;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (Application.isPlaying && !m_IsPlayed)
		{
			m_IsPlayed = true;
			Target.GetComponent<MeshRenderer>().sharedMaterials[MaterialIndex].DOFloat(Value, PropertyName, Duration).SetEase(Ease.Linear);
		}
	}
}
