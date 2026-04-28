using System;
using UnityEngine;
using UnityEngine.AI;

public class ObjectiveEnemySpawner : Objective
{
	[Serializable]
	public class EnemySpawnerGroup
	{
		public Enemy Prefab;

		public Transform Location;

		public bool TargetPlayer;
	}

	[SerializeField]
	private EnemySpawnerGroup[] m_EnemySpawnerGroup;

	protected override void InternalInitialize()
	{
		if (m_EnemySpawnerGroup != null && m_EnemySpawnerGroup.Length != 0)
		{
			for (int i = 0; i < m_EnemySpawnerGroup.Length; i++)
			{
				EnemySpawnerGroup enemySpawnerGroup = m_EnemySpawnerGroup[i];
				if (enemySpawnerGroup.Prefab != null && enemySpawnerGroup.Location != null)
				{
					Enemy enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>(enemySpawnerGroup.Prefab);
					enemy.SetSection(GetComponentInParent<Section>().SectionID);
					enemy.transform.position = enemySpawnerGroup.Location.position;
					enemy.transform.eulerAngles = enemySpawnerGroup.Location.eulerAngles;
					if (enemySpawnerGroup.TargetPlayer)
					{
						enemy.OnInitializeOnComplete -= HandleEnemyOnINitializeOnComplete;
						enemy.OnInitializeOnComplete += HandleEnemyOnINitializeOnComplete;
					}
				}
			}
		}
		SendOnComplete();
	}

	private void HandleEnemyOnINitializeOnComplete(object sender, EventArgs e)
	{
		Enemy obj = sender as Enemy;
		obj.OnInitializeOnComplete -= HandleEnemyOnINitializeOnComplete;
		obj.SetTarget(GameManager.Instance.Player.transform);
		obj.SetState(State.Character.Follow);
		obj.Agent.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
		obj.Agent.ResetAgent();
	}
}
