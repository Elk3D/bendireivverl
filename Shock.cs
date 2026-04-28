using UnityEngine;

public class Shock : JMonoBehaviour
{
	private ParticleSystem[] m_Particles;

	public override void Awake()
	{
		m_Particles = GetComponentsInChildren<ParticleSystem>(includeInactive: true);
	}

	public void Play()
	{
		if (m_Particles != null)
		{
			for (int i = 0; i < m_Particles.Length; i++)
			{
				m_Particles[i]?.Play();
			}
		}
	}

	protected override void OnDisposed()
	{
		m_Particles = null;
		base.OnDisposed();
	}
}
