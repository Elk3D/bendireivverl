using System;
using UnityEngine;

public class ObjectiveBreakables : Objective
{
	[Header("Breakables")]
	[SerializeField]
	private Breakable[] m_Breakables;

	protected override void InternalInitialize()
	{
		if (m_Breakables != null && m_Breakables.Length != 0)
		{
			for (int i = 0; i < m_Breakables.Length; i++)
			{
				Breakable obj = m_Breakables[i];
				obj.OnHit -= HandleBreakableOnHit;
				obj.OnHit += HandleBreakableOnHit;
				obj.SetActive(active: true);
			}
		}
	}

	private void HandleBreakableOnHit(object sender, EventArgs e)
	{
		RemoveListeners();
		SendOnComplete();
	}

	protected override void RemoveListeners()
	{
		if (m_Breakables != null)
		{
			for (int i = 0; i < m_Breakables.Length; i++)
			{
				m_Breakables[i].OnHit -= HandleBreakableOnHit;
			}
		}
	}
}
