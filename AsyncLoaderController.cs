using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AsyncLoaderController : UIController
{
	[Header("Image")]
	[SerializeField]
	private Image m_Loader;

	[Header("Frames")]
	[SerializeField]
	private Sprite[] m_Fames;

	private int m_FrameIndex;

	private bool m_IsLoading;

	protected override void OnInitialized(object _data)
	{
		m_Loader.color = new Color(1f, 1f, 1f, 0f);
		m_FrameIndex = 0;
		m_Loader.sprite = m_Fames[m_FrameIndex];
		m_IsLoading = true;
		StartCoroutine(DOLoader());
	}

	private IEnumerator DOLoader()
	{
		m_Loader.color = new Color(1f, 1f, 1f, 1f);
		m_Loader.sprite = m_Fames[0];
		while (m_IsLoading)
		{
			m_Loader.sprite = m_Fames[m_FrameIndex];
			m_FrameIndex--;
			if (m_FrameIndex < 0)
			{
				m_FrameIndex = m_Fames.Length - 1;
			}
			yield return new WaitForSecondsRealtime(0.015f);
		}
		m_Loader.color = new Color(1f, 1f, 1f, 0f);
		PlayOut();
	}

	public void LoadingComplete()
	{
		m_FrameIndex = 0;
		m_IsLoading = false;
	}

	protected override void OnDisposed()
	{
		StopCoroutine(DOLoader());
		base.OnDisposed();
	}
}
