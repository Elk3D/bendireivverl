using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraDepthOfFieldDistanceSetter : JMonoBehaviour
{
	private const float MAX_FOCAL_DISTANCE = 10f;

	[Header("DoF Settings")]
	[SerializeField]
	private LayerMask m_LayerMask;

	[SerializeField]
	private float m_Speed = 5f;

	[SerializeField]
	private float m_Radius = 0.015f;

	[SerializeField]
	private float m_MaxDistance = 5f;

	[SerializeField]
	private float m_CloseRadius = 1f;

	[SerializeField]
	private float m_CloseMaxDistance = 2f;

	private DepthOfField m_DOFSettings;

	private RaycastHit m_Hit;

	private float m_FocalDistance = 10f;

	private float m_ResetSpeed = 1f;

	private bool m_IsActive;

	public void Initialize(bool isActive, DepthOfField dof)
	{
		m_DOFSettings = dof;
		m_IsActive = isActive;
	}

	private void Update()
	{
		if (!m_IsActive || m_DOFSettings == null || !m_DOFSettings.enabled || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		float num = 10f;
		if (Physics.SphereCast(base.transform.position, m_Radius, base.transform.forward, out m_Hit, m_MaxDistance, m_LayerMask, QueryTriggerInteraction.Ignore))
		{
			num = m_Hit.distance;
			if (!m_DOFSettings.active)
			{
				m_DOFSettings.active = true;
			}
		}
		if (m_FocalDistance != num)
		{
			float num2 = ((num > m_MaxDistance && m_FocalDistance < num) ? m_ResetSpeed : m_Speed);
			m_FocalDistance = Mathf.Lerp(m_FocalDistance, num, num2 * Time.deltaTime);
			if (m_FocalDistance > 9.99f)
			{
				m_FocalDistance = 10f;
			}
			FloatParameter focusDistance = m_DOFSettings.focusDistance;
			focusDistance.value = m_FocalDistance;
			m_DOFSettings.focusDistance = focusDistance;
		}
		else if (!Physics.SphereCast(base.transform.position, m_CloseRadius, base.transform.forward, out m_Hit, m_CloseMaxDistance, m_LayerMask, QueryTriggerInteraction.Ignore))
		{
			if (m_DOFSettings.active)
			{
				m_DOFSettings.active = false;
			}
		}
		else if (!m_DOFSettings.active)
		{
			m_DOFSettings.active = true;
		}
	}

	protected override void OnDisposed()
	{
		m_DOFSettings = null;
		base.OnDisposed();
	}
}
