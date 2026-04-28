using UnityEngine;

public class ObjectiveSignalTower : Objective
{
	[SerializeField]
	private SignalTower m_SignalTower;

	[SerializeField]
	private bool m_TurnOff = true;

	[SerializeField]
	private bool m_OnForceComplete = true;

	protected override void InternalEnable()
	{
		Initialize();
	}

	protected override void InternalInitialize()
	{
		m_SignalTower.TurnOn();
		SendOnComplete();
	}

	protected override void InternalDisable()
	{
		Inactive();
	}

	protected override void InternalInactive()
	{
		m_SignalTower.TurnOff();
		SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		if (m_OnForceComplete)
		{
			if (m_TurnOff)
			{
				m_SignalTower.TurnOff();
			}
			else
			{
				m_SignalTower.TurnOn();
			}
		}
		SendOnComplete();
	}
}
