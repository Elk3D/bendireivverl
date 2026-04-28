using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentBatteryCasingController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private GentBatteryCasingGroup[] m_Group;

	public SectionID SectionID => m_SectionID;

	public GentBatteryCasingGroup[] Group => m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			GentBatteryCasingGroup gentBatteryCasingGroup = m_Group[i];
			if (gentBatteryCasingGroup != null)
			{
				gentBatteryCasingGroup.Controller.OnActivate -= HandleControllerOnActivate;
				if ((GentBatteryCasingDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentBatteryCasingDataObject>(m_SectionID, gentBatteryCasingGroup.ID) != null)
				{
					gentBatteryCasingGroup.IsComplete = true;
					gentBatteryCasingGroup.Controller.Content.ForceActivateComplete();
				}
				else
				{
					(gentBatteryCasingGroup.Controller.Content as GentBatteryCasingContent).InitializeContent();
					gentBatteryCasingGroup.Controller.OnActivate += HandleControllerOnActivate;
				}
			}
		}
		yield return null;
	}

	private void HandleControllerOnActivate(object sender, EventArgs e)
	{
		GentBatteryCasing gentBatteryCasing = sender as GentBatteryCasing;
		gentBatteryCasing.OnActivate -= HandleControllerOnActivate;
		for (int i = 0; i < m_Group.Length; i++)
		{
			GentBatteryCasingGroup gentBatteryCasingGroup = m_Group[i];
			if (gentBatteryCasingGroup.Controller == gentBatteryCasing)
			{
				GentBatteryCasingDataObject gentBatteryCasingDataObject = (GentBatteryCasingDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentBatteryCasingDataObject>(m_SectionID, gentBatteryCasingGroup.ID);
				if (gentBatteryCasingDataObject == null)
				{
					gentBatteryCasingDataObject = DataObject<int, GentBatteryCasingDataObject>.Create(gentBatteryCasingGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, gentBatteryCasingDataObject);
				}
				gentBatteryCasingGroup.IsComplete = true;
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
