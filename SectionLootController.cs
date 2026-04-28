using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionLootController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private LootGroup[] m_Group;

	private LootDrawerConnector[] m_LootDrawerConnectors;

	private LootConnector[] m_LootConnectors;

	public SectionID SectionID => m_SectionID;

	public LootGroup[] Group => m_Group;

	protected override IEnumerator InternalInitialize()
	{
		m_LootDrawerConnectors = base.transform.GetComponentsInChildren<LootDrawerConnector>(includeInactive: true);
		for (int i = 0; i < m_LootDrawerConnectors.Length; i++)
		{
			m_LootDrawerConnectors[i].Initialize();
		}
		yield return null;
		m_LootConnectors = base.transform.GetComponentsInChildren<LootConnector>(includeInactive: true);
		for (int j = 0; j < m_LootConnectors.Length; j++)
		{
			m_LootConnectors[j].Initialize();
		}
		yield return null;
		for (int k = 0; k < m_Group.Length; k++)
		{
			LootGroup lootGroup = m_Group[k];
			if (lootGroup != null)
			{
				lootGroup.Controller.OnActivate -= HandleControllerOnActivate;
				if ((LootDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, LootDataObject>(m_SectionID, lootGroup.ID) != null)
				{
					lootGroup.IsComplete = true;
					lootGroup.Controller.Content.ForceActivateComplete();
				}
				else
				{
					lootGroup.Controller.OnActivate += HandleControllerOnActivate;
				}
			}
		}
	}

	private void HandleControllerOnActivate(object sender, EventArgs e)
	{
		Loot loot = sender as Loot;
		loot.OnActivate -= HandleControllerOnActivate;
		for (int i = 0; i < m_Group.Length; i++)
		{
			LootGroup lootGroup = m_Group[i];
			if (lootGroup.Controller == loot)
			{
				LootDataObject lootDataObject = (LootDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, LootDataObject>(m_SectionID, lootGroup.ID);
				if (lootDataObject == null)
				{
					lootDataObject = DataObject<int, LootDataObject>.Create(lootGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, lootDataObject);
				}
				lootGroup.IsComplete = true;
				break;
			}
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		if (m_Group != null)
		{
			for (int i = 0; i < m_Group.Length; i++)
			{
				m_Group[i].Controller.OnActivate -= HandleControllerOnActivate;
			}
		}
	}
}
