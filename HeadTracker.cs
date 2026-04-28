using UnityEngine;

public class HeadTracker : JComponent
{
	[SerializeField]
	private bool m_IsActive = true;

	[Header("Transform Options")]
	[SerializeField]
	private Transform m_ForwardDirection;

	[SerializeField]
	private Vector3 m_ForwardOffset = new Vector3(0f, 270f, 270f);

	[SerializeField]
	private float m_Speed = 5f;

	[SerializeField]
	private float m_Distance = 8f;

	[SerializeField]
	private float m_Angle = 45f;

	[Header("Other Options")]
	[SerializeField]
	private bool m_IsInstantStart;

	private Transform m_Target;

	private Quaternion m_CurrentRotation;

	private Quaternion m_LastLookRotation;

	private bool m_InRange;

	private bool m_IgnoreTracking;

	public override void OnEnable()
	{
		m_LastLookRotation = base.transform.rotation;
		if (m_ForwardDirection == null)
		{
			m_ForwardDirection = base.transform;
		}
		if (m_IsActive && m_IsInstantStart)
		{
			InstantStart();
		}
	}

	private void InstantStart()
	{
		if (!(GameManager.Instance.Player == null))
		{
			if (m_Target == null)
			{
				m_Target = GameManager.Instance.Player.HeadContainer;
			}
			if (!(m_Target == null))
			{
				m_CurrentRotation = base.transform.rotation;
				m_InRange = Vector3.Distance(base.transform.position, m_Target.position) < m_Distance;
				Quaternion rotation = (CheckBasicFOV(m_ForwardDirection, m_Target, m_Angle) ? (Quaternion.LookRotation(m_Target.position - base.transform.position) * Quaternion.Euler(m_ForwardOffset)) : m_CurrentRotation);
				base.transform.rotation = rotation;
				m_LastLookRotation = base.transform.rotation;
			}
		}
	}

	private void LateUpdate()
	{
		if (m_IsActive && !(GameManager.Instance.Player == null))
		{
			if (m_Target == null)
			{
				m_Target = GameManager.Instance.Player.HeadContainer;
			}
			if (!(m_Target == null))
			{
				m_CurrentRotation = base.transform.rotation;
				m_InRange = Vector3.Distance(base.transform.position, m_Target.position) < m_Distance;
				bool flag = CheckBasicFOV(m_ForwardDirection, m_Target, m_Angle);
				Quaternion b = (flag ? (Quaternion.LookRotation(m_Target.position - base.transform.position) * Quaternion.Euler(m_ForwardOffset)) : m_CurrentRotation);
				base.transform.rotation = Quaternion.Slerp(m_LastLookRotation, b, (flag ? m_Speed : (m_Speed * 2f)) * Time.deltaTime);
				m_LastLookRotation = base.transform.rotation;
			}
		}
	}

	public bool CheckBasicFOV(Transform from, Transform to, float angle)
	{
		if (m_InRange && !(to == null) && !(from == null) && !m_IgnoreTracking)
		{
			return Vector3.Angle(from.forward, to.position - from.position) < angle;
		}
		return false;
	}

	public void SetActive(bool active)
	{
		m_LastLookRotation = base.transform.rotation;
		m_IsActive = active;
	}

	public void SetIgnore(bool ignore)
	{
		m_IgnoreTracking = ignore;
	}

	public void SetTarget(Transform target)
	{
		m_Target = target;
	}

	public void SetForwardDirection(Transform forwardDirection)
	{
		m_ForwardDirection = forwardDirection;
	}

	public void SetOffset(Vector3 offset)
	{
		m_ForwardOffset = offset;
	}

	public void SetSpeed(float speed)
	{
		m_Speed = speed;
	}

	public void SetDistance(float distance)
	{
		m_Distance = distance;
	}

	public void SetAngle(float angle)
	{
		m_Angle = angle;
	}

	protected override void OnDisposed()
	{
		m_Target = null;
		base.OnDisposed();
	}
}
