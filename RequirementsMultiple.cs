using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Requirements/New Requirements Multiple")]
public class RequirementsMultiple : Requirements
{
	[SerializeField]
	private Requirement[] m_SecondaryList;

	public Requirement[] SecondaryList => m_SecondaryList;

	protected override bool InternalIsComplete(bool isComplete)
	{
		bool flag = isComplete;
		if (!flag)
		{
			flag = true;
			for (int i = 0; i < m_SecondaryList.Length; i++)
			{
				Requirement requirement = m_SecondaryList[i];
				if (requirement != null)
				{
					if (!requirement.IsComplete())
					{
						flag = false;
						break;
					}
					continue;
				}
				flag = false;
				break;
			}
		}
		return flag;
	}
}
