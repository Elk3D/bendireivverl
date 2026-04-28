using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Thunder : JMonoBehaviour
{
	[Serializable]
	public class ThunderGroup
	{
		public AudioSource AudioSource;

		public Light[] Light;

		[HideInInspector]
		public List<float> Intensity = new List<float>();
	}

	[SerializeField]
	private bool m_ActiveOnStart;

	[SerializeField]
	private AudioClip m_AudioClipThunder;

	[SerializeField]
	private float m_TimeMin = 60f;

	[SerializeField]
	private float m_TimeMax = 240f;

	[Header("Thunder Groups")]
	[SerializeField]
	private ThunderGroup[] m_ThunderGroups;

	private float m_Timer;

	private float m_TimeLimit;

	private bool m_Thunder;

	private bool m_IsActive;

	public override void Start()
	{
		for (int i = 0; i < m_ThunderGroups.Length; i++)
		{
			ThunderGroup thunderGroup = m_ThunderGroups[i];
			for (int j = 0; j < thunderGroup.Light.Length; j++)
			{
				Light light = thunderGroup.Light[j];
				thunderGroup.Intensity.Add(light.intensity);
			}
		}
		m_TimeLimit = UnityEngine.Random.Range(m_TimeMin, m_TimeMax);
		if (m_ActiveOnStart)
		{
			m_IsActive = true;
		}
	}

	private void Update()
	{
		if (!m_IsActive || m_Thunder)
		{
			return;
		}
		m_Timer += Time.deltaTime;
		if (!(m_Timer > m_TimeLimit))
		{
			return;
		}
		m_Thunder = true;
		Transform transform = null;
		float num = float.PositiveInfinity;
		Vector3 position = GameManager.Instance.Player.transform.position;
		for (int i = 0; i < m_ThunderGroups.Length; i++)
		{
			ThunderGroup thunderGroup = m_ThunderGroups[i];
			thunderGroup.AudioSource.PlayOneShot(m_AudioClipThunder);
			float num2 = Vector3.Distance(thunderGroup.AudioSource.transform.position, position);
			if (num2 < num)
			{
				transform = thunderGroup.AudioSource.transform;
				num = num2;
			}
			for (int j = 0; j < thunderGroup.Light.Length; j++)
			{
				Light obj = thunderGroup.Light[j];
				obj.intensity = 15f;
				obj.DOKill();
				obj.DOIntensity(thunderGroup.Intensity[j], 0.5f).OnComplete(ResetThunder);
			}
		}
		if (transform != null && Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position) < 20f)
		{
			CameraEffects.ShakeRotation(1f, 0.5f);
		}
	}

	private void ResetThunder()
	{
		m_Timer = 0f;
		m_TimeLimit = UnityEngine.Random.Range(m_TimeMin, m_TimeMax);
		m_Thunder = false;
	}

	public void ForceThunder()
	{
		m_IsActive = true;
		m_Thunder = false;
		m_TimeLimit = 0f;
		m_Timer = 10000f;
	}

	public void StopThunder()
	{
		m_IsActive = false;
		m_Thunder = true;
		m_Timer = -1f;
	}

	public void StopAll()
	{
		StopThunder();
		for (int i = 0; i < m_ThunderGroups.Length; i++)
		{
			ThunderGroup thunderGroup = m_ThunderGroups[i];
			thunderGroup.AudioSource.Stop();
			thunderGroup.AudioSource.clip = null;
			thunderGroup.AudioSource.volume = 0f;
			for (int j = 0; j < thunderGroup.Light.Length; j++)
			{
				thunderGroup.Light[j].DOKill();
				thunderGroup.Light[j].intensity = thunderGroup.Intensity[j];
			}
		}
	}
}
