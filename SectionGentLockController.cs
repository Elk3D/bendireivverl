using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentLockController : SectionController
{
	private GentLock[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		m_Group = base.transform.GetComponentsInChildren<GentLock>(includeInactive: true);
		for (int i = 0; i < m_Group.Length; i++)
		{
			m_Group[i].Initialize();
			yield return null;
		}
	}

	public override void SaveData()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			GentLock gentLock = m_Group[i];
			if (!(gentLock == null) && !(gentLock.Content == null))
			{
				GentLockDataObject gentLockDataObject = (GentLockDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.GentLockDirectory.GetValue(gentLock.GentLockID);
				if (gentLockDataObject != null && gentLockDataObject.Status == CutsceneStatus.Active)
				{
					gentLockDataObject.SetTimeline(gentLock.Content.Director.PlayableDirector.time);
				}
			}
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		m_Group = null;
	}
}
