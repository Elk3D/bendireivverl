using UnityEngine;

public class ObjectiveNewTask : Objective
{
	[SerializeField]
	private TaskID m_TaskID;

	[SerializeField]
	private TaskEntryID m_TaskEntryID;

	[SerializeField]
	private bool m_ShowPrompt = true;

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
		TaskEntryDataObject entry = taskDataObject.GetEntry(m_TaskEntryID);
		if (entry == null)
		{
			entry = DataObject<TaskEntryID, TaskEntryDataObject>.Create(m_TaskEntryID);
			taskDataObject.Tasks.Add(entry);
		}
		if (m_ShowPrompt)
		{
			string message = TextUtility.GetKey(m_TaskID.ToString()).ToUpper();
			if (m_DisplayEntryText)
			{
				message = TextUtility.GetKey(m_TaskEntryID.ToString()).ToUpper();
			}
			GameManager.Instance.ShowObjective(TextUtility.GetKey("Objective_New").ToUpper(), message, m_IsReal);
		}
		SendOnComplete();
	}
}
