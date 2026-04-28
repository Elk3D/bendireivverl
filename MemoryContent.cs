using UnityEngine;

public class MemoryContent : JMonoBehaviour
{
	[SerializeField]
	private MemoryDirector m_Director;

	[SerializeField]
	private CutsceneActivator m_Activator;

	[SerializeField]
	private GameObject m_InitializeContent;

	[SerializeField]
	private Transform m_StartLocation;

	public MemoryDirector Director => m_Director;

	public CutsceneActivator Activator => m_Activator;

	public GameObject InitializeContent => m_InitializeContent;

	public Transform StartLocation => m_StartLocation;

	public void Initialize()
	{
		if (m_InitializeContent != null)
		{
			m_InitializeContent.SetActive(value: true);
		}
	}
}
