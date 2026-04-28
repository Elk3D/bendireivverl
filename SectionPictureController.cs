using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionPictureController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private PictureGroup[] m_Group;

	public SectionID SectionID => m_SectionID;

	public PictureGroup[] Group => m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			PictureGroup pictureGroup = m_Group[i];
			if (pictureGroup != null)
			{
				pictureGroup.Controller.OnActivate -= HandleControllerOnActivate;
				if ((PictureDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, PictureDataObject>(m_SectionID, pictureGroup.ID) != null)
				{
					pictureGroup.IsComplete = true;
					pictureGroup.Controller.Content.ForceActivateComplete();
				}
				else
				{
					(pictureGroup.Controller.Content as PictureContent).InitializeContent();
					pictureGroup.Controller.OnActivate += HandleControllerOnActivate;
				}
			}
			yield return null;
		}
	}

	private void HandleControllerOnActivate(object sender, EventArgs e)
	{
		Picture picture = sender as Picture;
		picture.OnActivate -= HandleControllerOnActivate;
		for (int i = 0; i < m_Group.Length; i++)
		{
			PictureGroup pictureGroup = m_Group[i];
			if (pictureGroup.Controller == picture)
			{
				PictureDataObject pictureDataObject = (PictureDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, PictureDataObject>(m_SectionID, pictureGroup.ID);
				if (pictureDataObject == null)
				{
					pictureDataObject = DataObject<int, PictureDataObject>.Create(pictureGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, pictureDataObject);
				}
				pictureGroup.IsComplete = true;
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
