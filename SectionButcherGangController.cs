using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionButcherGangController : SectionController
{
	[Header("Timer")]
	[SerializeField]
	private float m_TimerLimitMin = 180f;

	[SerializeField]
	private float m_TimerLimitMax = 360f;

	[SerializeField]
	private float m_ActiveTimerLimit = 60f;

	private ButcherGangController[] m_Group;

	private ButcherGangController m_CurrentButcherGangController;

	private bool IsButcherGangActive;

	private float m_Timer;

	private float m_TimerLimit;

	private float m_ActiveTimer;

	protected override IEnumerator InternalInitialize()
	{
		m_Group = base.transform.GetComponentsInChildren<ButcherGangController>(includeInactive: true);
		if (m_Group != null)
		{
			for (int i = 0; i < m_Group.Length; i++)
			{
				m_Group[i].Initialize();
			}
			AddListeners();
		}
		yield return null;
	}

	private void Update()
	{
		if (GameManager.Instance.Player == null || m_Group == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (IsButcherGangActive)
		{
			if (m_ActiveTimer >= m_ActiveTimerLimit && m_CurrentButcherGangController.ActiveSpawner != null && !m_CurrentButcherGangController.ActiveSpawner.IsSpawned)
			{
				if (GameManager.Instance.Player.CombatStatus != CombatStatus.Hide)
				{
					if (!m_CurrentButcherGangController.AlwaysActive)
					{
						m_CurrentButcherGangController.Disable();
					}
					m_ActiveTimer = 0f;
					ActivateButcherGang();
				}
			}
			else
			{
				m_ActiveTimer += Time.deltaTime;
			}
		}
		else
		{
			if (!IsAvailable())
			{
				return;
			}
			if (m_Timer >= m_TimerLimit)
			{
				if (GameManager.Instance.Player.CombatStatus != CombatStatus.Hide)
				{
					IsButcherGangActive = true;
					ActivateButcherGang();
				}
			}
			else
			{
				m_Timer += Time.deltaTime;
			}
		}
	}

	private bool IsAvailable()
	{
		bool result = false;
		if (m_Group != null)
		{
			for (int i = 0; i < m_Group.Length; i++)
			{
				if (m_Group[i].IsActive)
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}

	private void ActivateButcherGang()
	{
		if (m_Group.Length > 1)
		{
			m_Group.Shuffle();
			for (int i = 0; i < m_Group.Length; i++)
			{
				ButcherGangController butcherGangController = m_Group[i];
				if (butcherGangController.IsActive)
				{
					m_CurrentButcherGangController = butcherGangController;
					m_CurrentButcherGangController.Enable();
					break;
				}
			}
		}
		else if (m_Group != null && m_Group.Length != 0)
		{
			m_CurrentButcherGangController = m_Group[0];
			m_CurrentButcherGangController.Enable();
		}
	}

	private void ResetTimeLimit()
	{
		m_Timer = 0f;
		m_TimerLimit = UnityEngine.Random.Range(m_TimerLimitMin, m_TimerLimitMax);
	}

	private void HandleGroupOnReset(object sender, EventArgs e)
	{
		ResetTimeLimit();
		IsButcherGangActive = false;
	}

	private void AddListeners()
	{
		if (m_Group != null)
		{
			for (int i = 0; i < m_Group.Length; i++)
			{
				ButcherGangController obj = m_Group[i];
				obj.OnReset -= HandleGroupOnReset;
				obj.OnReset += HandleGroupOnReset;
			}
		}
	}

	protected override void RemoveListeners()
	{
		if (m_Group != null)
		{
			for (int i = 0; i < m_Group.Length; i++)
			{
				m_Group[i].OnReset -= HandleGroupOnReset;
			}
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		m_Group = null;
	}
}
