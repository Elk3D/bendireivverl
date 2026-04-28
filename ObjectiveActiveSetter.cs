using UnityEngine;

public class ObjectiveActiveSetter : Objective
{
	[SerializeField]
	private ActiveSetter m_ActiveSetter;

	[SerializeField]
	private bool m_ForceComplete = true;

	[SerializeField]
	private bool m_ActiveOnComplete;

	protected override void InternalInitialize()
	{
		SetActive();
	}

	protected override void InternalEnable()
	{
		m_ActiveSetter?.SetActive(active: true);
	}

	protected override void InternalDisable()
	{
		m_ActiveSetter?.SetActive(active: false);
	}

	private void SetActive()
	{
		m_ActiveSetter?.SetActive(!m_ActiveSetter.IsActive);
	}

	protected override void InternalForceComplete()
	{
		if (m_ForceComplete)
		{
			m_ActiveSetter.ForceComplete();
		}
		else if (m_ActiveOnComplete)
		{
			m_ActiveSetter.SetActive(active: true);
		}
		SendOnComplete();
	}
}
