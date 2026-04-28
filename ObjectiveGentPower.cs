using System;
using UnityEngine;

public class ObjectiveGentPower : Objective
{
	[Header("Gent Power")]
	[SerializeField]
	private GentPower m_GentPower;

	protected override void InternalInitialize()
	{
		JDebug.Log("GentPower :: InternalInitialize", this, JDebug.JDebugType.Objectives);
		m_GentPower.OnComplete -= HandleCutsceneOnComplete;
		m_GentPower.OnComplete += HandleCutsceneOnComplete;
		if (m_GentPower.Content.Activator.ActivateType == CutsceneActivateType.Callback)
		{
			m_GentPower.Play();
		}
	}

	private void HandleCutsceneOnComplete(object sender, EventArgs e)
	{
		JDebug.Log("GentPower :: HandleCutsceneOnComplete", this, JDebug.JDebugType.Objectives);
		m_GentPower.OnComplete -= HandleCutsceneOnComplete;
		SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		JDebug.Log("GentPower :: InternalForceComplete", this, JDebug.JDebugType.Objectives);
		RemoveListeners();
		SendOnComplete();
	}

	protected override void InternalComplete()
	{
		JDebug.Log("GentPower :: InternalComplete", this, JDebug.JDebugType.Objectives);
		RemoveListeners();
	}

	protected override void RemoveListeners()
	{
		if (m_GentPower != null)
		{
			m_GentPower.OnComplete -= HandleCutsceneOnComplete;
		}
	}
}
