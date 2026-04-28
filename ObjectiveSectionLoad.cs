using System;
using UnityEngine;

public class ObjectiveSectionLoad : Objective
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private bool m_Unload;

	[SerializeField]
	private bool m_ShowLoader;

	[SerializeField]
	private bool m_UnloadAllExcept;

	protected override void InternalInitialize()
	{
		if (m_SectionID != SectionID.NONE)
		{
			if (m_UnloadAllExcept)
			{
				Section[] allSections = GameManager.Instance.SectionManager.GetAllSections();
				if (allSections.Length != 0)
				{
					for (int num = allSections.Length - 1; num >= 0; num--)
					{
						Section section = allSections[num];
						if (section.SectionID != m_SectionID)
						{
							GameManager.Instance.SectionManager.Remove(section.SectionID);
						}
					}
				}
				SendOnComplete();
			}
			else if (!GameManager.Instance.SectionManager.Contains(m_SectionID))
			{
				if (!m_Unload)
				{
					GameManager.Instance.SectionManager.OnLoaded += HandleSectionOnLoaded;
					GameManager.Instance.SectionManager.LoadSectionAsync(m_SectionID);
					if (m_ShowLoader)
					{
						GameManager.Instance.ShowAsyncLoader();
					}
				}
				else
				{
					SendOnComplete();
				}
			}
			else
			{
				GameManager.Instance.SectionManager.Remove(m_SectionID);
				SendOnComplete();
			}
		}
		else
		{
			SendOnComplete();
		}
	}

	private void HandleSectionOnLoaded(object sender, EventArgs e)
	{
		if ((sender as Section).SectionID == m_SectionID)
		{
			GameManager.Instance.SectionManager.OnLoaded -= HandleSectionOnLoaded;
			if (m_ShowLoader)
			{
				GameManager.Instance.CompleteAsyncLoader();
			}
			SendOnComplete();
		}
	}
}
