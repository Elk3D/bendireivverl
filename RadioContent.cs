using System;
using System.Collections.Generic;
using S13Audio.BATDR;
using UnityEngine;

public class RadioContent : JMonoBehaviour
{
	[SerializeField]
	private Interactable m_Interactable;

	[SerializeField]
	private BATDRRadioPlayer m_radioPlayer;

	[SerializeField]
	private AudioClip[] m_AudioClips;

	private List<AudioClip> m_AvailableAudioClips = new List<AudioClip>();

	private AudioClip m_CurrentAudioClip;

	private float m_AudioClipTimer;

	private float m_AudioClipLength;

	private bool m_IsPlaying;

	public event EventHandler OnInteract;

	public event EventHandler OnComplete;

	public override void Start()
	{
		if (m_AudioClips.Length != 0)
		{
			SetAudioClips();
			m_Interactable.OnInteract -= HandleInteractableOnInteract;
			m_Interactable.OnInteract += HandleInteractableOnInteract;
		}
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		m_Interactable.OnInteract -= HandleInteractableOnInteract;
		m_Interactable.SetActive(active: false);
		GameManager.Instance.Player.Interaction.ResetInteraction();
		GetNextAudioClip();
		m_IsPlaying = true;
		this.OnInteract.Send(this);
	}

	private void Update()
	{
		if (m_IsPlaying && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			if (m_AudioClipTimer > m_AudioClipLength && m_radioPlayer.IsPlaying)
			{
				ResetRadio();
			}
			else
			{
				m_AudioClipTimer += Time.deltaTime;
			}
		}
	}

	private void SetAudioClips()
	{
		m_AvailableAudioClips.Clear();
		for (int i = 0; i < m_AudioClips.Length; i++)
		{
			AudioClip item = m_AudioClips[i];
			if (!m_AvailableAudioClips.Contains(item))
			{
				m_AvailableAudioClips.Add(item);
			}
		}
	}

	private void GetNextAudioClip()
	{
		AudioClip audioClip = m_AudioClips[0];
		if (m_AudioClips.Length > 1)
		{
			audioClip = GetAudioClip();
			if (m_AvailableAudioClips.Contains(audioClip))
			{
				m_AvailableAudioClips.Remove(audioClip);
			}
		}
		PlayAudioClip(audioClip);
	}

	private AudioClip GetAudioClip()
	{
		AudioClip audioClip = m_AvailableAudioClips[UnityEngine.Random.Range(0, m_AvailableAudioClips.Count)];
		if (audioClip == m_CurrentAudioClip)
		{
			audioClip = GetAudioClip();
		}
		return audioClip;
	}

	private void PlayAudioClip(AudioClip audioClip)
	{
		m_CurrentAudioClip = audioClip;
		m_AudioClipLength = m_CurrentAudioClip.length - 0.5f;
		m_radioPlayer.Play(m_CurrentAudioClip);
	}

	private void ResetRadio()
	{
		m_radioPlayer.Stop();
		m_AudioClipTimer = 0f;
		m_AudioClipLength = 0f;
		if (m_AvailableAudioClips.Count <= 0)
		{
			SetAudioClips();
		}
		m_Interactable.OnInteract -= HandleInteractableOnInteract;
		m_Interactable.OnInteract += HandleInteractableOnInteract;
		m_Interactable.ResetAction();
		m_Interactable.SetActive(active: true);
		m_IsPlaying = false;
		this.OnComplete.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnInteract = null;
		this.OnComplete = null;
		m_Interactable.OnInteract -= HandleInteractableOnInteract;
		m_CurrentAudioClip = null;
		base.OnDisposed();
	}
}
