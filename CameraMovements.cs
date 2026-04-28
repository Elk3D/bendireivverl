using System;
using UnityEngine;

[Serializable]
public class CameraMovements : JMonoBehaviour
{
	[SerializeField]
	private bool m_IsActive = true;

	[SerializeField]
	private float m_SwaySpeed = 0.6f;

	[SerializeField]
	private float m_BaseSwayAmount = 1.5f;

	[SerializeField]
	private float m_TrackingSwayAmount = 1.5f;

	[SerializeField]
	private float m_TrackingBias;

	[SerializeField]
	private float m_FollowSpeed = 1f;

	private Transform m_Target;

	private Quaternion m_OriginalRotation;

	private Vector3 m_FollowVelocity;

	private Vector3 m_FollowAngles;

	private Vector2 m_RotationRange;

	public bool IsActive => m_IsActive;

	public override void Awake()
	{
		base.Awake();
		m_OriginalRotation = base.transform.localRotation;
		m_Target = new GameObject("Forward Camera Target").transform;
		m_Target.SetParent(base.transform);
		m_Target.localPosition = Vector3.forward;
		m_Target.localEulerAngles = Vector3.zero;
		m_IsActive = GameManager.Instance.PlayerSettings.ViewSwaying;
	}

	private void Update()
	{
		if (m_IsActive && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			Sway();
		}
	}

	public void SetActive(bool active)
	{
		m_IsActive = active;
		if (!m_IsActive)
		{
			base.transform.localEulerAngles = Vector3.zero;
		}
	}

	public void Sway()
	{
		base.transform.localRotation = m_OriginalRotation;
		Vector3 vector = base.transform.parent.InverseTransformPoint(m_Target.position);
		float value = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
		value = Mathf.Clamp(value, -10.5f, 10.5f);
		base.transform.localRotation = m_OriginalRotation * Quaternion.Euler(0f, value, 0f);
		vector = base.transform.parent.InverseTransformPoint(m_Target.position);
		float value2 = Mathf.Atan2(vector.y, vector.z) * 57.29578f;
		value2 = Mathf.Clamp(value2, -10.5f, 10.5f);
		m_FollowAngles = Vector3.SmoothDamp(target: new Vector3(m_FollowAngles.x + Mathf.DeltaAngle(m_FollowAngles.x, value2), m_FollowAngles.y + Mathf.DeltaAngle(m_FollowAngles.y, value)), current: m_FollowAngles, currentVelocity: ref m_FollowVelocity, smoothTime: m_FollowSpeed);
		base.transform.localRotation = m_OriginalRotation * Quaternion.Euler(0f - m_FollowAngles.x, m_FollowAngles.y, 0f);
		float num = Mathf.PerlinNoise(0f, Time.time * m_SwaySpeed) - 0.5f;
		float num2 = Mathf.PerlinNoise(0f, Time.time * m_SwaySpeed + 100f) - 0.5f;
		num *= m_BaseSwayAmount;
		float num3 = num2 * m_BaseSwayAmount;
		float num4 = Mathf.PerlinNoise(0f, Time.time * m_SwaySpeed) - 0.5f + m_TrackingBias;
		float num5 = Mathf.PerlinNoise(0f, Time.time * m_SwaySpeed + 100f) - 0.5f + m_TrackingBias;
		num4 *= (0f - m_TrackingSwayAmount) * m_FollowVelocity.x;
		num5 *= m_TrackingSwayAmount * m_FollowVelocity.y;
		float xAngle = num + num4;
		float num6 = num3 + num5;
		base.transform.Rotate(xAngle, num6, (0f - num6) * 0.5f);
	}

	protected override void OnDisposed()
	{
		m_Target = null;
		base.OnDisposed();
	}
}
