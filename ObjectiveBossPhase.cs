using System;
using UnityEngine;

public class ObjectiveBossPhase : Objective
{
	[SerializeField]
	private BossPhaseController m_BossPhaseController;

	protected override void InternalInitialize()
	{
		m_BossPhaseController.OnComplete -= HandleBossPhaseControllerOnComplete;
		m_BossPhaseController.OnComplete += HandleBossPhaseControllerOnComplete;
	}

	private void HandleBossPhaseControllerOnComplete(object sender, EventArgs e)
	{
		m_BossPhaseController.OnComplete -= HandleBossPhaseControllerOnComplete;
		SendOnComplete();
	}
}
