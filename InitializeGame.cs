using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using I2.Loc;
using InControl;
using S13Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializeGame : JMonoBehaviour
{
	[SerializeField]
	private bool m_IsGameInitializer;

	private List<Action> m_ActionSequence;

	private GameManager m_GameManager => GameManager.Instance;

	public override void Awake()
	{
		Initialize();
	}

	private void Initialize()
	{
		Application.targetFrameRate = ProjectSettings.Instance.TargetFrameRate;
		PreloadFramework();
	}

	private void PreloadFramework()
	{
		ClearActionSequence();
		m_ActionSequence = new List<Action>();
		m_ActionSequence.Add(InitAssetManager);
		m_ActionSequence.Add(InitDOTween);
		m_ActionSequence.Add(InitInControl);
		m_ActionSequence.Add(InitI2Localization);
		m_ActionSequence.Add(InitS13AudioManager);
		m_ActionSequence.Add(InitPoolingManager);
		m_ActionSequence.Add(InitSectionManager);
		m_ActionSequence.Add(InitRespawnManager);
		m_ActionSequence.Add(InitAchievementManager);
		m_ActionSequence.Add(InitUIManager);
		m_ActionSequence.Add(InitPlayerSettings);
		m_ActionSequence.Add(InitSaveData);
		for (int i = 0; i < m_ActionSequence.Count; i++)
		{
			m_ActionSequence[i]?.Invoke();
		}
		StartCoroutine(PrepareFinalizeFramework());
	}

	private void InitAssetManager()
	{
		JDebug.Log("InitializeGame :: InitAssetManager", this, JDebug.JDebugType.Initialize);
		if (m_GameManager.AssetManager != null)
		{
			JDebug.LogWarning("Asset Manager already exists.", this, JDebug.JDebugType.Initialize);
		}
		else
		{
			m_GameManager.AssetManager = AssetManager.Create();
		}
	}

	private void InitDOTween()
	{
		JDebug.Log("InitializeGame :: InitDOTween", this, JDebug.JDebugType.Initialize);
		DOTween.Init();
		DOTween.SetTweensCapacity(1024, 128);
	}

	private void InitInControl()
	{
		JDebug.Log("InitializeGame :: InitInControl", this, JDebug.JDebugType.Initialize);
		if (m_GameManager.InControlManager != null)
		{
			JDebug.LogWarning("InControllerManager already exists.", this, JDebug.JDebugType.Initialize);
		}
		else
		{
			m_GameManager.InControlManager = new GameObject("[InControl]").AddComponent<InControlManager>();
		}
	}

	private void InitI2Localization()
	{
		JDebug.Log("InitializeGame :: InitI2Localization", this, JDebug.JDebugType.Initialize);
		I2LocalizationGroup i2LocalizationGroup = Resources.Load<I2LocalizationGroup>("Localization/I2LocalizationGroup");
		for (int i = 0; i < i2LocalizationGroup.LanguageSourceAssets.Length; i++)
		{
			LanguageSourceData mSource = i2LocalizationGroup.LanguageSourceAssets[i].mSource;
			if (!LocalizationManager.Sources.Contains(mSource))
			{
				LocalizationManager.AddSource(mSource);
			}
		}
	}

	private void InitS13AudioManager()
	{
		JDebug.Log("InitializeGame :: InitS13AudioManager", this, JDebug.JDebugType.Initialize);
		if (m_GameManager.AudioManager != null)
		{
			JDebug.LogWarning("S13Audio Manager already exists.", this, JDebug.JDebugType.Initialize);
			return;
		}
		S13Manager assetPrefab = Resources.Load<S13Manager>("S13AudioManager");
		m_GameManager.AudioManager = GameManager.Instance.AssetManager.CreateAsset<S13Manager>(assetPrefab);
		m_GameManager.AudioManager.gameObject.name = "[S13AudioManager]";
	}

	private void InitPoolingManager()
	{
		JDebug.Log("InitializeGame :: InitPoolingManager", this, JDebug.JDebugType.Initialize);
		if (m_GameManager.PoolingManager != null)
		{
			JDebug.LogWarning("Pooling Manager already exists.", this, JDebug.JDebugType.Initialize);
		}
		else
		{
			m_GameManager.PoolingManager = PoolingManager.Create();
		}
	}

	private void InitSectionManager()
	{
		JDebug.Log("InitializeGame :: InitSectionManager", this, JDebug.JDebugType.Initialize);
		if (m_GameManager.SectionManager != null)
		{
			JDebug.LogWarning("Section Manager already exists.", this, JDebug.JDebugType.Initialize);
		}
		else
		{
			m_GameManager.SectionManager = SectionManager.Create();
		}
	}

	private void InitRespawnManager()
	{
		JDebug.Log("InitializeGame :: InitRespawnManager", this, JDebug.JDebugType.Initialize);
		if (m_GameManager.RespawnManager != null)
		{
			JDebug.LogWarning("Respawn Manager already exists.", this, JDebug.JDebugType.Initialize);
		}
		else
		{
			m_GameManager.RespawnManager = RespawnManager.Create();
		}
	}

	private void InitAchievementManager()
	{
		JDebug.Log("InitializeGame :: InitAchievementManager", this, JDebug.JDebugType.Initialize);
		if (m_GameManager.SteamManager == null)
		{
			m_GameManager.SteamManager = new GameObject("[SteamManager]").AddComponent<SteamManager>();
		}
		m_GameManager.AchievementManager = AchievementManager.Create();
		m_GameManager.AchievementManager.Init();
	}

	private void InitUIManager()
	{
		JDebug.Log("InitializeGame :: InitUIManager", this, JDebug.JDebugType.Initialize);
		if (m_GameManager.UIManager != null)
		{
			JDebug.LogWarning("UI Mananger already exists.", this, JDebug.JDebugType.Initialize);
		}
		else
		{
			m_GameManager.UIManager = UIManager.Create();
		}
	}

	private void InitPlayerSettings()
	{
		JDebug.Log("InitializeGame :: InitPlayerSettings", this, JDebug.JDebugType.Initialize);
		if (m_GameManager.PlayerSettings != null)
		{
			JDebug.LogWarning("Player Settings already exists.", this, JDebug.JDebugType.Initialize);
		}
		else
		{
			m_GameManager.PlayerSettings = PlayerPreferences.Create();
		}
	}

	private void InitSaveData()
	{
		JDebug.Log("InitializeGame :: InitSaveData", this, JDebug.JDebugType.Initialize);
		if (m_GameManager.GameDataManager != null)
		{
			JDebug.LogWarning("Game Data Manager already exists.", this, JDebug.JDebugType.Initialize);
			return;
		}
		m_GameManager.GameData = new GameData();
		m_GameManager.GameDataManager = new GameDataManager();
		m_GameManager.GameDataManager.LoadGameData();
		if (!m_GameManager.GameData.Credits && m_GameManager.AchievementManager.GetAchievement(AchievementName.THE_MASTERS_PEN))
		{
			m_GameManager.GameData.SetCredits(has: true);
		}
	}

	private IEnumerator PrepareFinalizeFramework()
	{
		while (GameManager.Instance.bAsyncFileIOInProgress)
		{
			yield return null;
		}
		FinalizeFramework();
	}

	private void FinalizeFramework()
	{
		JDebug.Log("InitializeGame :: FinalizeFramework", this, JDebug.JDebugType.Initialize);
		if (m_GameManager.isGameLoaded)
		{
			GameManager.Instance.UIManager.Show<UITitleView>("UI/Views/UITitleView", "VIEW");
			return;
		}
		m_GameManager.isGameLoaded = true;
		if (m_IsGameInitializer)
		{
			SceneManager.LoadScene("Empty");
			GameManager.Instance.UIManager.Show<UIJDS>("UI/Videos/UIJDS", "VIEW").OnPlayOutComplete += HandleJDSIntroOnPlayOutComplete;
		}
		else
		{
			Dispose();
		}
	}

	private void HandleJDSIntroOnPlayOutComplete(object sender, EventArgs e)
	{
		JDebug.Log("InitializeGame :: HandleJDSIntroOnPlayOutComplete", this, JDebug.JDebugType.Initialize);
		(sender as UIJDS).OnPlayOutComplete -= HandleJDSIntroOnPlayOutComplete;
		ShowPhotosensitivityWarning();
	}

	private void HandleConsoleIntroOnPlayOutComplete(object sender, EventArgs e)
	{
		JDebug.Log("InitializeGame :: HandleConsoleIntroOnPlayOutComplete", this, JDebug.JDebugType.Initialize);
		(sender as UIConsole).OnPlayOutComplete -= HandleConsoleIntroOnPlayOutComplete;
		ShowPhotosensitivityWarning();
	}

	private void ShowPhotosensitivityWarning()
	{
		GameManager.Instance.UIManager.Show<UIPhotosensitiveWarning>("UI/Views/UIPhotosensitiveWarning", "VIEW").OnPlayOutComplete += HandlePhotosensitiveWarningOnPlayOutComplete;
	}

	private void HandlePhotosensitiveWarningOnPlayOutComplete(object sender, EventArgs e)
	{
		JDebug.Log("InitializeGame :: HandlePhotosensitiveWarningOnPlayOutComplete", this, JDebug.JDebugType.Initialize);
		(sender as UIPhotosensitiveWarning).OnPlayOutComplete -= HandlePhotosensitiveWarningOnPlayOutComplete;
		GameManager.Instance.UIManager.Show<UITitleView>("UI/Views/UITitleView", "VIEW");
	}

	private void ClearActionSequence()
	{
		if (m_ActionSequence != null)
		{
			m_ActionSequence.Clear();
			m_ActionSequence = null;
		}
	}

	protected override void OnDisposed()
	{
		ClearActionSequence();
		base.OnDisposed();
	}
}
