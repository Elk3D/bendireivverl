using System;
using UnityEngine;

public class ObjectiveKingWidowDirector : Objective
{
	[SerializeField]
	private KingWidowDirector m_Director;

	[SerializeField]
	private Transform m_StartLocation;

	protected override void InternalInitialize()
	{
		m_Director.OnComplete -= HandleDirectorOnComplete;
		m_Director.OnComplete += HandleDirectorOnComplete;
		m_Director.Initialize(m_StartLocation);
	}

	private void HandleDirectorOnComplete(object sender, EventArgs e)
	{
		m_Director.OnComplete -= HandleDirectorOnComplete;
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_Director.OnComplete -= HandleDirectorOnComplete;
		base.OnDisposed();
	}
}
