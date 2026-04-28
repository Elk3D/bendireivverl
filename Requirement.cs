using UnityEngine;

public class Requirement : ScriptableObject
{
	[SerializeField]
	protected SectionID m_SectionID;

	[SerializeField]
	protected int m_ID = -1;

	public SectionID SectionID => m_SectionID;

	public int ID => m_ID;

	public bool IsComplete()
	{
		return InternalIsComplete();
	}

	protected virtual bool InternalIsComplete()
	{
		return true;
	}
}
