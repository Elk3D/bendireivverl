using UnityEngine;

public class EventTriggerMulti : EventTrigger
{
	[Header("Multi Trigger Tags")]
	[SerializeField]
	[TagSelector]
	private string[] m_TriggerTags;

	private int m_ActiveTriggers;

	protected override bool InternalEnterCheck(Collider col)
	{
		if (CheckTag(col.tag))
		{
			m_ActiveTriggers++;
			if (m_ActiveTriggers == 1)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	protected override bool InternalExitCheck(Collider col)
	{
		if (CheckTag(col.tag))
		{
			m_ActiveTriggers--;
			if (m_ActiveTriggers <= 0)
			{
				m_ActiveTriggers = 0;
				return true;
			}
			return false;
		}
		return false;
	}

	protected override bool CanTrigger(bool _isTriggered, Collider col)
	{
		return true;
	}

	protected override bool CheckTag(string tag)
	{
		bool result = false;
		for (int i = 0; i < m_TriggerTags.Length; i++)
		{
			if (m_TriggerTags[i] == tag)
			{
				result = true;
				break;
			}
		}
		return result;
	}
}
