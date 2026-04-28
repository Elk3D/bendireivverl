using System;
using UnityEngine;

public class ObjectiveBreakable : Objective
{
	[SerializeField]
	private Breakable m_Breakable;

	protected override void InternalInitialize()
	{
		if (!(m_Breakable == null))
		{
			m_Breakable.OnHit -= HandleBreakableOnHit;
			m_Breakable.OnHit += HandleBreakableOnHit;
			m_Breakable.SetActive(active: true);
		}
	}

	private void HandleBreakableOnHit(object sender, EventArgs e)
	{
		m_Breakable.OnHit -= HandleBreakableOnHit;
		SendOnComplete();
	}

	protected override void RemoveListeners()
	{
		if (m_Breakable != null)
		{
			m_Breakable.OnHit -= HandleBreakableOnHit;
		}
	}
}
