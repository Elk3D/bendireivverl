using System.Collections;

public class SectionCharacterDirectorController : SectionController
{
	private CharacterDirector[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		m_Group = GetComponentsInChildren<CharacterDirector>(includeInactive: true);
		for (int i = 0; i < m_Group.Length; i++)
		{
			CharacterDirector characterDirector = m_Group[i];
			characterDirector.SetSectionID(base.Section.SectionID);
			if (characterDirector.OnStart)
			{
				characterDirector.Initialize();
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
