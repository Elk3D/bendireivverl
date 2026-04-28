using System;
using UnityEngine;

[Serializable]
public class SaveFileData : IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_FileID;

	[SerializeField]
	private string m_FileName;

	[SerializeField]
	private string m_FileTitle;

	[SerializeField]
	private string m_TimeStamp;

	[SerializeField]
	private string m_CurrentZone;

	[SerializeField]
	private SectionID m_CurrentSectionID;

	[SerializeField]
	private DifficultyLevel m_Difficulty;

	public int ID => m_FileID;

	public string Name => m_FileName;

	public string Title => m_FileTitle;

	public string TimeStamp => m_TimeStamp;

	public string CurrentZone => m_CurrentZone;

	public SectionID CurrentSectionID => m_CurrentSectionID;

	public DifficultyLevel Difficulty => m_Difficulty;

	public SaveFileData(int id, string fileName, string fileTitle)
	{
		m_FileID = id;
		m_FileName = fileName;
		m_FileTitle = fileTitle;
	}

	public void Update(SaveData saveData)
	{
		if (GameManager.Instance.Player != null)
		{
			m_CurrentZone = GameManager.Instance.Player.CurrentZone;
			m_CurrentSectionID = GameManager.Instance.Player.CurrentSectionID;
		}
		m_TimeStamp = new DateTime(saveData.TimeData.Date).ToString();
		m_Difficulty = saveData.Difficulty.Difficulty;
	}
}
