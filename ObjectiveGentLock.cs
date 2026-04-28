using System;
using UnityEngine;

public class ObjectiveGentLock : Objective
{
	[Header("Gent Lock")]
	[SerializeField]
	private GentLock m_GentLock;

	protected override void InternalInitialize()
	{
		m_GentLock.OnComplete -= HandleCutsceneOnComplete;
		m_GentLock.OnComplete += HandleCutsceneOnComplete;
		if (m_GentLock.Content != null && m_GentLock.Content.Activator != null && m_GentLock.Content.Activator.ActivateType == CutsceneActivateType.Callback)
		{
			m_GentLock.Play();
		}
	}

	private void HandleCutsceneOnComplete(object sender, EventArgs e)
	{
		m_GentLock.OnComplete -= HandleCutsceneOnComplete;
		SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		RemoveListeners();
	}

	protected override void InternalComplete()
	{
		RemoveListeners();
	}

	protected override void RemoveListeners()
	{
		if (m_GentLock != null)
		{
			m_GentLock.OnComplete -= HandleCutsceneOnComplete;
		}
	}
}
