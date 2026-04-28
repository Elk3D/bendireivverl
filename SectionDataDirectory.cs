using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SectionDataDirectory : DataDirectory<SectionID, SectionDataObject>
{
	[SerializeField]
	private List<SectionID> m_ActiveSections = new List<SectionID>();

	[SerializeField]
	private List<SectionID> m_InitializedSections = new List<SectionID>();

	public List<SectionID> ActiveSections => m_ActiveSections;

	public List<SectionID> InitializedSections => m_InitializedSections;

	public void Update()
	{
		Section[] allSections = GameManager.Instance.SectionManager.GetAllSections();
		List<SectionID> list = new List<SectionID>();
		List<SectionID> list2 = new List<SectionID>();
		foreach (Section section in allSections)
		{
			if (!(section != null))
			{
				continue;
			}
			SectionID item = (SectionID)section.ID;
			if (!list.Contains(item))
			{
				if (section.IsInitialized)
				{
					list2.Add(item);
				}
				list.Add(item);
			}
		}
		m_ActiveSections = list;
		m_InitializedSections = list2;
	}
}
