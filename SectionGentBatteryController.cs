using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentBatteryController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private GentBatteryGroup[] m_Group;

	public SectionID SectionID => m_SectionID;

	public GentBatteryGroup[] Group => m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			GentBatteryGroup gentBatteryGroup = m_Group[i];
			if (gentBatteryGroup != null)
			{
				gentBatteryGroup.Controller.OnActivate -= HandleControllerOnActivate;
				if ((GentBatteryDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentBatteryDataObject>(m_SectionID, gentBatteryGroup.ID) != null)
				{
					gentBatteryGroup.IsComplete = true;
					gentBatteryGroup.Controller.Content.ForceActivateComplete();
				}
				else
				{
					(gentBatteryGroup.Controller.Content as GentBatteryContent).InitializeContent();
					gentBatteryGroup.Controller.OnActivate += HandleControllerOnActivate;
				}
			}
		}
		yield return null;
	}

	private void HandleControllerOnActivate(object sender, EventArgs e)
	{
		GentBattery gentBattery = sender as GentBattery;
		gentBattery.OnActivate -= HandleControllerOnActivate;
		for (int i = 0; i < m_Group.Length; i++)
		{
			GentBatteryGroup gentBatteryGroup = m_Group[i];
			if (gentBatteryGroup.Controller == gentBattery)
			{
				GentBatteryDataObject gentBatteryDataObject = (GentBatteryDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentBatteryDataObject>(m_SectionID, gentBatteryGroup.ID);
				if (gentBatteryDataObject == null)
				{
					gentBatteryDataObject = DataObject<int, GentBatteryDataObject>.Create(gentBatteryGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, gentBatteryDataObject);
				}
				gentBatteryGroup.IsComplete = true;
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
