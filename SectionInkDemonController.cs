using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionInkDemonController : SectionController
{
	[SerializeField]
	private Requirements m_Requirements;

	public bool IsAvailable { get; private set; }

	protected override IEnumerator InternalInitialize()
	{
		if (m_Requirements != null)
		{
			if (!CheckStatus())
			{
				GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
				GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
				if (GameManager.Instance.InkDemonManager == null)
				{
					GameManager.Instance.InkDemonManager = InkDemonManager.Create();
				}
				GameManager.Instance.InkDemonManager.CheckAvailability();
			}
		}
		else
		{
			IsAvailable = false;
			if (GameManager.Instance.InkDemonManager == null)
			{
				GameManager.Instance.InkDemonManager = InkDemonManager.Create();
			}
			GameManager.Instance.InkDemonManager.CheckAvailability();
		}
		yield return null;
	}

	private void AllowInkDemonSpawning()
	{
		IsAvailable = true;
		if (GameManager.Instance.InkDemonManager == null)
		{
			GameManager.Instance.InkDemonManager = InkDemonManager.Create();
		}
		if (GameManager.Instance.InkDemonManager.Timer >= GameManager.Instance.InkDemonManager.TimerLimit)
		{
			GameManager.Instance.InkDemonManager.ResetTimer();
		}
		GameManager.Instance.InkDemonManager.CheckAvailability();
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("SectionInkDemonController :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
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
			AllowInkDemonSpawning();
		}
		return flag;
	}

	protected override void RemoveListeners()
	{
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
