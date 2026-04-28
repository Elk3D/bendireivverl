using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionDoorSwitchPanelController : SectionController
{
	private DoorSwitchPanel[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		m_Group = base.transform.GetComponentsInChildren<DoorSwitchPanel>(includeInactive: true);
		for (int i = 0; i < m_Group.Length; i++)
		{
			m_Group[i].Initialize();
		}
		yield return null;
	}
}
