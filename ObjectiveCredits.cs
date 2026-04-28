using System;
using UnityEngine.SceneManagement;

public class ObjectiveCredits : Objective
{
	protected override void InternalInitialize()
	{
		GameManager.Instance.LockPause();
		SceneManager.LoadScene("Empty");
		GameManager.Instance.ClearPlayer();
		GameManager.Instance.UIManager.Show<UIMainCredits>("UI/Videos/UIMainCredits", "VIEW").OnPlayOutComplete += HandleUIMainCreditsOnPlayOutComplete;
	}

	private void HandleUIMainCreditsOnPlayOutComplete(object sender, EventArgs e)
	{
		JDebug.Log("ObjectiveCredits :: HandleUIMainCreditsOnPlayOutComplete", this, JDebug.JDebugType.UI);
		(sender as UIMainCredits).OnPlayOutComplete -= HandleUIMainCreditsOnPlayOutComplete;
		SceneManager.sceneLoaded += HandleGameSceneLoaded;
		SceneManager.LoadScene("Game");
	}

	private void HandleGameSceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		JDebug.Log("ObjectiveCredits :: HandleGameSceneLoaded", this, JDebug.JDebugType.UI);
		SceneManager.sceneLoaded -= HandleGameSceneLoaded;
		Dispose();
	}
}
