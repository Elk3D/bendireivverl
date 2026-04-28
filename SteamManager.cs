using System;
using System.Text;
using Steamworks;
using UnityEngine;

[DisallowMultipleComponent]
public class SteamManager : JMonoBehaviour
{
	private static bool s_EverInialized;

	private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

	protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;

	public bool IsInitialized { get; private set; }

	private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
	}

	public override void Awake()
	{
		if (s_EverInialized)
		{
			throw new Exception("Tried to Initialize the SteamAPI twice in one session!");
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (!Packsize.Test())
		{
			SteamDebug("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
		}
		if (!DllCheck.Test())
		{
			SteamDebug("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
		}
		try
		{
			if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
			{
				Application.Quit();
				return;
			}
		}
		catch (DllNotFoundException ex)
		{
			SteamDebug("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + ex, this);
			Application.Quit();
			return;
		}
		IsInitialized = SteamAPI.Init();
		if (!IsInitialized)
		{
			SteamDebug("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
			return;
		}
		SteamUserStats.RequestCurrentStats();
		s_EverInialized = true;
	}

	public override void OnEnable()
	{
		if (IsInitialized)
		{
			if (m_SteamAPIWarningMessageHook == null)
			{
				m_SteamAPIWarningMessageHook = SteamAPIDebugTextHook;
				SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
			}
			if (m_GameOverlayActivated == null)
			{
				m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
			}
		}
	}

	private void Update()
	{
		if (IsInitialized)
		{
			SteamAPI.RunCallbacks();
		}
	}

	private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
	{
		if (pCallback.m_bActive != 0)
		{
			if (!GameManager.Instance.IsPaused)
			{
				Time.timeScale = 0f;
			}
			if (AudioListener.volume >= 1f)
			{
				AudioListener.volume = 0f;
			}
		}
		else
		{
			if (!GameManager.Instance.IsPaused)
			{
				Time.timeScale = 1f;
			}
			if (AudioListener.volume <= 0f)
			{
				AudioListener.volume = 1f;
			}
		}
	}

	private void SteamDebug(object message, UnityEngine.Object context)
	{
	}

	protected override void OnDisposed()
	{
		if (IsInitialized)
		{
			SteamAPI.Shutdown();
		}
		m_SteamAPIWarningMessageHook = null;
		m_GameOverlayActivated = null;
		s_EverInialized = false;
		base.OnDisposed();
	}
}
