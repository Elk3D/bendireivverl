using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening.Core;
using S13Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameReset : JMonoBehaviour
{
	private List<GameObject> m_GameObjects = new List<GameObject>();

	public override void Awake()
	{
		if (GameManager.Instance.GameData != null && GameManager.Instance.GameData.CurrentSave != null)
		{
			GameManager.Instance.GameData.CurrentSave.PostSave();
		}
		GameManager.Instance.HideCursor();
		S13Manager.UnloadAllSoundBanks();
		m_GameObjects = Object.FindObjectsOfType<GameObject>().ToList();
		m_GameObjects.Remove(base.gameObject);
		foreach (GameObject gameObject in m_GameObjects)
		{
			if (!gameObject.GetComponent<DOTweenComponent>() && !gameObject.GetComponent<SteamManager>())
			{
				Object.Destroy(gameObject);
			}
		}
		GameManager.Instance.Clear();
		GameManager.Instance.isGameLoaded = true;
		GameManager.Instance.LockPause();
		Time.timeScale = 1f;
	}

	public override void Start()
	{
		StartCoroutine(GoToMainMenu());
	}

	private IEnumerator GoToMainMenu()
	{
		yield return new WaitForEndOfFrame();
		AsyncOperation async = SceneManager.LoadSceneAsync("InitializeGame");
		async.allowSceneActivation = false;
		while (async.progress < 0.9f)
		{
			yield return new WaitForEndOfFrame();
		}
		async.allowSceneActivation = true;
		Dispose();
	}

	protected override void OnDisposed()
	{
		if (m_GameObjects != null)
		{
			m_GameObjects.Clear();
			m_GameObjects = null;
		}
		base.OnDisposed();
	}
}
