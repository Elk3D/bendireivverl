using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Requirements/New Requirements")]
public class Requirements : ScriptableObject
{
	[SerializeField]
	protected Requirement[] m_List;

	public Requirement[] List => m_List;

	public bool IsComplete()
	{
		bool isComplete = true;
		for (int i = 0; i < m_List.Length; i++)
		{
			Requirement requirement = m_List[i];
			if (requirement != null)
			{
				if (!requirement.IsComplete())
				{
					isComplete = false;
					break;
				}
				continue;
			}
			isComplete = false;
			break;
		}
		return InternalIsComplete(isComplete);
	}

	protected virtual bool InternalIsComplete(bool isComplete)
	{
		return isComplete;
	}
}
