using System;
using UnityEngine;

public class ObjectiveHambush : Objective
{
	[SerializeField]
	private HambushController m_HambushController;

	protected override void InternalInitialize()
	{
		m_HambushController.OnComplete -= HandleHambushControllerOnComplete;
		m_HambushController.OnComplete += HandleHambushControllerOnComplete;
		m_HambushController.Initialize();
	}

	private void HandleHambushControllerOnComplete(object sender, EventArgs e)
	{
		m_HambushController.OnComplete -= HandleHambushControllerOnComplete;
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_HambushController.OnComplete -= HandleHambushControllerOnComplete;
		base.OnDisposed();
	}
}
