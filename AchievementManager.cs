public class AchievementManager : JDisposable
{
	private IAchievements m_AchievementController;

	private AchievementIDMapping[] m_AchievementIDs;

	public static AchievementManager Create()
	{
		return new AchievementManager();
	}

	protected AchievementManager()
	{
	}

	public void Init()
	{
		m_AchievementIDs = new AchievementIDMapping[0];
		m_AchievementController = new SteamAchievements();
		if (m_AchievementController != null)
		{
			m_AchievementController.Initialize(m_AchievementIDs);
		}
	}

	public bool GetAchievement(AchievementName key)
	{
		if (!CanUseAchievements())
		{
			return false;
		}
		return m_AchievementController.GetAchievement(key);
	}

	public void SetAchievement(AchievementName key)
	{
		if (CanUseAchievements())
		{
			m_AchievementController.SetAchievement(key);
		}
	}

	public void ClearAchievement(AchievementName key)
	{
		if (CanUseAchievements())
		{
			m_AchievementController.ClearAchievement(key);
		}
	}

	private bool CanUseAchievements()
	{
		if (m_AchievementController != null)
		{
			return m_AchievementController.IsConnected();
		}
		return false;
	}

	protected override void OnDisposed()
	{
		m_AchievementController = null;
		m_AchievementIDs = null;
		base.OnDisposed();
	}
}
