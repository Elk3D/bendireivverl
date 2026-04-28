using System;
using UnityEngine;

[Serializable]
public class AbilityDataDirectory : DataDirectory<State.PlayerAbility, AbilityDataObject>
{
	[SerializeField]
	private AbilityStatus m_AbilityStatus;

	public AbilityStatus AbilityStatus => m_AbilityStatus;

	public void SetStatus(AbilityStatus status)
	{
		m_AbilityStatus = status;
	}
}
