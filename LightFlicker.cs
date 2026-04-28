using System;
using S13Audio;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : JMonoBehaviour
{
	private enum WaveType
	{
		SIN,
		TRI,
		SQR,
		SAW,
		INV,
		NOISE
	}

	[SerializeField]
	private bool m_IsOff;

	[Header("Flicker Settings")]
	[SerializeField]
	private WaveType m_WaveType;

	[SerializeField]
	private float m_Base;

	[SerializeField]
	private float m_Amplitude;

	[SerializeField]
	private float m_Phase;

	[SerializeField]
	private float m_Frequency;

	[Header("Emission - Primary Light Fixture")]
	[SerializeField]
	private LightFixture m_LightFixture;

	[Header("Emission - Additional Light Fixtures")]
	[SerializeField]
	private LightFixture[] m_ExtraLightFixtures;

	[Header("Audio Settings")]
	[SerializeField]
	private S13LocalAction m_OnMaxBrightnessAction;

	private bool m_AtMaxFlag;

	private Color m_OriginalColor;

	private float m_OrigintalIntensity;

	private float m_OriginalEmission = 1f;

	private bool m_IsConnected;

	public Color OriginalColor => m_OriginalColor;

	public Light Light { get; private set; }

	public bool IsOff => m_IsOff;

	public float WaveValue { get; private set; }

	public override void Awake()
	{
		Light = GetComponent<Light>();
		m_OriginalColor = Light.color;
		m_OrigintalIntensity = Light.intensity;
		if (m_LightFixture != null)
		{
			m_OriginalEmission = m_LightFixture.EmissionValue;
		}
	}

	private void Update()
	{
		if (!IsOff && !base.IsDisposed && !GameManager.Instance.IsPaused && !(Light == null) && !m_IsConnected && Light.enabled && Light.gameObject.activeInHierarchy)
		{
			UpdateLight(EvalWave());
		}
	}

	public void SetConnected(bool isConnected)
	{
		m_IsConnected = isConnected;
	}

	public void TurnOn(bool light = false)
	{
		if (light)
		{
			m_IsOff = true;
		}
		else
		{
			m_IsOff = false;
		}
		if (Light != null)
		{
			Light.color = m_OriginalColor;
			Light.intensity = m_OrigintalIntensity;
		}
		if (!(m_LightFixture != null))
		{
			return;
		}
		m_LightFixture.SetEmission(m_OriginalEmission);
		if (m_ExtraLightFixtures == null || m_ExtraLightFixtures.Length == 0)
		{
			return;
		}
		for (int i = 0; i < m_ExtraLightFixtures.Length; i++)
		{
			LightFixture lightFixture = m_ExtraLightFixtures[i];
			if (lightFixture != null)
			{
				lightFixture.SetEmission(m_OriginalEmission);
			}
		}
	}

	public void TurnOff(bool light = false)
	{
		m_IsOff = true;
		if (light)
		{
			Light.color = m_OriginalColor;
			Light.intensity = 0f;
			if (!(m_LightFixture != null))
			{
				return;
			}
			m_LightFixture.SetEmission(0f);
			if (m_ExtraLightFixtures == null || m_ExtraLightFixtures.Length == 0)
			{
				return;
			}
			for (int i = 0; i < m_ExtraLightFixtures.Length; i++)
			{
				LightFixture lightFixture = m_ExtraLightFixtures[i];
				if (lightFixture != null)
				{
					lightFixture.SetEmission(0f);
				}
			}
			return;
		}
		if (Light != null)
		{
			Light.color = m_OriginalColor;
			Light.intensity = m_OrigintalIntensity;
		}
		if (!(m_LightFixture != null))
		{
			return;
		}
		m_LightFixture.SetEmission(m_OriginalEmission);
		if (m_ExtraLightFixtures.Length == 0)
		{
			return;
		}
		for (int j = 0; j < m_ExtraLightFixtures.Length; j++)
		{
			LightFixture lightFixture2 = m_ExtraLightFixtures[j];
			if (lightFixture2 != null)
			{
				lightFixture2.SetEmission(m_OriginalEmission);
			}
		}
	}

	public void UpdateLight(float value)
	{
		if (value < 0f)
		{
			Light.color = m_OriginalColor * 0f;
		}
		else
		{
			Light.color = m_OriginalColor * value;
		}
		if (m_LightFixture != null)
		{
			m_LightFixture.SetEmission(value);
			if (m_ExtraLightFixtures != null && m_ExtraLightFixtures.Length != 0)
			{
				for (int i = 0; i < m_ExtraLightFixtures.Length; i++)
				{
					LightFixture lightFixture = m_ExtraLightFixtures[i];
					if (lightFixture != null)
					{
						lightFixture.SetEmission(value);
					}
				}
			}
		}
		if (!m_AtMaxFlag && value >= 0.95f)
		{
			m_AtMaxFlag = true;
			m_OnMaxBrightnessAction.Execute();
		}
		else if (m_AtMaxFlag && value < 0.95f)
		{
			m_AtMaxFlag = false;
		}
	}

	private float EvalWave()
	{
		float num = (Time.time + m_Phase) * m_Frequency;
		float num2 = 0f;
		num -= Mathf.Floor(num);
		return WaveValue = m_WaveType switch
		{
			WaveType.SIN => Mathf.Sin(num * 2f * MathF.PI), 
			WaveType.TRI => (!(num < 0.5f)) ? (-4f * num + 3f) : (4f * num - 1f), 
			WaveType.SQR => (!(num < 0.5f)) ? (-1f) : 1f, 
			WaveType.SAW => num, 
			WaveType.INV => 1f - num, 
			WaveType.NOISE => 1f - UnityEngine.Random.value * 2f, 
			_ => 1f, 
		} * m_Amplitude + m_Base;
	}

	protected override void OnDisposed()
	{
		Light = null;
		base.OnDisposed();
	}
}
