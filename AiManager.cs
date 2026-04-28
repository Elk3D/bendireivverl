using System;
using System.Collections.Generic;
using UnityEngine;

public class AiManager : JMonoBehaviour
{
	[SerializeField]
	private Companion m_CompanionPrefab;

	private List<SectionZone> m_ActiveZones = new List<SectionZone>();

	private SectionZone m_LastKnownZone;

	private float m_TimeInZone;

	private Companion m_Companion;

	private Transform m_CurrentTarget;

	private Transform m_PreviousTarget;

	private bool hasInkDemon;

	private Player m_Player => GameManager.Instance.Player;

	public SectionZone CurrentZone { get; private set; }

	public static AiManager Create()
	{
		AiManager aiManager = new GameObject("[AI MANAGER]").AddComponent<AiManager>();
		aiManager.Initialize();
		return aiManager;
	}

	public void Initialize()
	{
	}

	private void Update()
	{
		if (!hasInkDemon)
		{
			UpdateCompanion();
		}
	}

	private void UpdateInkDemon()
	{
	}

	private void UpdateCompanion()
	{
		if (CurrentZone == null)
		{
			return;
		}
		m_TimeInZone += Time.deltaTime;
		if (m_Companion != null)
		{
			m_Companion.Timer = m_TimeInZone;
		}
		if (!(m_TimeInZone > 5f))
		{
			return;
		}
		m_TimeInZone = 0f;
		if (m_Companion != null)
		{
			Vector3 position = m_Companion.transform.position;
			float num = Vector3.Distance(GameManager.Instance.Player.transform.position, position);
			if (GameManager.Instance.GameCamera.CheckPositionAngle(position) && num > 50f)
			{
				hasInkDemon = true;
				UpdateInkDemon();
				m_Companion.Dispose();
				return;
			}
		}
		if (m_Companion == null)
		{
			for (int i = 0; i < m_ActiveZones.Count; i++)
			{
				if (m_ActiveZones[i] != CurrentZone)
				{
					Vector3 position2 = m_ActiveZones[i].transform.position;
					if (GameManager.Instance.GameCamera.CheckPositionAngle(position2, 25f))
					{
						m_Companion = UnityEngine.Object.Instantiate(m_CompanionPrefab);
						m_Companion.transform.position = position2;
					}
					break;
				}
			}
		}
		_ = m_Companion != null;
	}

	private void HandleCompanionAnimationOnComplete(object sender, EventArgs e)
	{
		m_Companion.OnSpecialAnimationComplete -= HandleCompanionAnimationOnComplete;
		m_Companion.SetNode(m_CurrentTarget.GetComponent<MonoBehaviour>() as CompanionNode);
	}

	private void HandleZoneOnEnter(object sender, EventArgs e)
	{
		SectionZone currentZone = sender as SectionZone;
		CurrentZone = currentZone;
		m_LastKnownZone = CurrentZone;
		m_TimeInZone = 0f;
	}

	private void HandleZoneOnExit(object sender, EventArgs e)
	{
		CurrentZone = null;
		m_TimeInZone = 0f;
	}

	public void AddZones(SectionZone[] zones)
	{
		if (zones == null)
		{
			return;
		}
		for (int i = 0; i < zones.Length; i++)
		{
			if (!m_ActiveZones.Contains(zones[i]))
			{
				m_ActiveZones.Add(zones[i]);
				zones[i].OnEnter += HandleZoneOnEnter;
				zones[i].OnExit += HandleZoneOnExit;
			}
		}
	}

	public void RemoveZones(SectionZone[] zones)
	{
		if (zones == null)
		{
			return;
		}
		for (int num = zones.Length - 1; num > -1; num--)
		{
			if (m_ActiveZones.Contains(zones[num]))
			{
				m_ActiveZones.Remove(zones[num]);
				zones[num].OnEnter -= HandleZoneOnEnter;
				zones[num].OnExit -= HandleZoneOnExit;
			}
		}
	}

	protected override void OnDisposed()
	{
		m_ActiveZones?.Clear();
		m_ActiveZones = null;
		CurrentZone = null;
		m_LastKnownZone = null;
		base.OnDisposed();
	}
}
