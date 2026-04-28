using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIChapterTitle : UIController
{
	[SerializeField]
	private CanvasGroup m_CanvasGroup;

	[SerializeField]
	private TextMeshProUGUI m_ChapterLabel;

	[SerializeField]
	private TextMeshProUGUI m_TitleLabel;

	private string[] m_Labels;

	protected override void OnInitialized(object _data)
	{
		m_Labels = (string[])_data;
		m_CanvasGroup.alpha = 0f;
		if (m_Labels == null)
		{
			JDebug.LogError("UIChapterTitle :: OnInitialized => m_Labels = " + m_Labels, this);
		}
		if (m_Labels != null)
		{
			m_ChapterLabel.text = m_Labels[0];
			m_TitleLabel.text = m_Labels[1];
		}
	}

	public override void PlayIn()
	{
		m_CanvasGroup.DOFade(1f, 2f).SetDelay(0.25f).SetEase(Ease.Linear)
			.OnComplete(PlayInComplete);
	}

	public override void PlayInComplete()
	{
		base.PlayInComplete();
		PlayOut();
	}

	public override void PlayOut()
	{
		m_CanvasGroup.DOFade(0f, 3f).SetEase(Ease.Linear).SetDelay(4.5f)
			.OnComplete(PlayOutComplete);
	}

	protected override void OnDisposed()
	{
		m_CanvasGroup.DOKill();
		base.OnDisposed();
	}
}
