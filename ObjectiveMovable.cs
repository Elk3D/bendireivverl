using UnityEngine;

public class ObjectiveMovable : Objective
{
	[SerializeField]
	private Movable m_Movable;

	protected override void InternalInitialize()
	{
		CheckState(enable: true);
	}

	protected override void InternalEnable()
	{
		CheckState(enable: true);
	}

	protected override void InternalDisable()
	{
		CheckState(enable: false);
	}

	protected override void InternalInactive()
	{
		CheckState(enable: false);
	}

	private void CheckState(bool enable)
	{
		if (!(m_Movable == null))
		{
			if (enable)
			{
				m_Movable.Content.Enable();
			}
			else
			{
				m_Movable.Content.Disable();
			}
		}
	}
}
