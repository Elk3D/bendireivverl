using System;
using UnityEngine;

public class ObjectivePiano : Objective
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Piano")]
	[SerializeField]
	private Piano m_Piano;

	[SerializeField]
	private bool m_IsSuccess;

	protected override void InternalEnable()
	{
		Initialize();
	}

	protected override void InternalInitialize()
	{
		RemoveListeners();
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		if (!CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
		}
	}

	private void HandlePiano(object sender, EventArgs e)
	{
		SendOnComplete();
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		CheckStatus();
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
			if (m_IsSuccess)
			{
				m_Piano.OnSuccess += HandlePiano;
			}
			else
			{
				m_Piano.OnFail += HandlePiano;
			}
			m_Piano.Initialize();
		}
		return flag;
	}

	protected override void InternalForceComplete()
	{
		RemoveListeners();
		SendOnComplete();
	}

	protected override void InternalComplete()
	{
		RemoveListeners();
	}

	protected override void RemoveListeners()
	{
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		m_Piano.OnSuccess -= HandlePiano;
		m_Piano.OnFail -= HandlePiano;
	}
}
