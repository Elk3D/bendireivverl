using UnityEngine;

public class LootDrawerConnector : JMonoBehaviour
{
	[SerializeField]
	private Transform m_ContentConnector;

	[SerializeField]
	private Transform m_Content;

	public void Initialize()
	{
		if (m_Content != null && m_ContentConnector != null)
		{
			m_Content.SetParent(m_ContentConnector);
		}
	}
}
