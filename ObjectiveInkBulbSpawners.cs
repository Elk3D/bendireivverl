using UnityEngine;

public class ObjectiveInkBulbSpawners : Objective
{
	[SerializeField]
	private InkBulb[] m_InkBulbs;

	[SerializeField]
	private bool m_EnableOnForceComplete;

	protected override void InternalInitialize()
	{
		EnableInkBulbs();
		SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		if (m_EnableOnForceComplete)
		{
			EnableInkBulbs();
		}
		SendOnComplete();
	}

	private void EnableInkBulbs()
	{
		m_InkBulbs.Shuffle();
		for (int i = 0; i < GetMaxBulbs(); i++)
		{
			InkBulb inkBulb = m_InkBulbs[i];
			inkBulb.SetActive(active: true);
			if (m_EnableOnForceComplete)
			{
				inkBulb.Activate();
			}
		}
	}

	private int GetMaxBulbs()
	{
		int num = m_InkBulbs.Length;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Easy:
			num /= 4;
			break;
		case DifficultyLevel.Normal:
			num /= 2;
			break;
		}
		return num;
	}
}
