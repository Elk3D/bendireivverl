using System;
using UnityEngine;

public class BossPhaseController : JMonoBehaviour
{
	[SerializeField]
	private BossPhase[] m_BossPhases;

	private int m_PhaseCount;

	public event EventHandler OnComplete;

	public void Initialize()
	{
		InitializePhase();
	}

	private void InitializePhase()
	{
		BossPhase obj = m_BossPhases[m_PhaseCount];
		obj.OnEnd -= HandleBossPhaseOnEnd;
		obj.OnEnd += HandleBossPhaseOnEnd;
		obj.Initialize();
	}

	private void HandleBossPhaseOnEnd(object sender, EventArgs e)
	{
		(sender as BossPhase).OnEnd -= HandleBossPhaseOnEnd;
		m_PhaseCount++;
		if (m_PhaseCount >= m_BossPhases.Length)
		{
			this.OnComplete.Send(this);
		}
		else
		{
			InitializePhase();
		}
	}

	protected override void OnDisposed()
	{
		this.OnComplete = null;
		if (m_BossPhases != null)
		{
			for (int i = 0; i < m_BossPhases.Length; i++)
			{
				m_BossPhases[i].OnEnd -= HandleBossPhaseOnEnd;
			}
		}
		base.OnDisposed();
	}
}
