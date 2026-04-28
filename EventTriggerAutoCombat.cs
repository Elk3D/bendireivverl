using UnityEngine;

public class EventTriggerAutoCombat : EventTrigger
{
	[Header("Enemies")]
	[SerializeField]
	private Enemy[] m_Enemies;

	protected override void OnInternalEnter(Collider col)
	{
		if (m_Enemies != null && m_Enemies.Length != 0)
		{
			GameManager.Instance.Player.SetCombatStatus(CombatStatus.Combat);
			for (int i = 0; i < m_Enemies.Length; i++)
			{
				Enemy obj = m_Enemies[i];
				obj.SetTarget(GameManager.Instance.Player.transform);
				obj.SetState(State.Character.Follow);
			}
		}
	}

	protected override void OnInternalExit(Collider col)
	{
		ResetAction();
	}
}
