using UnityEngine;

public class ObjectiveLootables : Objective
{
	[SerializeField]
	private Loot[] m_Lootables;

	[SerializeField]
	private bool m_DisableOnComplete;

	protected override void InternalEnable()
	{
		Initialize();
	}

	protected override void InternalInitialize()
	{
		for (int i = 0; i < m_Lootables.Length; i++)
		{
			Loot loot = m_Lootables[i];
			if (!loot.Content.IsActivated)
			{
				loot.Content.Enable();
			}
		}
		if (!m_DisableOnComplete)
		{
			SendOnComplete();
		}
	}

	protected override void InternalDisable()
	{
		for (int i = 0; i < m_Lootables.Length; i++)
		{
			m_Lootables[i].Content.Disable();
		}
		if (m_DisableOnComplete)
		{
			SendOnComplete();
		}
	}
}
