using UnityEngine;

public class LightFlickerConnector : JMonoBehaviour
{
	[Header("Primary Light Flicker Controls")]
	[SerializeField]
	private LightFlicker m_PrimaryLight;

	[Header("Additional Light Flickers (Copy Primary)")]
	[SerializeField]
	private LightFlicker[] m_AdditionalLights;

	public override void Start()
	{
		if (m_AdditionalLights != null)
		{
			for (int i = 0; i < m_AdditionalLights.Length; i++)
			{
				m_AdditionalLights[i].SetConnected(isConnected: true);
			}
		}
	}

	private void Update()
	{
		if (!base.IsDisposed && !GameManager.Instance.IsPaused && !(m_PrimaryLight == null) && !m_PrimaryLight.IsOff && m_AdditionalLights != null)
		{
			for (int i = 0; i < m_AdditionalLights.Length; i++)
			{
				m_AdditionalLights[i].UpdateLight(m_PrimaryLight.WaveValue);
			}
		}
	}
}
