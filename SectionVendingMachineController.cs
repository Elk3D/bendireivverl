using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionVendingMachineController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private VendingMachineGroup[] m_Group;

	public SectionID SectionID => m_SectionID;

	public VendingMachineGroup[] Group => m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			VendingMachineGroup vendingMachineGroup = m_Group[i];
			if (vendingMachineGroup != null)
			{
				if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Slugs >= 5)
				{
					vendingMachineGroup.Controller.Content.Enable();
				}
				else
				{
					vendingMachineGroup.Controller.Content.Disable();
				}
			}
		}
		yield return null;
	}
}
