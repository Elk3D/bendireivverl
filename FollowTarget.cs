using UnityEngine;

public class FollowTarget : JMonoBehaviour
{
	[SerializeField]
	private Transform m_Target;

	[SerializeField]
	private bool m_LockX;

	[SerializeField]
	private bool m_LockY;

	[SerializeField]
	private bool m_LockZ;

	[SerializeField]
	private bool m_IsFixed;

	private void Update()
	{
		if (!m_IsFixed && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			Follow();
		}
	}

	private void FixedUpdate()
	{
		if (m_IsFixed && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			Follow();
		}
	}

	private void Follow()
	{
		Vector3 position = m_Target.position;
		if (m_LockX)
		{
			position.x = base.transform.position.x;
		}
		if (m_LockY)
		{
			position.y = base.transform.position.y;
		}
		if (m_LockZ)
		{
			position.z = base.transform.position.z;
		}
		base.transform.position = position;
	}
}
