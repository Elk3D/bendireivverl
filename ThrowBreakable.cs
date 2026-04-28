using UnityEngine;

public class ThrowBreakable : MonoBehaviour
{
	[SerializeField]
	private GameObject m_Prefab;

	[SerializeField]
	private ActiveSetter m_ActiveSetter;

	private void OnTriggerEnter(Collider other)
	{
		ThrowObject component = other.GetComponent<ThrowObject>();
		if (component == null)
		{
			component = other.transform.root.GetComponent<ThrowObject>();
		}
		if (component != null)
		{
			m_Prefab.SetActive(value: true);
			m_Prefab.transform.SetParent(null);
			if (m_ActiveSetter != null)
			{
				m_ActiveSetter.SetActive(active: true);
			}
			Object.Destroy(base.gameObject);
		}
	}
}
