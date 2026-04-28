using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionHideController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private InteractableHideGroup[] m_Group;

	public SectionID SectionID => m_SectionID;

	public InteractableHideGroup[] Group => m_Group;

	protected override IEnumerator InternalInitialize()
	{
		int num = -1;
		for (int i = 0; i < m_Group.Length; i++)
		{
			InteractableHideGroup interactableHideGroup = m_Group[i];
			HideDataObject hideDataObject = (HideDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, HideDataObject>(m_SectionID, interactableHideGroup.ID);
			if (hideDataObject == null)
			{
				hideDataObject = DataObject<int, HideDataObject>.Create(interactableHideGroup.ID);
				GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, hideDataObject);
			}
			else if (hideDataObject.HideState == 1)
			{
				num = hideDataObject.ID;
			}
			interactableHideGroup.Controller.OnEnter -= HandleInteractableHideOnEnter;
			interactableHideGroup.Controller.OnEnter += HandleInteractableHideOnEnter;
			interactableHideGroup.Controller.OnExit -= HandleInteractableHideOnExit;
			interactableHideGroup.Controller.OnExit += HandleInteractableHideOnExit;
			interactableHideGroup.Controller.Initialize();
			if (interactableHideGroup.ID == num && GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.CombatStatus == CombatStatus.Hide)
			{
				interactableHideGroup.Controller.ForceEnter(hideDataObject);
			}
		}
		yield return null;
	}

	private void HandleInteractableHideOnEnter(object sender, EventArgs e)
	{
		InteractableHide interactableHide = sender as InteractableHide;
		HideDataObject hideDataObject = (HideDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, HideDataObject>(m_SectionID, interactableHide.ID);
		if (hideDataObject == null)
		{
			hideDataObject = DataObject<int, HideDataObject>.Create(interactableHide.ID);
			GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, hideDataObject);
		}
		hideDataObject.SetHideState(isHiding: true);
	}

	private void HandleInteractableHideOnExit(object sender, EventArgs e)
	{
		InteractableHide interactableHide = sender as InteractableHide;
		((HideDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, HideDataObject>(m_SectionID, interactableHide.ID))?.SetHideState(isHiding: false);
	}

	protected override void RemoveListeners()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			InteractableHideGroup obj = m_Group[i];
			obj.Controller.OnEnter -= HandleInteractableHideOnEnter;
			obj.Controller.OnExit -= HandleInteractableHideOnExit;
		}
	}

	public override void SaveData()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			InteractableHideGroup interactableHideGroup = m_Group[i];
			HideDataObject hideDataObject = (HideDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, HideDataObject>(m_SectionID, interactableHideGroup.ID);
			if (hideDataObject != null && hideDataObject.HideState == 1)
			{
				hideDataObject.SetRotationX(GameManager.Instance.Player.AnimationContainer.localRotation);
				hideDataObject.SetRotationY(GameManager.Instance.Player.HeadContainer.localRotation);
			}
		}
	}
}
