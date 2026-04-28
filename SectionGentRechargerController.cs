using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentRechargerController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private GentRechargerGroup[] m_Group;

	public SectionID SectionID => m_SectionID;

	public GentRechargerGroup[] Group => m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			GentRechargerGroup gentRechargerGroup = m_Group[i];
			if (gentRechargerGroup != null)
			{
				GentRechargerDataObject gentRechargerDataObject = (GentRechargerDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentRechargerDataObject>(m_SectionID, gentRechargerGroup.ID);
				if (gentRechargerDataObject != null)
				{
					gentRechargerGroup.Controller.Initialize(gentRechargerDataObject.Batteries);
				}
				else
				{
					gentRechargerGroup.Controller.Initialize(0);
				}
				gentRechargerGroup.Controller.OnDeposit -= HandleControllerOnChanged;
				gentRechargerGroup.Controller.OnCharged -= HandleControllerOnChanged;
				gentRechargerGroup.Controller.OnDeposit += HandleControllerOnChanged;
				gentRechargerGroup.Controller.OnCharged += HandleControllerOnChanged;
			}
		}
		yield return null;
	}

	private void HandleControllerOnChanged(object sender, EventArgs e)
	{
		GentRecharger gentRecharger = sender as GentRecharger;
		for (int i = 0; i < m_Group.Length; i++)
		{
			GentRechargerGroup gentRechargerGroup = m_Group[i];
			if (gentRechargerGroup.Controller == gentRecharger)
			{
				GentRechargerDataObject gentRechargerDataObject = (GentRechargerDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentRechargerDataObject>(m_SectionID, gentRechargerGroup.ID);
				if (gentRechargerDataObject != null)
				{
					gentRechargerDataObject.SetBatteries(gentRecharger.BatteryCount);
					break;
				}
				gentRechargerDataObject = DataObject<int, GentRechargerDataObject>.Create(gentRechargerGroup.ID);
				gentRechargerDataObject.SetBatteries(gentRecharger.BatteryCount);
				GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, gentRechargerDataObject);
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
				GentRechargerGroup obj = m_Group[i];
				obj.Controller.OnDeposit -= HandleControllerOnChanged;
				obj.Controller.OnCharged -= HandleControllerOnChanged;
			}
		}
	}
}
