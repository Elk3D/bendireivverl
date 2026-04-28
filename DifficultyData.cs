using System;
using UnityEngine;

[Serializable]
public class DifficultyData
{
	[SerializeField]
	private DifficultyLevel m_Difficulty;

	[SerializeField]
	private bool m_DifficultyChanged;

	[SerializeField]
	private ButcherGangStatus m_ButcherGangStatus;

	[SerializeField]
	private bool m_IsNotSocialite;

	public DifficultyLevel Difficulty => m_Difficulty;

	public bool DifficultyChanged => m_DifficultyChanged;

	public ButcherGangStatus ButcherGangStatus => m_ButcherGangStatus;

	public bool IsNotSocialite => m_IsNotSocialite;

	public DifficultyData(DifficultyLevel difficulty)
	{
		m_Difficulty = difficulty;
		m_DifficultyChanged = false;
	}

	public void Change(DifficultyLevel difficulty)
	{
		m_Difficulty = difficulty;
		m_DifficultyChanged = true;
	}

	public void SetButcherGangStatus(ButcherGangStatus status)
	{
		m_ButcherGangStatus = status;
	}

	public void UpdateSocialite()
	{
		m_IsNotSocialite = true;
	}
}
