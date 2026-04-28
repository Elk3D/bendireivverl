using System;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class TransformTracker : JComponent
{
	[Serializable]
	public class ClampRotations
	{
		public bool IsActive;

		public float LowClamp = 230f;

		public float HighClamp = 290f;
	}

	[SerializeField]
	private bool m_IsActive = true;

	[Header("Transform Options")]
	[SerializeField]
	private Transform m_Target;

	[SerializeField]
	private Vector3 m_ForwardOffset = new Vector3(0f, 270f, 270f);

	[SerializeField]
	private float m_Speed = 15f;

	[Header("Clamp Options")]
	[SerializeField]
	private ClampRotations m_ClampRotationX;

	[SerializeField]
	private ClampRotations m_ClampRotationY;

	[SerializeField]
	private ClampRotations m_ClampRotationZ;

	private Quaternion m_LastLookRotation;

	private Vector3 m_ClampRotation;

	public override void Awake()
	{
		m_LastLookRotation = base.transform.rotation;
	}

	private void LateUpdate()
	{
		if (m_IsActive && !(m_Target == null))
		{
			Quaternion quaternion = base.transform.rotation;
			if (m_IsActive)
			{
				quaternion = Quaternion.LookRotation(m_Target.forward) * Quaternion.Euler(m_ForwardOffset);
			}
			m_ClampRotation = quaternion.eulerAngles;
			SetClamp(ref m_ClampRotation.x, ref m_ClampRotationX);
			SetClamp(ref m_ClampRotation.y, ref m_ClampRotationY);
			SetClamp(ref m_ClampRotation.z, ref m_ClampRotationZ);
			if (m_IsActive)
			{
				base.transform.rotation = Quaternion.Slerp(m_LastLookRotation, Quaternion.Euler(m_ClampRotation), m_Speed * Time.deltaTime);
			}
			m_LastLookRotation = base.transform.rotation;
		}
	}

	private void SetClamp(ref float rotation, ref ClampRotations axis)
	{
		if (axis.IsActive)
		{
			if (rotation < axis.LowClamp)
			{
				rotation = axis.LowClamp;
			}
			else if (rotation > axis.HighClamp)
			{
				rotation = axis.HighClamp;
			}
		}
	}

	public void SetActive(bool active)
	{
		m_LastLookRotation = base.transform.rotation;
		m_IsActive = active;
	}

	public void SetTarget(Transform target)
	{
		m_Target = target;
	}
}
