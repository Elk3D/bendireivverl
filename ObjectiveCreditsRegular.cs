using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveCreditsRegular : Objective
{
	private Sequence m_Sequence;

	protected override void InternalInitialize()
	{
		JDebug.Log("ObjectiveCreditsRegular :: InternalInitialize", this, JDebug.JDebugType.UI);
		base.transform.SetParent(null);
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		GameManager.Instance.LockPause();
		ResetSequence();
		m_Sequence.InsertCallback(3f, delegate
		{
		});
		m_Sequence.InsertCallback(5f, delegate
		{
			GameManager.Instance.ClearPlayer();
			SceneManager.sceneLoaded += HandleGameSceneLoaded;
			SceneManager.LoadScene("Empty");
		});
	}

	private void HandleGameSceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		JDebug.Log("ObjectiveCreditsRegular :: HandleGameSceneLoaded", this, JDebug.JDebugType.UI);
		SceneManager.sceneLoaded -= HandleGameSceneLoaded;
		ResetSequence();
		m_Sequence.InsertCallback(2f, delegate
		{
			AudioListener.volume = 1f;
			GameManager.Instance.UIManager.Show<UICredits>("UI/Views/UICredits", "VIEW").OnPlayOutComplete += HandleUICreditsOnPlayOutComplete;
		});
	}

	private void HandleUICreditsOnPlayOutComplete(object sender, EventArgs e)
	{
		JDebug.Log("ObjectiveCreditsRegular :: HandleUICreditsOnPlayOutComplete", this, JDebug.JDebugType.UI);
		(sender as UICredits).OnPlayOutComplete -= HandleUICreditsOnPlayOutComplete;
		SceneManager.LoadScene("Reset");
		Dispose();
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			m_Sequence.Kill();
			m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		SceneManager.sceneLoaded -= HandleGameSceneLoaded;
		KillSequence();
		base.OnDisposed();
	}
}
