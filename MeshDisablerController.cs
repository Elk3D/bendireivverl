using UnityEngine;

public class MeshDisablerController : JMonoBehaviour
{
	private Renderer m_renderer;

	public void SetMeshEnabled(bool isEnabled)
	{
		if (m_renderer == null)
		{
			m_renderer = GetComponent<MeshRenderer>();
			if (m_renderer == null)
			{
				m_renderer = GetComponentInChildren<MeshRenderer>();
			}
			if (m_renderer != null)
			{
				m_renderer.enabled = isEnabled;
			}
		}
		else
		{
			m_renderer.enabled = isEnabled;
		}
	}

	public void SetSkinnedMeshEnabled(bool isEnabled)
	{
		if (m_renderer == null)
		{
			m_renderer = GetComponent<SkinnedMeshRenderer>();
			if (m_renderer == null)
			{
				m_renderer = GetComponentInChildren<SkinnedMeshRenderer>();
			}
			if (m_renderer != null)
			{
				m_renderer.enabled = isEnabled;
			}
		}
		else
		{
			m_renderer.enabled = isEnabled;
		}
	}
}
