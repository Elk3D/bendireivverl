using System;
using UnityEngine;

public class ObjectiveRequirements : Objective
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	protected override void InternalInitialize()
	{
		JDebug.Log("ObjectiveRequirements :: InternalInitialize", this, JDebug.JDebugType.Objectives);
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		if (!CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
		}
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
			SendOnComplete();
		}
		return flag;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("ObjectiveRequirements :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
		CheckStatus();
	}

	protected override void InternalForceComplete()
	{
		JDebug.Log("ObjectiveRequirements :: InternalForceComplete", this, JDebug.JDebugType.Objectives);
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		base.OnDisposed();
	}
}
