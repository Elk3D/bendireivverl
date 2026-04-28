using System;
using UnityEngine;

[Serializable]
public class EnemyDataObject : DataObject<int, EnemyDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private EnemyType m_EnemyType;

	[SerializeField]
	private LostOneType m_LostOneType;

	[SerializeField]
	private int m_CurrentNodeID;

	[SerializeField]
	private int m_CombatState;

	[SerializeField]
	private GameObjectData m_GameObjectDatas;

	public override int ID => m_DataID;

	public EnemyType EnemyType => m_EnemyType;

	public LostOneType LostOneType => m_LostOneType;

	public int CurrentNodeID => m_CurrentNodeID;

	public int CombatState => m_CombatState;

	public GameObjectData GameObjectDatas => m_GameObjectDatas;

	public void SetEnemyType(EnemyType enemyType)
	{
		m_EnemyType = enemyType;
	}

	public void SetLostOneType(LostOneType lostOneType)
	{
		m_LostOneType = lostOneType;
	}

	public void SetCurrentNode(int id)
	{
		m_CurrentNodeID = id;
	}

	public void SetCombatState(bool inCombat)
	{
		m_CombatState = (inCombat ? 1 : 0);
	}

	public void SetGameObjectData(GameObject gameObject)
	{
		m_GameObjectDatas = GameObjectData.GetData(gameObject);
	}

	protected override void Deserialize()
	{
		m_DataID = m_ID;
	}
}
