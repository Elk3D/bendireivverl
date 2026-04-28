using System;
using System.Collections.Generic;
using UnityEngine;

public class DummySpawner : JMonoBehaviour
{
	[SerializeField]
	private BoxCollider m_Bounds;

	[Header("Prefabs")]
	[SerializeField]
	private Dummy[] m_DummyPrefabs;

	private CharacterControllable m_BeastBendy;

	private float m_SpawnTimer;

	private float m_SpawnTimerLimit = 2f;

	private int m_SpawnCount;

	private int m_SpawnLimit = 3;

	private List<Dummy> m_SpawnedDummies = new List<Dummy>();

	public override void Start()
	{
		if (GameManager.Instance.DummyManager == null)
		{
			GameManager.Instance.DummyManager = DummyManager.Create();
		}
	}

	private void Update()
	{
		if (base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (m_BeastBendy == null)
		{
			m_BeastBendy = GameManager.Instance.BeastBendy;
		}
		if (m_BeastBendy == null)
		{
			return;
		}
		for (int num = m_SpawnedDummies.Count - 1; num >= 0; num--)
		{
			Dummy dummy = m_SpawnedDummies[num];
			if (Vector3.Distance(dummy.transform.position, m_BeastBendy.transform.position) > 130f && m_SpawnedDummies.Contains(dummy))
			{
				m_SpawnedDummies.Remove(dummy);
				m_SpawnCount--;
				if (m_SpawnCount < 0)
				{
					m_SpawnCount = 0;
				}
				dummy.OnDeath -= HandleDummyOnDeath;
				dummy.ForceKil();
			}
		}
		m_SpawnTimer += Time.deltaTime;
		if (m_SpawnTimer >= m_SpawnTimerLimit && m_SpawnCount < m_SpawnLimit && m_Bounds.bounds.Contains(m_BeastBendy.transform.position))
		{
			m_SpawnTimer = 0f;
			Spawn();
		}
	}

	public void Spawn()
	{
		if (GameManager.Instance.DummyManager.CanSpawn)
		{
			int num = UnityEngine.Random.Range(0, m_DummyPrefabs.Length);
			Dummy dummy = UnityEngine.Object.Instantiate(m_DummyPrefabs[num]);
			dummy.transform.position = base.transform.position;
			dummy.transform.eulerAngles = base.transform.eulerAngles;
			dummy.OnDeath += HandleDummyOnDeath;
			if (!m_SpawnedDummies.Contains(dummy))
			{
				m_SpawnedDummies.Add(dummy);
			}
			m_SpawnCount++;
			GameManager.Instance.DummyManager.AddDummy(dummy);
		}
	}

	private void HandleDummyOnDeath(object sender, EventArgs e)
	{
		Dummy dummy = sender as Dummy;
		dummy.OnDeath -= HandleDummyOnDeath;
		if (m_SpawnedDummies.Contains(dummy))
		{
			m_SpawnedDummies.Remove(dummy);
		}
		m_SpawnCount--;
		if (m_SpawnCount < 0)
		{
			m_SpawnCount = 0;
		}
	}
}
