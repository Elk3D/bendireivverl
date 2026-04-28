using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Character/New Character Action Group")]
public class CharacterActionGroup : ScriptableObject
{
	[SerializeField]
	private string m_GroupName = "Action";

	[SerializeField]
	private List<CharacterAction> m_Actions = new List<CharacterAction>();

	public string Name => m_GroupName;

	public List<CharacterAction> Actions => m_Actions;

	public CharacterAction GetAction(Character character, Transform target = null)
	{
		if (character != null && target == null)
		{
			target = character.Target;
		}
		CharacterAction result = null;
		for (int i = 0; i < m_Actions.Count; i++)
		{
			CharacterAction characterAction = m_Actions[i];
			if (characterAction.Validate(character, target))
			{
				result = characterAction;
				break;
			}
		}
		return result;
	}
}
