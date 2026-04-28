using UnityEngine;
using UnityEngine.Rendering;

public class MeshRendererActions : JMonoBehaviour
{
	[SerializeField]
	private Renderer[] m_Renderers;

	public void SetShadowsOnly()
	{
		for (int i = 0; i < m_Renderers.Length; i++)
		{
			m_Renderers[i].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
		}
	}
}
