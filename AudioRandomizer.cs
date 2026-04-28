using System;
using UnityEngine;

public class AudioRandomizer : JMonoBehaviour
{
	[Serializable]
	public class AudioGroup
	{
		public AudioClip AudioClip;

		public string Subtitles;
	}

	[SerializeField]
	private AudioSource m_AudioSource;

	[SerializeField]
	private AudioGroup[] m_AudioGroups;

	private AudioGroup m_CurrentAudioGroup;

	private int m_Index;

	public void Play()
	{
		while (m_CurrentAudioGroup == null || m_CurrentAudioGroup.AudioClip == m_AudioSource.clip)
		{
			GetAudioClip();
		}
		m_AudioSource.clip = m_CurrentAudioGroup.AudioClip;
		m_AudioSource.Play();
		GameManager.Instance.ShowSubtitles(TextUtility.GetKey(m_CurrentAudioGroup.Subtitles), m_CurrentAudioGroup.AudioClip.length, isTrimmed: true);
	}

	private void GetAudioClip()
	{
		m_Index = UnityEngine.Random.Range(0, m_AudioGroups.Length);
		m_CurrentAudioGroup = m_AudioGroups[m_Index];
	}

	protected override void OnDisposed()
	{
		m_CurrentAudioGroup = null;
		base.OnDisposed();
	}
}
