using UnityEngine;

public class ObjectiveAbility : Objective
{
	[SerializeField]
	private State.PlayerAbility m_AbilityType;

	protected override void InternalEnable()
	{
		Initialize();
	}

	protected override void InternalInitialize()
	{
		GameManager.Instance.Player.SetAbility(m_AbilityType);
		SendOnComplete();
	}
}
