using UnityEngine;

public class EventTriggerActiveSetter : EventTrigger
{
	[SerializeField]
	private ActiveSetter m_ActiveSetter;

	[SerializeField]
	private bool m_OnEnter = true;

	[SerializeField]
	private bool m_OnExit = true;

	protected override void OnInternalEnter(Collider col)
	{
		if (m_OnEnter)
		{
			m_ActiveSetter.SetActiveFalse();
		}
	}

	protected override void OnInternalExit(Collider col)
	{
		if (m_OnExit)
		{
			m_ActiveSetter.SetActiveTrue();
		}
	}
}
