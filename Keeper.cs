using UnityEngine;

public class Keeper : JMonoBehaviour
{
	[SerializeField]
	private Enemy m_Enemy;

	[SerializeField]
	private SkinnedMeshRenderer m_MeshRenderer;

	[SerializeField]
	private Material m_DefaultMaterial;

	[SerializeField]
	private Material m_EliteMaterial;

	private bool m_InCombat;

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (m_Enemy.InCombat)
		{
			if (!m_InCombat)
			{
				m_InCombat = true;
				m_MeshRenderer.material = m_EliteMaterial;
			}
		}
		else if (m_InCombat)
		{
			m_InCombat = false;
			m_MeshRenderer.material = m_DefaultMaterial;
		}
	}
}
