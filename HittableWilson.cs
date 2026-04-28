using UnityEngine;

public class HittableWilson : Hittable
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
		m_Particles.SetActive(active);
		m_Collider.enabled = active;
	}

	protected override bool InternalHit(RaycastHit hit)
	{
		GameManager.Instance.PoolingManager.GetFromPool("Effects/Effects_Hit_Ink_Color").transform.transform.position = hit.point;
		object[] renderers = m_Renderers;
		ShaderEffects.Fade("_PostHitGlow", 2f, 0.3f, 0.75f, renderers);
		return true;
	}
}
