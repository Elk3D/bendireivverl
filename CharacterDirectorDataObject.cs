using System;
using UnityEngine;

[Serializable]
public class CharacterDirectorDataObject : DataObject<int, CharacterDirectorDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private EnemyType m_EnemyType;

	[SerializeField]
	private LostOneType m_LostOneType;

	[SerializeField]
	private int m_CombatState;

	[SerializeField]
	private TransformData m_Transform;

	[SerializeField]
	private int m_CurrentPathIndex;

	[SerializeField]
	private bool m_HasSpecialPath;

	public override int ID => m_DataID;

	public EnemyType EnemyType => m_EnemyType;

	public LostOneType LostOneType => m_LostOneType;

	public int CombatState => m_CombatState;

	public TransformData Transform => m_Transform;

	public int CurrentPathIndex => m_CurrentPathIndex;

	public bool HasSpecialPath => m_HasSpecialPath;

	public void SetEnemyType(EnemyType enemyType)
	{
		m_EnemyType = enemyType;
	}

	public void SetLostOneType(LostOneType lostOneType)
	{
		m_LostOneType = lostOneType;
	}

	public void SetCombatState(bool inCombat)
	{
		m_CombatState = (inCombat ? 1 : 0);
	}

	public void SetTransform(Transform transform)
	{
		m_Transform = TransformData.Get(transform);
	}

	public void SetPathIndex(int index)
	{
		m_CurrentPathIndex = index;
	}

	public void SetSpecialPath(bool hasSpecialPath)
	{
		m_HasSpecialPath = hasSpecialPath;
	}

	protected override void Deserialize()
	{
		m_DataID = m_ID;
	}
}
