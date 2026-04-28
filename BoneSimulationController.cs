using UnityEngine;

public class BoneSimulationController : JMonoBehaviour
{
	[SerializeField]
	private BoneFollow[] m_BoneSimulations;

	private bool m_IsActive = true;

	private void Update()
	{
		if (!(GameManager.Instance.GameCamera == null) && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			Transform transform = GameManager.Instance.GameCamera.transform;
			float minDistance = Vector3.Distance(transform.position, base.transform.position);
			Vector3 normalized = (base.transform.position - transform.position).normalized;
			if (Vector3.Angle(transform.forward, normalized) < 90f)
			{
				CheckDistance(minDistance, 60f);
			}
			else
			{
				CheckDistance(minDistance, 10f);
			}
		}
	}

	private void CheckDistance(float minDistance, float maxDistance)
	{
		if (minDistance > maxDistance)
		{
			Disable();
		}
		else
		{
			Enable();
		}
	}

	private void Enable()
	{
		if (!m_IsActive)
		{
			for (int i = 0; i < m_BoneSimulations.Length; i++)
			{
				m_BoneSimulations[i].Enable();
			}
			m_IsActive = true;
		}
	}

	private void Disable()
	{
		if (m_IsActive)
		{
			m_IsActive = false;
			for (int i = 0; i < m_BoneSimulations.Length; i++)
			{
				m_BoneSimulations[i].Disable();
			}
		}
	}

	protected override void OnDisposed()
	{
		if (m_BoneSimulations != null)
		{
			for (int i = 0; i < m_BoneSimulations.Length; i++)
			{
				m_BoneSimulations[i].ClearSimulations();
			}
		}
		base.OnDisposed();
	}
}
