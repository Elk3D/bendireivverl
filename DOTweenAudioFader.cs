using DG.Tweening;
using UnityEngine;

public class DOTweenAudioFader : JMonoBehaviour
{
	[SerializeField]
	private AudioSource m_AudioSource;

	[SerializeField]
	private float m_StartVolume;

	[SerializeField]
	private float m_EndVolume;

	[SerializeField]
	private float m_Duration;

	public override void Start()
	{
		m_AudioSource.volume = m_StartVolume;
		m_AudioSource.DOKill();
		m_AudioSource.DOFade(m_EndVolume, m_Duration).SetEase(Ease.Linear);
	}
}
