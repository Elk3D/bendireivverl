using System;
using S13Audio;
using UnityEngine;

public class ActionEventAudioConnector : JMonoBehaviour
{
	[SerializeField]
	private ActionEvent m_ActionEvent;

	[SerializeField]
	private AudioSource m_AudioSource;

	[SerializeField]
	private AudioClip[] m_AudioClips;

	[SerializeField]
	private float m_RepeatDelay;

	[SerializeField]
	private bool m_IncludeOnExit;

	[SerializeField]
	private bool m_UseS13;

	[Header("Studio13 Audio")]
	[SerializeField]
	private S13LocalAction onPlayAction;

	private bool m_IsPlayed;

	private float m_RepeatDelayCount;

	private AudioClip m_CurrentAudioClip;

	public event EventHandler OnEnter;

	public override void Start()
	{
		if (m_ActionEvent != null)
		{
			m_ActionEvent.OnInteract += HandleActionEventOnInteract;
			if (m_IncludeOnExit)
			{
				m_ActionEvent.OnExit += HandleActionEventOnInteract;
			}
		}
	}

	private void HandleActionEventOnInteract(object sender, EventArgs e)
	{
		PlayS13Audio();
		if (!m_IsPlayed)
		{
			PlayAudio();
		}
	}

	private void Update()
	{
		if (m_IsPlayed && m_RepeatDelay != 0f)
		{
			m_RepeatDelayCount += Time.deltaTime;
			if (m_RepeatDelayCount >= m_RepeatDelay)
			{
				m_IsPlayed = false;
				m_RepeatDelayCount = 0f;
			}
		}
	}

	private void PlayAudio()
	{
		if (!m_UseS13)
		{
			AudioClip currentAudioClip = m_CurrentAudioClip;
			if (m_AudioClips.Length > 1)
			{
				while (currentAudioClip == m_CurrentAudioClip)
				{
					m_CurrentAudioClip = m_AudioClips[UnityEngine.Random.Range(0, m_AudioClips.Length)];
				}
			}
			else
			{
				if (m_AudioClips.Length != 1)
				{
					return;
				}
				m_CurrentAudioClip = m_AudioClips[0];
			}
			if (m_AudioSource != null)
			{
				m_AudioSource.clip = m_CurrentAudioClip;
				m_AudioSource.Play();
			}
		}
		m_IsPlayed = true;
		this.OnEnter.Send(this);
	}

	private void PlayS13Audio()
	{
		if (onPlayAction.IsExecutable)
		{
			onPlayAction.Execute();
		}
	}

	protected override void OnDisposed()
	{
		this.OnEnter = null;
		if (m_ActionEvent != null)
		{
			m_ActionEvent.OnInteract -= HandleActionEventOnInteract;
			if (m_IncludeOnExit)
			{
				m_ActionEvent.OnExit -= HandleActionEventOnInteract;
			}
		}
		base.OnDisposed();
	}
}
