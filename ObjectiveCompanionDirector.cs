using UnityEngine;

public class ObjectiveCompanionDirector : Objective
{
	[SerializeField]
	private CompanionDirector m_CompanionDirector;

	[SerializeField]
	private Transform m_StartLocation;

	[SerializeField]
	private bool m_DisposeCompanionOnUnload;

	protected override void InternalInitialize()
	{
		m_CompanionDirector.Initialize(m_StartLocation);
	}

	protected override void InternalDisable()
	{
		m_CompanionDirector.UnloadData(m_DisposeCompanionOnUnload);
	}
}
