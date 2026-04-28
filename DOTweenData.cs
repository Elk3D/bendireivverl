using UnityEngine;

public class DOTweenData
{
	[SerializeField]
	private bool m_IsPlaying;

	[SerializeField]
	private float m_AnimationTime;

	public bool IsPlaying => m_IsPlaying;

	public float AnimationTime => m_AnimationTime;

	public void Set(float time)
	{
		m_IsPlaying = true;
		m_AnimationTime = time;
	}
}
