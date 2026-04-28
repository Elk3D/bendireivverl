using System;
using UnityEngine;

public class ButcherGangController : JMonoBehaviour
{
	[Header("Active Status")]
	[SerializeField]
	private bool m_AlwaysActive;

	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Spawners")]
	[SerializeField]
	private ButcherGangSpawner[] m_Spawners;

	private ButcherGangSpawner m_ActiveSpawner;

	public bool AlwaysActive => m_AlwaysActive;

	public ButcherGangSpawner ActiveSpawner => m_ActiveSpawner;

	public bool IsActive { get; private set; }

	public event EventHandler OnReset;

	public void Initialize()
	{
		Disable();
		if (!CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
		}
	}

	public void Enable()
	{
		if (m_Spawners.Length == 0)
		{
			return;
		}
		ButcherGangSpawner closest = GetClosest();
		if (closest != null && closest != m_ActiveSpawner)
		{
			if (AlwaysActive && m_ActiveSpawner != null)
			{
				m_ActiveSpawner.OnReturned -= HandleSpawnerOnReturned;
				m_ActiveSpawner.SetActive(active: false);
			}
			m_ActiveSpawner = closest;
			m_ActiveSpawner.OnSpawned -= HandleSpawnerOnSpawned;
			m_ActiveSpawner.OnSpawned += HandleSpawnerOnSpawned;
			m_ActiveSpawner.OnReturned -= HandleSpawnerOnReturned;
			m_ActiveSpawner.OnReturned += HandleSpawnerOnReturned;
			m_ActiveSpawner.SetActive(active: true);
		}
	}

	private void HandleSpawnerOnSpawned(object sender, EventArgs e)
	{
		m_ActiveSpawner.OnSpawned -= HandleSpawnerOnSpawned;
		GameManager.Instance.Player.SetBattleStatus(BattleStatus.ButcherGang);
		GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		GameManager.Instance.Player.OnDeath += HandlePlayerOnDeath;
	}

	public void Disable()
	{
		for (int i = 0; i < m_Spawners.Length; i++)
		{
			m_Spawners[i].SetActive(active: false);
		}
	}

	public void ResetActiveSpawner()
	{
		if ((bool)m_ActiveSpawner)
		{
			if (GameManager.Instance.Player != null)
			{
				GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
			}
			m_ActiveSpawner.OnSpawned -= HandleSpawnerOnSpawned;
			m_ActiveSpawner.OnReturned -= HandleSpawnerOnReturned;
			m_ActiveSpawner = null;
		}
	}

	private void HandleSpawnerOnReturned(object sender, EventArgs e)
	{
		m_ActiveSpawner.OnReturned -= HandleSpawnerOnReturned;
		m_ActiveSpawner.OnSpawned -= HandleSpawnerOnSpawned;
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		}
		if (GameManager.Instance.Player.BattleStatus == BattleStatus.ButcherGang)
		{
			GameManager.Instance.Player.SetBattleStatus(BattleStatus.None);
		}
		m_ActiveSpawner.SetActive(active: false);
		this.OnReset.Send(this);
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetBattleStatus(BattleStatus.Dead);
		for (int i = 0; i < m_Spawners.Length; i++)
		{
			m_Spawners[i].Cancel();
		}
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		CheckStatus();
	}

	private bool CheckStatus()
	{
		bool flag = true;
		if (m_Requirements != null)
		{
			flag = m_Requirements.IsComplete();
		}
		if (flag)
		{
			GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
			IsActive = true;
		}
		return flag;
	}

	private ButcherGangSpawner GetClosest()
	{
		ButcherGangSpawner result = null;
		float num = float.PositiveInfinity;
		Vector3 position = GameManager.Instance.Player.transform.position;
		if (m_Spawners.Length == 1)
		{
			ResetActiveSpawner();
		}
		ButcherGangSpawner[] spawners = m_Spawners;
		foreach (ButcherGangSpawner butcherGangSpawner in spawners)
		{
			if (AlwaysActive || !(butcherGangSpawner == m_ActiveSpawner))
			{
				float num2 = Vector3.Distance(butcherGangSpawner.transform.position, position);
				if (num2 < num)
				{
					result = butcherGangSpawner;
					num = num2;
				}
			}
		}
		return result;
	}

	protected override void OnDisposed()
	{
		ResetActiveSpawner();
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		}
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		base.OnDisposed();
	}
}
