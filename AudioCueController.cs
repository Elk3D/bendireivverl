using System;
using UnityEngine;

public class AudioCueController : JMonoBehaviour
{
	[SerializeField]
	private ActionEvent m_ActionEvent;

	[SerializeField]
	private bool m_ResetOnEnd;

	[SerializeField]
	private AudioSource[] m_AudioSources;

	private AudioSource m_CurrentAudioSource;

	private float m_CurrentTime;

	private int m_AudioIndex;

	public bool IsTriggered { get; private set; }

	public event EventHandler OnPlayStart;

	public event EventHandler OnPlayEnd;

	public override void Start()
	{
		m_AudioIndex = 0;
		if (m_ActionEvent != null)
		{
			m_ActionEvent.OnInteract -= HandleActionEventOnInteract;
			m_ActionEvent.OnInteract += HandleActionEventOnInteract;
		}
		for (int i = 0; i < m_AudioSources.Length; i++)
		{
			m_AudioSources[i].gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (!IsTriggered || m_CurrentAudioSource == null || m_CurrentAudioSource.clip == null)
		{
			return;
		}
		m_CurrentTime += Time.deltaTime;
		if (m_CurrentTime >= m_CurrentAudioSource.clip.length)
		{
			m_CurrentAudioSource.gameObject.SetActive(value: false);
			m_CurrentAudioSource = null;
			m_CurrentTime = 0f;
			IsTriggered = false;
			if (m_ResetOnEnd)
			{
				ResetEvent();
			}
			this.OnPlayEnd.Send(this);
		}
	}

	private void HandleActionEventOnInteract(object sender, EventArgs e)
	{
		m_ActionEvent.OnInteract -= HandleActionEventOnInteract;
		if (!IsTriggered && m_AudioSources.Length != 0)
		{
			IsTriggered = true;
			if (m_ResetOnEnd)
			{
				GameManager.Instance.Player.Interaction.ResetInteraction();
			}
			this.OnPlayStart.Send(this);
			m_CurrentAudioSource = m_AudioSources[m_AudioIndex];
			m_CurrentAudioSource.gameObject.SetActive(value: true);
			m_CurrentAudioSource.Play();
			m_AudioIndex++;
			if (m_AudioIndex >= m_AudioSources.Length)
			{
				m_AudioIndex = 0;
			}
		}
	}

	public void DisableEvent()
	{
		if (m_ActionEvent != null)
		{
			m_ActionEvent.SetActive(active: false);
			m_ActionEvent.OnInteract -= HandleActionEventOnInteract;
		}
	}

	public void ResetEvent()
	{
		IsTriggered = false;
		if (m_ActionEvent != null)
		{
			m_ActionEvent.OnInteract -= HandleActionEventOnInteract;
			m_ActionEvent.OnInteract += HandleActionEventOnInteract;
			m_ActionEvent.ResetAction();
			m_ActionEvent.SetActive(active: true);
		}
	}

	protected override void OnDisposed()
	{
		if (m_ActionEvent != null)
		{
			m_ActionEvent.OnInteract -= HandleActionEventOnInteract;
		}
		m_CurrentAudioSource = null;
		base.OnDisposed();
	}
}
