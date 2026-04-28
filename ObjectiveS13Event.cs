using S13Audio;
using UnityEngine;

public class ObjectiveS13Event : Objective
{
	[SerializeField]
	private S13ScriptableEvent m_ScriptableEvent;

	protected override void InternalInitialize()
	{
		m_ScriptableEvent?.Raise();
		SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		m_ScriptableEvent?.Raise();
	}
}
