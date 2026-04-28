using UnityEngine;

public class ObjectiveLadder : Objective
{
	[SerializeField]
	private Ladder m_Ladder;

	protected override void InternalInitialize()
	{
		Enable();
	}

	protected override void InternalEnable()
	{
		m_Ladder.Content.Enable();
	}

	protected override void InternalInactive()
	{
		Disable();
	}

	protected override void InternalDisable()
	{
		m_Ladder.Content.Disable();
	}

	protected override void InternalForceComplete()
	{
		m_Ladder.Content.ForceActivateComplete();
		SendOnComplete();
	}
}
