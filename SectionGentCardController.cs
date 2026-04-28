using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentCardController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private GentCardGroup[] m_Group;

	public SectionID SectionID => m_SectionID;

	public GentCardGroup[] Group => m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			GentCardGroup gentCardGroup = m_Group[i];
			if (gentCardGroup != null)
			{
				gentCardGroup.Controller.OnActivate -= HandleControllerOnActivate;
				if ((GentCardDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentCardDataObject>(m_SectionID, gentCardGroup.ID) != null)
				{
					gentCardGroup.IsComplete = true;
					gentCardGroup.Controller.Content.ForceActivateComplete();
				}
				else
				{
					(gentCardGroup.Controller.Content as GentCardContent).InitializeContent();
					gentCardGroup.Controller.OnActivate += HandleControllerOnActivate;
				}
			}
			yield return null;
		}
	}

	private void HandleControllerOnActivate(object sender, EventArgs e)
	{
		GentCard gentCard = sender as GentCard;
		gentCard.OnActivate -= HandleControllerOnActivate;
		for (int i = 0; i < m_Group.Length; i++)
		{
			GentCardGroup gentCardGroup = m_Group[i];
			if (gentCardGroup.Controller == gentCard)
			{
				GentCardDataObject gentCardDataObject = (GentCardDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentCardDataObject>(m_SectionID, gentCardGroup.ID);
				if (gentCardDataObject == null)
				{
					gentCardDataObject = DataObject<int, GentCardDataObject>.Create(gentCardGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, gentCardDataObject);
				}
				gentCardGroup.IsComplete = true;
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
