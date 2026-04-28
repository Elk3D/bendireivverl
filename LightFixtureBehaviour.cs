using UnityEngine;
using UnityEngine.Playables;

public class LightFixtureBehaviour : PlayableBehaviour
{
	public GameObject Target;

	public bool On;

	private LightFixture m_LightFixture;

	private bool m_IsPlayed;

	private float m_OriginalMaterial;

	public override void OnGraphStart(Playable playable)
	{
		if (!(Target == null))
		{
			m_LightFixture = Target.GetComponent<LightFixture>();
			_ = m_LightFixture == null;
		}
	}

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (!(m_LightFixture == null) && Application.isPlaying && !m_IsPlayed)
		{
			m_IsPlayed = true;
			if (On)
			{
				m_LightFixture.SetEmission(1f);
			}
			else
			{
				m_LightFixture.SetEmission(0f);
			}
		}
	}

	public override void PrepareFrame(Playable playable, FrameData info)
	{
	}

	public override void OnGraphStop(Playable playable)
	{
		_ = m_LightFixture == null;
	}
}
