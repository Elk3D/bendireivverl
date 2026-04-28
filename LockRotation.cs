using UnityEngine;

public class LockRotation : JMonoBehaviour
{
	[SerializeField]
	private bool m_UseLocalRotation;

	[SerializeField]
	private bool m_LockX;

	[SerializeField]
	private bool m_LockY;

	[SerializeField]
	private bool m_LockZ;

	private void Update()
	{
		if (!base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			Vector3 vector = (m_UseLocalRotation ? base.transform.localEulerAngles : base.transform.eulerAngles);
			if (m_LockX)
			{
				vector.x = 0f;
			}
			if (m_LockY)
			{
				vector.y = 0f;
			}
			if (m_LockZ)
			{
				vector.z = 0f;
			}
			if (m_UseLocalRotation)
			{
				base.transform.localEulerAngles = vector;
			}
			else
			{
				base.transform.eulerAngles = vector;
			}
		}
	}
}
