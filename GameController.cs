using System;
using UnityEngine;

public class GameController : JMonoBehaviour
{
	[SerializeField]
	private SectionSoftLoaderGroup[] m_Group;

	private Bounds m_Bounds;

	public override void Start()
	{
		GameManager.Instance.GameController = this;
		m_Bounds = new Bounds(Vector3.zero, Vector3.one * 5000f);
	}

	public void Initialize()
	{
		AddListeners();
	}

	private void Update()
	{
		if (!base.IsDisposed && !GameManager.Instance.IsPaused && GameManager.Instance.Player != null)
		{
			_ = m_Bounds;
			if (!m_Bounds.Contains(GameManager.Instance.Player.transform.position))
			{
				GameManager.Instance.Player.ForceDeath();
			}
		}
	}

	private void HandleGroupOnLoad(object sender, EventArgs e)
	{
		SectionSoftLoaderGroup sectionSoftLoaderGroup = sender as SectionSoftLoaderGroup;
		GameManager.Instance.SectionManager.LoadSectionAsync(sectionSoftLoaderGroup.ID, initialize: false);
	}

	private void HandleGroupOnUnload(object sender, EventArgs e)
	{
		SectionSoftLoaderGroup sectionSoftLoaderGroup = sender as SectionSoftLoaderGroup;
		Section section = GameManager.Instance.SectionManager.GetSection(sectionSoftLoaderGroup.ID);
		if (section != null)
		{
			UnloadSection(section);
		}
	}

	private void UnloadSection(Section section)
	{
		GameManager.Instance.SectionManager.Remove(section.SectionID);
		if (GameManager.Instance.InkDemonManager != null)
		{
			GameManager.Instance.InkDemonManager.CheckAvailability();
		}
	}

	private void AddListeners()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			SectionSoftLoaderGroup obj = m_Group[i];
			obj.OnLoad -= HandleGroupOnLoad;
			obj.OnLoad += HandleGroupOnLoad;
			obj.OnUnload -= HandleGroupOnUnload;
			obj.OnUnload += HandleGroupOnUnload;
			obj.Initialize();
		}
	}

	protected void RemoveListeners()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			SectionSoftLoaderGroup obj = m_Group[i];
			obj.OnLoad -= HandleGroupOnLoad;
			obj.OnUnload -= HandleGroupOnUnload;
			obj.RemoveListeners();
		}
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		base.OnDisposed();
	}
}
