using UnityEngine;

public class ActivationTrigger : EventTrigger
{
	[SerializeField]
	private GameObject[] m_ActivationObjects;

	protected override void OnInternalEnter(Collider col)
	{
		for (int i = 0; i < m_ActivationObjects.Length; i++)
		{
			GameObject obj = m_ActivationObjects[i];
			obj.SetActive(!obj.activeSelf);
		}
	}
}
