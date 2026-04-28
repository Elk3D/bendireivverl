using UnityEngine;

public class ObjectiveNewTasks : Objective
{
	[SerializeField]
	private TaskID m_TaskID;

	[SerializeField]
	private TaskEntryID[] m_TaskEntryID;

	[SerializeField]
	private bool m_ShowPrompt = true;

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
		for (int i = 0; i < m_TaskEntryID.Length; i++)
		{
			TaskEntryID taskEntryID = m_TaskEntryID[i];
			TaskEntryDataObject entry = taskDataObject.GetEntry(taskEntryID);
			if (entry == null)
			{
				entry = DataObject<TaskEntryID, TaskEntryDataObject>.Create(taskEntryID);
				taskDataObject.Tasks.Add(entry);
			}
		}
		if (m_ShowPrompt)
		{
			GameManager.Instance.ShowObjective(TextUtility.GetKey("Objective_New").ToUpper(), TextUtility.GetKey(m_TaskID.ToString()).ToUpper(), m_IsReal);
		}
		SendOnComplete();
	}
}
