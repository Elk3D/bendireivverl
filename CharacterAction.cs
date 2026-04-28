using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Character/New Character Action")]
public class CharacterAction : ScriptableObject
{
	[SerializeField]
	private string m_ActionName = "Action";

	[SerializeField]
	private string m_AnimationTrigger = "";

	public string Name => m_ActionName;

	public string AnimationTrigger => m_AnimationTrigger;

	protected virtual State.Character m_CharacterState { get; }

	public State.Character CharacterState => m_CharacterState;

	public void Action(Character character)
	{
		OnAction(character);
	}

	protected virtual void OnAction(Character character)
	{
	}

	public bool Validate(Character character, Transform target)
	{
		int num;
		if (!character)
		{
			num = 0;
		}
		else
		{
			num = (OnValidated(character, target) ? 1 : 0);
			if (num != 0)
			{
				character.SetCharacterAction(this);
				character.SetState(CharacterState);
			}
		}
		return (byte)num != 0;
	}

	protected virtual bool OnValidated(Character character, Transform target)
	{
		return true;
	}
}
