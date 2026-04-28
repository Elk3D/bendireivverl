using UnityEngine;

public class ObjectiveReplaceTaskEntry : Objective
{
	[SerializeField]
	private TaskID m_TaskID;

	[SerializeField]
	private TaskEntryID m_CurrentTaskEntryID;

	[SerializeField]
	private TaskEntryID m_ReplacementTaskEntryID;

	[SerializeField]
	private bool m_ShowPrompt;

	[SerializeField]
	private bool m_DisplayEntryText;

	[SerializeField]
	private bool m_IsReal;

	protected override void InternalInitialize()
	{
		TaskDataObject taskDataObject = GameManager.Instance.GameData.CurrentSave.DataDirectories.TaskDirectory.GetValue(m_TaskID) as TaskDataObject;
		if (taskDataObject == null)
		{
			taskDataObject = DataObject<TaskID, TaskDataObject>.Create(m_TaskID);
			GameManager.Instance.GameData.CurrentSave.DataDirectories.TaskDirectory.Add(m_TaskID, taskDataObject);
		}
		TaskEntryDataObject entry = taskDataObject.GetEntry(m_CurrentTaskEntryID);
		if (entry != null)
		{
			taskDataObject.Tasks.Remove(entry);
		}
		if (taskDataObject.GetEntry(m_ReplacementTaskEntryID) == null)
		{
			taskDataObject.Tasks.Add(DataObject<TaskEntryID, TaskEntryDataObject>.Create(m_ReplacementTaskEntryID));
		}
		if (m_ShowPrompt)
		{
			string key = m_TaskID.ToString();
			if (m_DisplayEntryText)
			{
				key = m_ReplacementTaskEntryID.ToString();
			}
			GameManager.Instance.ShowObjective(TextUtility.GetKey("Objective_Update").ToUpper(), TextUtility.GetKey(key).ToUpper(), m_IsReal);
		}
		SendOnComplete();
	}
}
