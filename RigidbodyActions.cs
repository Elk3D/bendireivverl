using UnityEngine;

public class RigidbodyActions : JMonoBehaviour
{
	[SerializeField]
	private Rigidbody m_Rigidbody;

	[SerializeField]
	private Transform m_ForceDirection;

	[SerializeField]
	private float m_ForceAmount;

	public void AddForce()
	{
		m_Rigidbody.AddForce(m_ForceDirection.forward * m_ForceAmount, ForceMode.Impulse);
	}
}
