using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionSlugController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private SlugGroup[] m_Group;

	public SectionID SectionID => m_SectionID;

	public SlugGroup[] Group => m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			SlugGroup slugGroup = m_Group[i];
			if (slugGroup != null)
			{
				slugGroup.Controller.OnActivate -= HandleControllerOnActivate;
				if ((SlugDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, SlugDataObject>(m_SectionID, slugGroup.ID) != null)
				{
					slugGroup.IsComplete = true;
					slugGroup.Controller.Content.ForceActivateComplete();
				}
				else
				{
					(slugGroup.Controller.Content as SlugContent).InitializeContent();
					slugGroup.Controller.OnActivate += HandleControllerOnActivate;
				}
			}
			yield return null;
		}
	}

	private void HandleControllerOnActivate(object sender, EventArgs e)
	{
		Slug slug = sender as Slug;
		slug.OnActivate -= HandleControllerOnActivate;
		for (int i = 0; i < m_Group.Length; i++)
		{
			SlugGroup slugGroup = m_Group[i];
			if (slugGroup.Controller == slug)
			{
				SlugDataObject slugDataObject = (SlugDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, SlugDataObject>(m_SectionID, slugGroup.ID);
				if (slugDataObject == null)
				{
					slugDataObject = DataObject<int, SlugDataObject>.Create(slugGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, slugDataObject);
				}
				slugGroup.IsComplete = true;
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
