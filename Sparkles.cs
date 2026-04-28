using UnityEngine;

public class Sparkles : JMonoBehaviour
{
	[SerializeField]
	private ParticleSystem m_ParticleSystem;

	public ParticleSystem ParticleSystem => m_ParticleSystem;

	public void Play()
	{
		m_ParticleSystem.Play();
	}

	public void Stop()
	{
		m_ParticleSystem.Stop();
	}
}
