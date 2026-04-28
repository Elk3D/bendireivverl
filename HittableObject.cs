using UnityEngine;

public class HittableObject : Hittable
{
	[SerializeField]
	private GameObject m_HitEffectPrefab;

	[SerializeField]
	private bool m_DisposeOnHit;

	private GameObject m_HitEffect;

	private Rigidbody m_Rigidbody;

	public override void Awake()
	{
		base.Awake();
		if (m_Rigidbody == null)
		{
			m_Rigidbody = GetComponent<Rigidbody>();
		}
	}

	protected override bool InternalHit(RaycastHit hit)
	{
		if ((bool)m_HitEffectPrefab)
		{
			m_HitEffect = Object.Instantiate(m_HitEffectPrefab);
			m_HitEffect.transform.position = hit.point;
			Quaternion rotation = Quaternion.LookRotation(hit.point - base.transform.position);
			m_HitEffect.transform.rotation = rotation;
			m_HitEffect.transform.SetParent(base.transform);
		}
		if (m_DisposeOnHit)
		{
			Dispose();
		}
		return true;
	}

	protected override void OnDisposed()
	{
		m_HitEffect = null;
		m_Rigidbody = null;
		base.OnDisposed();
	}
}
