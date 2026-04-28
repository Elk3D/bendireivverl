using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessVolumes : JMonoBehaviour
{
	[SerializeField]
	private PostProcessVolume m_BasePostProcessVolume;

	[SerializeField]
	private PostProcessVolume m_ColorPostProcessVolume;

	public PostProcessVolume BasePostProcessVolume => m_BasePostProcessVolume;

	public PostProcessVolume ColorPostProcessVolume => m_ColorPostProcessVolume;

	public PostProcessProfile BasePostProcess => m_BasePostProcessVolume.profile;

	public PostProcessProfile ColorPostProcess => m_ColorPostProcessVolume.profile;
}
