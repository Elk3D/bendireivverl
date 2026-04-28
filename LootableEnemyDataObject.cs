using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootableEnemyDataObject : DataObject<int, LootableEnemyDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private EnemyType m_EnemyType;

	[SerializeField]
	private LostOneType m_LostOneType;

	[SerializeField]
	private List<GameObjectData> m_GameObjectDatas;

	public override int ID => m_DataID;

	public EnemyType EnemyType => m_EnemyType;

	public LostOneType LostOneType => m_LostOneType;

	public List<GameObjectData> GameObjectDatas => m_GameObjectDatas;

	public void SetEnemyType(EnemyType enemyType)
	{
		m_EnemyType = enemyType;
	}

	public void SetLostOneType(LostOneType lostOneType)
	{
		m_LostOneType = lostOneType;
	}

	public void SetGameObjectData(GameObject gameObject)
	{
		m_GameObjectDatas = GameObjectData.GetChildrenData(gameObject);
	}

	protected override void Deserialize()
	{
		m_DataID = m_ID;
	}
}
