using UnityEngine;
using UnityEngine.Playables;

public class TimelineCutsceneControllerVibrationBehaviour : PlayableBehaviour
{
	public bool IsEnd;

	public bool SlowPlayer;

	public bool LockPlayer;

	public bool SkipPlayIn;

	public bool DisableSkip;

	private bool m_IsPlayed;

	private GameObject m_Owner;

	private CutsceneDirector m_CutsceneDirector;

	public float m_Duration;

	public float m_Strength;

	public void SetOwner(GameObject owner)
	{
		m_Owner = owner;
	}

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (Application.isPlaying && !m_IsPlayed)
		{
			m_IsPlayed = true;
			if (!IsEnd)
			{
				Begin();
			}
			else
			{
				End();
			}
		}
	}

	private void Begin()
	{
		GameManager.Instance.TriggerRumble(m_Duration, m_Strength);
	}

	private void End()
	{
	}
}
