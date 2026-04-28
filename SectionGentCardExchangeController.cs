using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentCardExchangeController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private GentCardExchangeGroup[] m_Group;

	public SectionID SectionID => m_SectionID;

	public GentCardExchangeGroup[] Group => m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			m_Group[i]?.Controller.Content.Enable();
		}
		yield return null;
	}
}
