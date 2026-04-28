public static class AchievementChecker
{
	public static void Check(AchievementName achievementName)
	{
		if (CanUnlock(achievementName))
		{
			GetAchievement(achievementName);
		}
	}

	private static bool CanUnlock(AchievementName achievementName)
	{
		bool result = true;
		switch (achievementName)
		{
		case AchievementName.SOCIALITE:
			if (GameManager.Instance.GameData.CurrentSave.Difficulty.IsNotSocialite)
			{
				result = false;
			}
			break;
		case AchievementName.THE_MASTERS_PEN:
		{
			DifficultyData difficulty = GameManager.Instance.GameData.CurrentSave.Difficulty;
			if (!difficulty.DifficultyChanged)
			{
				if (difficulty.Difficulty == DifficultyLevel.Hard)
				{
					GetAchievement(AchievementName.STUDIO_STARTER);
					GetAchievement(AchievementName.STUDIO_SCRAPPER);
					GetAchievement(AchievementName.STUDIO_BREAKER);
				}
				else if (difficulty.Difficulty == DifficultyLevel.Normal)
				{
					GetAchievement(AchievementName.STUDIO_STARTER);
					GetAchievement(AchievementName.STUDIO_SCRAPPER);
				}
				else if (difficulty.Difficulty == DifficultyLevel.Easy)
				{
					GetAchievement(AchievementName.STUDIO_STARTER);
				}
			}
			if (difficulty.ButcherGangStatus == ButcherGangStatus.Butcher)
			{
				GetAchievement(AchievementName.A_BUTCHERED_DECISION);
			}
			if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Life <= 0)
			{
				GetAchievement(AchievementName.INK_MASTER);
			}
			GameManager.Instance.GameData.SetCredits(has: true);
			break;
		}
		}
		return result;
	}

	private static void GetAchievement(AchievementName achievementName)
	{
		GameManager.Instance.AchievementManager.SetAchievement(achievementName);
	}
}
