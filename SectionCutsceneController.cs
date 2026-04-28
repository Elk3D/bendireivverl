using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionCutsceneController : SectionController
{
	private Cutscene[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		m_Group = base.transform.GetComponentsInChildren<Cutscene>(includeInactive: true);
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
			Cutscene cutscene = m_Group[i];
			if (!(cutscene == null) && !(cutscene.Content == null))
			{
				CutsceneDataObject cutsceneDataObject = (CutsceneDataObject)GameManager.Instance.GameData.CurrentSave.GetData<CutsceneID, CutsceneDataObject>(cutscene.SectionID, cutscene.CutsceneID);
				if (cutsceneDataObject != null && cutsceneDataObject.Status == CutsceneStatus.Active)
				{
					cutsceneDataObject.SetTimeline(cutscene.Content.Director.PlayableDirector.time);
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
