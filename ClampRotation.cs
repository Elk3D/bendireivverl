using UnityEngine;

public class ClampRotation : JMonoBehaviour
{
	private float m_ClampX = 20f;

	private float m_ClampZ = 20f;

	private Quaternion m_LastRotation;

	public override void Start()
	{
		m_LastRotation = base.transform.localRotation;
	}

	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.identity;
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		if (localEulerAngles.x > m_ClampX && localEulerAngles.x < 180f)
		{
			localEulerAngles.x = m_ClampX;
		}
		else if (localEulerAngles.x > m_ClampX && localEulerAngles.x < 360f - m_ClampX)
		{
			localEulerAngles.x = 360f - m_ClampX;
		}
		if (localEulerAngles.z > m_ClampZ && localEulerAngles.z < 180f)
		{
			localEulerAngles.z = m_ClampZ;
		}
		else if (localEulerAngles.z > m_ClampZ && localEulerAngles.z < 360f - m_ClampZ)
		{
			localEulerAngles.z = 360f - m_ClampZ;
		}
		base.transform.localRotation = Quaternion.Slerp(m_LastRotation, Quaternion.Euler(localEulerAngles), 1.5f * Time.deltaTime);
		m_LastRotation = base.transform.localRotation;
	}
}
