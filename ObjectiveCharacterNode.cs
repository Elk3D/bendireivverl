using UnityEngine;

public class ObjectiveCharacterNode : Objective
{
	[SerializeField]
	private Transform m_CharacterNodeParent;

	[SerializeField]
	private bool m_Enable = true;

	private CharacterNode[] m_CharacterNodes;

	protected override void InternalInitialize()
	{
		SetNodes();
		SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		SetNodes();
	}

	private void SetNodes()
	{
		m_CharacterNodes = m_CharacterNodeParent.GetComponentsInChildren<CharacterNode>(includeInactive: true);
		CharacterNode[] characterNodes = m_CharacterNodes;
		for (int i = 0; i < characterNodes.Length; i++)
		{
			characterNodes[i].SetActive(m_Enable);
		}
	}
}
