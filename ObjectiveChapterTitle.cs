using UnityEngine;

public class ObjectiveChapterTitle : Objective
{
	[SerializeField]
	private ChapterID m_ChapterID;

	protected override void InternalEnable()
	{
		Initialize();
	}

	protected override void InternalInitialize()
	{
		string chapter = TextUtility.GetKey(Chapter.GetChapterString(m_ChapterID)).ToUpper() + ":";
		string title = TextUtility.GetKey(Chapter.GetChapterName(m_ChapterID)).ToUpper();
		GameManager.Instance.ShowChapterTitle(chapter, title);
		CheckAchievement();
		SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		SendOnComplete();
	}

	private void CheckAchievement()
	{
		if (m_ChapterID == ChapterID.Chapter_Five)
		{
			AchievementChecker.Check(AchievementName.TIMELESS_REMAINS);
		}
		else if (m_ChapterID == ChapterID.Chapter_Four)
		{
			AchievementChecker.Check(AchievementName.THRILLS_AND_SPILLS);
		}
		else if (m_ChapterID == ChapterID.Chapter_Three)
		{
			AchievementChecker.Check(AchievementName.RUBBERHOSE_NIGHTMARE);
		}
		else if (m_ChapterID == ChapterID.Chapter_Two)
		{
			AchievementChecker.Check(AchievementName.CARTOON_MADNESS);
		}
		else if (m_ChapterID == ChapterID.Chapter_One)
		{
			AchievementChecker.Check(AchievementName.WELCOME_TO_THE_STUDIO);
		}
	}
}
