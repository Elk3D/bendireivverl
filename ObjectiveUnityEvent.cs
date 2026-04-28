using UnityEngine;
using UnityEngine.Events;

public class ObjectiveUnityEvent : Objective
{
	[Header("Force Complete")]
	[SerializeField]
	private bool m_ForceComplete = true;

	[Header("Unity Events")]
	[SerializeField]
	private UnityEvent m_UnityEvent;

	protected override void InternalInitialize()
	{
		m_UnityEvent?.Invoke();
		SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		if (m_ForceComplete)
		{
			Initialize();
		}
		else
		{
			SendOnComplete();
		}
	}
}
