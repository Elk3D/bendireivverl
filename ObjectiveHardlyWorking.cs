using System;
using UnityEngine;

public class ObjectiveHardlyWorking : Objective
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Settings")]
	[SerializeField]
	private float m_Length = 900f;

	private float m_Time;

	private bool m_IsActivated;

	protected override void InternalUpdate()
	{
		if (m_IsActivated)
		{
			if (m_Time >= m_Length)
			{
				Unlock();
			}
			else
			{
				m_Time += Time.deltaTime;
			}
		}
	}

	protected override void InternalInitialize()
	{
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		if (!CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
		}
	}

	private void Unlock()
	{
		GameManager.Instance.LockPause();
		Deactivate();
		SendOnComplete();
	}

	protected override void InternalDisable()
	{
		Deactivate();
	}

	protected override void InternalComplete()
	{
		Deactivate();
	}

	protected override void InternalForceComplete()
	{
		Deactivate();
	}

	private void Deactivate()
	{
		m_IsActivated = false;
		m_Time = 0f;
	}

	private bool CheckStatus()
	{
		bool flag = true;
		if (m_Requirements != null)
		{
			flag = m_Requirements.IsComplete();
		}
		if (flag)
		{
			GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
			if (!m_IsActivated)
			{
				m_IsActivated = true;
				m_Time = 0f;
			}
		}
		return flag;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		CheckStatus();
	}
}
