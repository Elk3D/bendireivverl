using UnityEngine;

public class ObjectiveCombatStatus : Objective
{
	[SerializeField]
	private CombatStatus m_CombatStatus;

	protected override void InternalEnable()
	{
		Initialize();
	}

	protected override void InternalInitialize()
	{
		GameManager.Instance.Player.SetCombatStatus(m_CombatStatus);
	}

	protected override void InternalForceComplete()
	{
		Enable();
		SendOnComplete();
	}
}
