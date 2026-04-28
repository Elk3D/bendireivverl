using System;
using System.Collections;
using UnityEngine;

public class SectionLoaderController : SectionController
{
	[Header("Section")]
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private SectionLoaderGroup[] m_Group;

	public SectionID SectionID => m_SectionID;

	protected override IEnumerator InternalInitialize()
	{
		AddListeners();
		yield return null;
	}

	private void HandleGroupOnLoad(object sender, EventArgs e)
	{
		SectionLoaderGroup sectionLoaderGroup = sender as SectionLoaderGroup;
		GameManager.Instance.SectionManager.InitializeSection(sectionLoaderGroup.ID);
	}

	private void HandleGroupOnUnload(object sender, EventArgs e)
	{
		SectionLoaderGroup sectionLoaderGroup = sender as SectionLoaderGroup;
		Section section = GameManager.Instance.SectionManager.GetSection(sectionLoaderGroup.ID);
		if (section != null)
		{
			section.SetActive(active: false);
			if (GameManager.Instance.InkDemonManager != null)
			{
				GameManager.Instance.InkDemonManager.CheckAvailability();
			}
		}
	}

	private void AddListeners()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			SectionLoaderGroup obj = m_Group[i];
			obj.OnLoad -= HandleGroupOnLoad;
			obj.OnLoad += HandleGroupOnLoad;
			obj.OnUnload -= HandleGroupOnUnload;
			obj.OnUnload += HandleGroupOnUnload;
			obj.Initialize();
		}
	}

	protected override void RemoveListeners()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			SectionLoaderGroup obj = m_Group[i];
			obj.OnLoad -= HandleGroupOnLoad;
			obj.OnUnload -= HandleGroupOnUnload;
			obj.RemoveListeners();
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		if (m_Group != null)
		{
			for (int i = 0; i < m_Group.Length; i++)
			{
				m_Group[i].Clear();
			}
		}
	}
}
