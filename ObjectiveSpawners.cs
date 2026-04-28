using UnityEngine;

public class ObjectiveSpawners : Objective
{
	[SerializeField]
	private bool m_InstantSpawn;

	[SerializeField]
	private Spawner[] m_Spawners;

	protected override void InternalInitialize()
	{
		Activate();
	}

	protected override void InternalEnable()
	{
		Activate();
	}

	private void Activate()
	{
		for (int i = 0; i < m_Spawners.Length; i++)
		{
			if (m_InstantSpawn)
			{
				m_Spawners[i].Spawn();
			}
			else
			{
				m_Spawners[i].Activate();
			}
		}
	}

	protected override void InternalDisable()
	{
		Deactivate();
	}

	protected override void InternalInactive()
	{
		Deactivate();
	}

	private void Deactivate()
	{
		for (int i = 0; i < m_Spawners.Length; i++)
		{
			m_Spawners[i].Deactivate();
		}
	}
}
