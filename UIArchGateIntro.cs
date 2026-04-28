using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIArchGateIntro : UIController
{
	[SerializeField]
	private TextMeshProUGUI m_Label;

	[SerializeField]
	private CanvasGroup m_CanvasGroup;

	private Sequence m_Sequence;

	protected override void OnInitialized(object _data)
	{
		m_CanvasGroup.alpha = 0f;
	}

	public override void PlayIn()
	{
		InitializeSequence();
		m_Sequence.Insert(1f, m_CanvasGroup.DOFade(1f, 1f).SetEase(Ease.OutQuad));
		m_Sequence.Insert(3f, m_CanvasGroup.DOFade(0f, 1f).SetEase(Ease.InQuad));
		m_Sequence.OnComplete(PlayInComplete);
	}

	public override void PlayInComplete()
	{
		base.PlayInComplete();
		PlayOut();
	}

	private void InitializeSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		m_Sequence?.Kill();
		m_Sequence = null;
	}

	protected override void OnDisposed()
	{
		KillSequence();
		base.OnDisposed();
	}
}
