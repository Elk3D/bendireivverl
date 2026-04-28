using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIArchGateJDSPresents : UIController
{
	[SerializeField]
	private CanvasGroup m_CanvasGroup;

	[SerializeField]
	private TextMeshProUGUI m_Label;

	private UILabelDataVO m_DataVO;

	protected override void OnInitialized(object _data)
	{
		m_DataVO = (UILabelDataVO)_data;
		if (m_DataVO != null)
		{
			m_Label.text = m_DataVO.Message;
		}
		m_CanvasGroup.alpha = 0f;
	}

	public override void PlayIn()
	{
		m_CanvasGroup.DOFade(1f, 3f).SetEase(Ease.Linear).OnComplete(PlayInComplete);
	}

	public override void PlayInComplete()
	{
		base.PlayInComplete();
		PlayOut();
	}

	public override void PlayOut()
	{
		m_CanvasGroup.DOFade(0f, 2f).SetEase(Ease.Linear).SetDelay(1f)
			.OnComplete(PlayOutComplete);
	}

	protected override void OnDisposed()
	{
		m_DataVO = null;
		base.OnDisposed();
	}
}
