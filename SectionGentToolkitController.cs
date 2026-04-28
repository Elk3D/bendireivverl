using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentToolkitController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private GentToolkitGroup[] m_Group;

	public SectionID SectionID => m_SectionID;

	public GentToolkitGroup[] Group => m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			GentToolkitGroup gentToolkitGroup = m_Group[i];
			if (gentToolkitGroup != null)
			{
				gentToolkitGroup.Controller.OnActivate -= HandleControllerOnActivate;
				if ((GentToolkitDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentToolkitDataObject>(m_SectionID, gentToolkitGroup.ID) != null)
				{
					gentToolkitGroup.IsComplete = true;
					gentToolkitGroup.Controller.Content.ForceActivateComplete();
				}
				else
				{
					(gentToolkitGroup.Controller.Content as GentToolkitContent).InitializeContent();
					gentToolkitGroup.Controller.OnActivate += HandleControllerOnActivate;
				}
			}
			yield return null;
		}
	}

	private void HandleControllerOnActivate(object sender, EventArgs e)
	{
		GentToolkit gentToolkit = sender as GentToolkit;
		gentToolkit.OnActivate -= HandleControllerOnActivate;
		for (int i = 0; i < m_Group.Length; i++)
		{
			GentToolkitGroup gentToolkitGroup = m_Group[i];
			if (gentToolkitGroup.Controller == gentToolkit)
			{
				GentToolkitDataObject gentToolkitDataObject = (GentToolkitDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentToolkitDataObject>(m_SectionID, gentToolkitGroup.ID);
				if (gentToolkitDataObject == null)
				{
					gentToolkitDataObject = DataObject<int, GentToolkitDataObject>.Create(gentToolkitGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, gentToolkitDataObject);
				}
				gentToolkitGroup.IsComplete = true;
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
