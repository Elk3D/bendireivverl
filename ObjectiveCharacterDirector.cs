using UnityEngine;

public class ObjectiveCharacterDirector : Objective
{
	[SerializeField]
	private CharacterDirector m_CharacterDirector;

	protected override void InternalInitialize()
	{
		m_CharacterDirector.Initialize();
	}

	protected override void InternalDisable()
	{
		m_CharacterDirector.UnloadData();
	}

	protected override void InternalForceComplete()
	{
		m_CharacterDirector.LoadData();
	}
}
