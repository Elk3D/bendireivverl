using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : JDisposable
{
	private List<PlayerRespawn> m_Respawner = new List<PlayerRespawn>();

	public static RespawnManager Create()
	{
		return new RespawnManager();
	}

	public PlayerRespawn GetClosest(Vector3 _currentPosition)
	{
		PlayerRespawn result = null;
		float num = float.PositiveInfinity;
		foreach (PlayerRespawn item in m_Respawner)
		{
			if (item == null || !item.IsActive)
			{
				continue;
			}
			Section componentInParent = item.GetComponentInParent<Section>(includeInactive: true);
			if (!(componentInParent == null) && componentInParent.IsActive && componentInParent.IsInitialized)
			{
				float num2 = Vector3.Distance(item.transform.position, _currentPosition);
				if (num2 < num)
				{
					result = item;
					num = num2;
				}
			}
		}
		return result;
	}

	public PlayerRespawn GetRespawner(PlayerRespawn respawner)
	{
		PlayerRespawn result = null;
		if (Contains(respawner))
		{
			result = m_Respawner[m_Respawner.IndexOf(respawner)];
		}
		return result;
	}

	public bool Contains(PlayerRespawn respawner)
	{
		return m_Respawner.Contains(respawner);
	}

	public void Add(PlayerRespawn respawner)
	{
		if (!Contains(respawner))
		{
			m_Respawner.Add(respawner);
		}
	}

	public void Remove(PlayerRespawn respawner)
	{
		if (Contains(respawner))
		{
			m_Respawner.Remove(respawner);
		}
	}

	public void Clear()
	{
		if (m_Respawner != null)
		{
			m_Respawner.Clear();
		}
	}

	protected override void OnDisposed()
	{
		Clear();
		m_Respawner = null;
		base.OnDisposed();
	}
}
