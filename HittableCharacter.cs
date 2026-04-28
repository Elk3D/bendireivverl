using UnityEngine;

public class HittableCharacter : Hittable
{
	[SerializeField]
	private Collider m_Collider;

	[SerializeField]
	private Renderer[] m_Renderers;

	[SerializeField]
	private GameObject m_Particles;

	public override void Awake()
	{
		SetActive(active: false);
	}

	public void SetActive(bool active)
	{
		if (m_Particles != null)
		{
			m_Particles.SetActive(active);
		}
		if (m_Collider != null)
		{
			m_Collider.enabled = active;
		}
	}

	protected override bool InternalHit(RaycastHit hit)
	{
		GameManager.Instance.PoolingManager.GetFromPool("Effects/Effects_Hit_Smoke").transform.transform.position = hit.point;
		object[] renderers = m_Renderers;
		ShaderEffects.Fade("_PostHitGlow", 2f, 0.3f, 0.75f, renderers);
		return true;
	}
}
