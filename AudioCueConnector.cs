using System;
using UnityEngine;

public class AudioCueConnector : JMonoBehaviour
{
	[SerializeField]
	private AudioCueController[] m_AudioCues;

	public override void Start()
	{
		for (int i = 0; i < m_AudioCues.Length; i++)
		{
			AudioCueController obj = m_AudioCues[i];
			obj.OnPlayStart += HandleAudioCueOnPlayStart;
			obj.OnPlayEnd += HandleAudioCueOnPlayEnd;
		}
	}

	private void HandleAudioCueOnPlayStart(object sender, EventArgs e)
	{
		for (int i = 0; i < m_AudioCues.Length; i++)
		{
			m_AudioCues[i].DisableEvent();
		}
	}

	private void HandleAudioCueOnPlayEnd(object sender, EventArgs e)
	{
		AudioCueController audioCueController = (AudioCueController)sender;
		for (int i = 0; i < m_AudioCues.Length; i++)
		{
			AudioCueController audioCueController2 = m_AudioCues[i];
			if (audioCueController2 != audioCueController)
			{
				audioCueController2.ResetEvent();
			}
		}
	}

	protected override void OnDisposed()
	{
		for (int i = 0; i < m_AudioCues.Length; i++)
		{
			AudioCueController obj = m_AudioCues[i];
			obj.OnPlayStart -= HandleAudioCueOnPlayStart;
			obj.OnPlayEnd -= HandleAudioCueOnPlayEnd;
		}
		base.OnDisposed();
	}
}
