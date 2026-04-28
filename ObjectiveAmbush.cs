using UnityEngine;

public class ObjectiveAmbush : Objective
{
	[Header("Enemy Controller")]
	[SerializeField]
	private SectionEnemyController m_SectionEnemyController;

	[Header("Enemey")]
	[SerializeField]
	private EnemySelector m_EnemySelector;

	[SerializeField]
	private CharacterNode m_CharacterNode;

	[Header("Options")]
	[SerializeField]
	private bool m_IsAmbush = true;

	protected override void InternalInitialize()
	{
		Enemy enemy = m_SectionEnemyController.Spawn(m_EnemySelector, m_CharacterNode, getNewNode: true, m_IsAmbush);
		enemy.SetSection(m_SectionEnemyController.Section.SectionID);
		enemy.Initialize();
		enemy.AddData();
		SendOnComplete();
	}
}
