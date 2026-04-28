using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionZoneController : SectionController
{
	private SectionZone[] m_SectionZones;

	private SectionZone m_CurrentZone;

	private SectionZone m_EnterZone;

	private SectionZone m_ExitZone;

	protected override IEnumerator InternalInitialize()
	{
		m_SectionZones = GetComponentsInChildren<SectionZone>(includeInactive: true);
		RemoveListeners();
		AddListeners();
		yield return null;
	}

	private void HandleSectionZoneOnEnter(object sender, EventArgs e)
	{
		SectionZone sectionZone = (SectionZone)sender;
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.SetSectionID(base.Section.SectionID);
		}
		if (m_CurrentZone == sectionZone)
		{
			return;
		}
		m_EnterZone = sectionZone;
		if (m_EnterZone != m_ExitZone)
		{
			m_CurrentZone = m_EnterZone;
			if (GameManager.Instance.Player != null)
			{
				GameManager.Instance.Player.SetZone(m_CurrentZone.Name);
			}
		}
	}

	private void HandleSectionZoneOnExit(object sender, EventArgs e)
	{
		SectionZone exitZone = (SectionZone)sender;
		m_ExitZone = exitZone;
		if (m_ExitZone != m_EnterZone)
		{
			m_CurrentZone = m_EnterZone;
			if (GameManager.Instance.Player != null)
			{
				GameManager.Instance.Player.SetZone(m_CurrentZone.Name);
			}
		}
	}

	private void AddListeners()
	{
		if (m_SectionZones != null)
		{
			for (int i = 0; i < m_SectionZones.Length; i++)
			{
				SectionZone obj = m_SectionZones[i];
				obj.OnEnter += HandleSectionZoneOnEnter;
				obj.OnExit += HandleSectionZoneOnExit;
				obj.Initialize();
			}
		}
	}

	protected override void RemoveListeners()
	{
		if (m_SectionZones != null)
		{
			for (int i = 0; i < m_SectionZones.Length; i++)
			{
				SectionZone obj = m_SectionZones[i];
				obj.OnEnter -= HandleSectionZoneOnEnter;
				obj.OnExit -= HandleSectionZoneOnExit;
			}
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		m_SectionZones = null;
		m_CurrentZone = null;
		m_EnterZone = null;
		m_ExitZone = null;
	}
}
