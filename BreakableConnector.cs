using System;
using UnityEngine;

public class BreakableConnector : Breakable
{
	[Header("Connected Breakables")]
	[SerializeField]
	private Breakable[] m_Breakables;

	public Breakable[] Breakables => m_Breakables;

	public override void Start()
	{
		for (int i = 0; i < m_Breakables.Length; i++)
		{
			Breakable obj = m_Breakables[i];
			obj.OnHit += HandleBreakableOnHit;
			obj.OnBroken += HandleBreakableOnBroken;
		}
	}

	protected override void InternalForceComplete()
	{
		for (int i = 0; i < m_Breakables.Length; i++)
		{
			m_Breakables[i].ForceComplete();
		}
	}

	private void HandleBreakableOnHit(object sender, EventArgs e)
	{
		for (int i = 0; i < m_Breakables.Length; i++)
		{
			m_Breakables[i].OnHit -= HandleBreakableOnHit;
		}
		SendOnHit();
	}

	private void HandleBreakableOnBroken(object sender, EventArgs e)
	{
		for (int i = 0; i < m_Breakables.Length; i++)
		{
			m_Breakables[i].OnBroken -= HandleBreakableOnBroken;
		}
		SendOnBroken();
	}

	protected override void OnDisposed()
	{
		if (m_Breakables != null)
		{
			for (int i = 0; i < m_Breakables.Length; i++)
			{
				Breakable obj = m_Breakables[i];
				obj.OnHit -= HandleBreakableOnHit;
				obj.OnBroken -= HandleBreakableOnBroken;
			}
		}
		base.OnDisposed();
	}
}
