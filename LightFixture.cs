using UnityEngine;

public class LightFixture : JMonoBehaviour
{
	private const string POWER = "_Power";

	[SerializeField]
	private MeshRenderer m_LightRenderer;

	[SerializeField]
	private SkinnedMeshRenderer m_SkinnedLightRenderer;

	public MeshRenderer Renderer => m_LightRenderer;

	public SkinnedMeshRenderer SkinnedRenderer => m_SkinnedLightRenderer;

	public float InitialEmissionValue { get; private set; }

	public float EmissionValue
	{
		get
		{
			if (m_LightRenderer != null)
			{
				if (!HasEmission)
				{
					return 1f;
				}
				return m_LightRenderer.material.GetFloat("_Power");
			}
			if (m_SkinnedLightRenderer != null)
			{
				if (!HasEmission)
				{
					return 1f;
				}
				return m_SkinnedLightRenderer.material.GetFloat("_Power");
			}
			return 1f;
		}
	}

	public bool HasEmission
	{
		get
		{
			if (m_LightRenderer != null)
			{
				if (!Application.isPlaying)
				{
					if (m_LightRenderer != null && m_LightRenderer.sharedMaterial != null)
					{
						return m_LightRenderer.sharedMaterial.HasProperty("_Power");
					}
					return false;
				}
				if (m_LightRenderer != null && m_LightRenderer.material != null)
				{
					return m_LightRenderer.material.HasProperty("_Power");
				}
				return false;
			}
			if (m_SkinnedLightRenderer != null)
			{
				if (!Application.isPlaying)
				{
					if (m_SkinnedLightRenderer != null && m_SkinnedLightRenderer.sharedMaterial != null)
					{
						return m_SkinnedLightRenderer.sharedMaterial.HasProperty("_Power");
					}
					return false;
				}
				if (m_SkinnedLightRenderer != null && m_SkinnedLightRenderer.material != null)
				{
					return m_SkinnedLightRenderer.material.HasProperty("_Power");
				}
				return false;
			}
			return false;
		}
	}

	public override void Start()
	{
		InitialEmissionValue = EmissionValue;
	}

	public void SetEmission(float value)
	{
		if (!HasEmission)
		{
			return;
		}
		if (value < 0f)
		{
			value = 0f;
		}
		else if (value > 1f)
		{
			value = 1f;
		}
		if (m_LightRenderer != null)
		{
			if (!Application.isPlaying)
			{
				m_LightRenderer.sharedMaterial?.SetFloat("_Power", value);
			}
			else
			{
				m_LightRenderer.material?.SetFloat("_Power", value);
			}
		}
		else if (m_SkinnedLightRenderer != null)
		{
			if (!Application.isPlaying)
			{
				m_SkinnedLightRenderer.sharedMaterial?.SetFloat("_Power", value);
			}
			else
			{
				m_SkinnedLightRenderer.material?.SetFloat("_Power", value);
			}
		}
	}
}
