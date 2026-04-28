using System;
using UnityEngine;

[Serializable]
public class AbilityDataObject : DataObject<State.PlayerAbility, AbilityDataObject>, IDataObject<State.PlayerAbility>, IDataObject
{
	[SerializeField]
	private State.PlayerAbility m_AbilityID;

	public override State.PlayerAbility ID => m_AbilityID;

	protected override void Deserialize()
	{
		m_AbilityID = m_ID;
	}
}
