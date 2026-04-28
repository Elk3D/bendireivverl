using System;
using DG.Tweening;
using UnityEngine;

public class EventTriggerAudioFader : JMonoBehaviour
{
	[SerializeField]
	private EventTrigger m_EventTrigger;

	[SerializeField]
	private AudioSource m_AudioSource;

	[SerializeField]
	private CutsceneDirector m_CutsceneDirector;

	public void Activate()
	{
		m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		m_EventTrigger.OnEnter += HandleEventTriggerOnEnter;
		m_EventTrigger.SetActive(active: true);
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		m_AudioSource.DOKill();
		m_AudioSource.DOFade(0f, 2f).SetEase(Ease.Linear).OnComplete(delegate
		{
			m_AudioSource.Stop();
			if (m_CutsceneDirector != null)
			{
				m_CutsceneDirector.Stop();
			}
		});
	}

	protected override void OnDisposed()
	{
		if (m_EventTrigger != null)
		{
			m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		}
		m_AudioSource.DOKill();
		base.OnDisposed();
	}
}
