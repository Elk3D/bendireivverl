using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIBlocker : UIController
{
	[Header("Canvas Groups")]
	[SerializeField]
	private CanvasGroup m_Blocker;

	[SerializeField]
	private Image m_Image;

	private bool m_IsIndependent;

	public event EventHandler OnShow;

	public event EventHandler OnHide;

	protected override void OnInitialized(object _data)
	{
		m_Blocker.alpha = 0f;
		m_Blocker.gameObject.SetActive(value: false);
	}

	public bool Show(float duration = 0.5f, float delay = 0f, bool isWhite = false)
	{
		if (m_Blocker.alpha >= 1f)
		{
			return false;
		}
		if (isWhite)
		{
			m_Image.color = Color.white;
		}
		else
		{
			m_Image.color = Color.black;
		}
		DOFade(1f, duration, delay).OnComplete(delegate
		{
			this.OnShow.Send(this);
		});
		return true;
	}

	public bool Hide(float duration = 0.5f, float delay = 0f, bool isWhite = false)
	{
		if (m_Blocker.alpha <= 0f)
		{
			return false;
		}
		if (isWhite)
		{
			m_Image.color = Color.white;
		}
		else
		{
			m_Image.color = Color.black;
		}
		DOFade(0f, duration, delay).OnComplete(DisableBlocker);
		return true;
	}

	private Tweener DOFade(float alpha, float duration, float delay)
	{
		m_Blocker.DOKill();
		m_Blocker.gameObject.SetActive(value: true);
		return m_Blocker.DOFade(alpha, duration).SetDelay(delay).SetEase(Ease.Linear)
			.SetUpdate(UpdateType.Normal, m_IsIndependent);
	}

	public void SetIndependent(bool isIndependent)
	{
		m_IsIndependent = isIndependent;
	}

	private void DisableBlocker()
	{
		m_Blocker.gameObject.SetActive(value: false);
		this.OnHide.Send(this);
		PlayOut();
	}

	protected override void OnDisposed()
	{
		m_Blocker.DOKill();
		this.OnShow = null;
		this.OnHide = null;
		base.OnDisposed();
	}
}
