using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentPartsController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private GentPartsGroup[] m_Group;

	public SectionID SectionID => m_SectionID;

	public GentPartsGroup[] Group => m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			GentPartsGroup gentPartsGroup = m_Group[i];
			if (gentPartsGroup != null)
			{
				gentPartsGroup.Controller.OnActivate -= HandleControllerOnActivate;
				if ((GentPartsDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentPartsDataObject>(m_SectionID, gentPartsGroup.ID) != null)
				{
					gentPartsGroup.IsComplete = true;
					gentPartsGroup.Controller.Content.ForceActivateComplete();
				}
				else
				{
					(gentPartsGroup.Controller.Content as GentPartsContent).InitializeContent();
					gentPartsGroup.Controller.OnActivate += HandleControllerOnActivate;
				}
			}
			yield return null;
		}
	}

	private void HandleControllerOnActivate(object sender, EventArgs e)
	{
		GentParts gentParts = sender as GentParts;
		gentParts.OnActivate -= HandleControllerOnActivate;
		for (int i = 0; i < m_Group.Length; i++)
		{
			GentPartsGroup gentPartsGroup = m_Group[i];
			if (gentPartsGroup.Controller == gentParts)
			{
				GentPartsDataObject gentPartsDataObject = (GentPartsDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentPartsDataObject>(m_SectionID, gentPartsGroup.ID);
				if (gentPartsDataObject == null)
				{
					gentPartsDataObject = DataObject<int, GentPartsDataObject>.Create(gentPartsGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, gentPartsDataObject);
				}
				gentPartsGroup.IsComplete = true;
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
