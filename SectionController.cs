using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionController : Controller
{
	private Section m_Section;

	public Section Section => m_Section;

	public void SetSection(Section section)
	{
		m_Section = section;
	}

	public virtual void SaveData()
	{
	}
}
