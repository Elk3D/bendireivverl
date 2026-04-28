using UnityEngine;

public class SectionLightingController : JMonoBehaviour
{
	private Light[] m_Lights;

	public override void Start()
	{
		m_Lights = GetComponentsInChildren<Light>(includeInactive: true);
	}

	private void Update()
	{
		if (GameManager.Instance.GameCamera == null || m_Lights == null || m_Lights.Length == 0 || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		float farClipPlane = GameManager.Instance.GameCamera.Camera.farClipPlane;
		Transform transform = GameManager.Instance.GameCamera.transform;
		Light[] lights = m_Lights;
		foreach (Light light in lights)
		{
			float minDistance = Vector3.Distance(transform.position, light.transform.position);
			float num = light.range + 10f;
			float maxDistance = farClipPlane + num;
			Vector3 normalized = (light.transform.position - transform.position).normalized;
			if (Vector3.Angle(transform.forward, normalized) < 80f)
			{
				CheckDistance(minDistance, maxDistance, light);
			}
			else
			{
				CheckDistance(minDistance, num, light);
			}
		}
	}

	private void CheckDistance(float minDistance, float maxDistance, Light light)
	{
		if (minDistance > maxDistance)
		{
			DisableLight(light);
		}
		else
		{
			EnableLight(light);
		}
	}

	private void EnableLight(Light light)
	{
		if (!light.enabled)
		{
			light.enabled = true;
		}
	}

	private void DisableLight(Light light)
	{
		if (light.enabled)
		{
			light.enabled = false;
		}
	}

	protected override void OnDisposed()
	{
		m_Lights = null;
		base.OnDisposed();
	}
}
