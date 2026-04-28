using UnityEngine;

public class GenericPrefabProxy : JMonoBehaviour
{
	[SerializeField]
	private GameObject m_Prefab;

	[SerializeField]
	private Transform m_Parent;

	public override void Awake()
	{
		if (!(m_Prefab == null))
		{
			Object.Instantiate(m_Prefab, (m_Parent != null) ? m_Parent : base.transform).name = m_Prefab.name;
			Object.Destroy(this);
		}
	}
}
