using System.Collections;

public class SectionCompanionDirectorController : SectionController
{
	private CompanionDirector[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		m_Group = GetComponentsInChildren<CompanionDirector>(includeInactive: true);
		for (int i = 0; i < m_Group.Length; i++)
		{
			CompanionDirector companionDirector = m_Group[i];
			if ((bool)companionDirector)
			{
				companionDirector.SetSectionID(base.Section.SectionID);
				companionDirector.Initialize();
			}
		}
		yield return null;
	}

	public override void SaveData()
	{
		if (m_Group != null)
		{
			for (int i = 0; i < m_Group.Length; i++)
			{
				m_Group[i].UpdateData();
			}
		}
	}

	protected override void OnDisposed()
	{
		SaveData();
		m_Group = null;
		base.OnDisposed();
	}
}
