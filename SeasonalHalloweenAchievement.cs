public static class SeasonalHalloweenAchievement
{
	public static int TrickCount;

	public static int TreatCount;

	public static void Trick()
	{
		TrickCount++;
		if (TrickCount >= 10)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.CABBAGE_NIGHT);
		}
	}

	public static void Treat()
	{
		TreatCount++;
		if (TreatCount >= 10)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.DING_DONG_DELIGHTS);
		}
	}

	public static void Mask()
	{
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.SKULL_OF_THE_CAULDRON);
	}
}
