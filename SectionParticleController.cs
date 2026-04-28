using UnityEngine;

public class SectionParticleController : JMonoBehaviour
{
	private ParticleSystem[] m_Particles;

	public override void Start()
	{
		m_Particles = GetComponentsInChildren<ParticleSystem>(includeInactive: true);
	}

	private void Update()
	{
		if (GameManager.Instance.GameCamera == null || m_Particles == null || m_Particles.Length == 0 || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		float farClipPlane = GameManager.Instance.GameCamera.Camera.farClipPlane;
		Transform transform = GameManager.Instance.GameCamera.transform;
		ParticleSystem[] particles = m_Particles;
		foreach (ParticleSystem particleSystem in particles)
		{
			float minDistance = Vector3.Distance(transform.position, particleSystem.transform.position);
			float num = 100f;
			float maxDistance = farClipPlane + num;
			Vector3 normalized = (particleSystem.transform.position - transform.position).normalized;
			if (Vector3.Angle(transform.forward, normalized) < 80f)
			{
				CheckDistance(minDistance, maxDistance, particleSystem);
			}
			else
			{
				CheckDistance(minDistance, num, particleSystem);
			}
		}
	}

	private void CheckDistance(float minDistance, float maxDistance, ParticleSystem particle)
	{
		if (minDistance > maxDistance)
		{
			DisableLight(particle);
		}
		else
		{
			EnableLight(particle);
		}
	}

	private void EnableLight(ParticleSystem particle)
	{
		if (particle.isPaused)
		{
			particle.Play();
		}
	}

	private void DisableLight(ParticleSystem particle)
	{
		if (particle.isPlaying && particle.main.playOnAwake)
		{
			particle.Pause();
		}
	}

	protected override void OnDisposed()
	{
		m_Particles = null;
		base.OnDisposed();
	}
}
