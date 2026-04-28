using UnityEngine;

public class InteractableActivator : Interactable
{
	[SerializeField]
	private GameObject[] m_ActivationGameObjects;

	[SerializeField]
	private GameObject[] m_DeactivateGameObjects;

	protected override void OnInternalInteract(Vector3 origin, RaycastHit hit, object sender = null)
	{
		if (m_ActivationGameObjects != null)
		{
			for (int i = 0; i < m_ActivationGameObjects.Length; i++)
			{
				GameObject obj = m_ActivationGameObjects[i];
				obj.SetActive(!obj.activeSelf);
			}
		}
		if (m_DeactivateGameObjects != null)
		{
			for (int j = 0; j < m_DeactivateGameObjects.Length; j++)
			{
				GameObject obj2 = m_DeactivateGameObjects[j];
				obj2.SetActive(!obj2.activeSelf);
			}
		}
	}
}
