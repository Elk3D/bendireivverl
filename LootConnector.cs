using System;
using System.Collections.Generic;
using UnityEngine;

public class LootConnector : JMonoBehaviour
{
	[SerializeField]
	private Loot m_Loot;

	[SerializeField]
	private LootConnectorGroup[] m_Group;

	[SerializeField]
	private ControllerRumble.RUMBLE_PRESETS m_presetRumble;

	[SerializeField]
	private Vector2 m_CustomRumble;

	private LootDrawer[] m_Drawers;

	private List<ParticleSystem> m_Particles = new List<ParticleSystem>();

	public Loot Loot => m_Loot;

	public LootConnectorGroup[] Group => m_Group;

	public ControllerRumble.RUMBLE_PRESETS PresetRumble => m_presetRumble;

	public Vector2 CustomRumble => m_CustomRumble;

	public void Initialize()
	{
		m_Drawers = m_Loot.GetComponentsInChildren<LootDrawer>(includeInactive: true);
		m_Loot.OnActivated -= HandleLootOnActivated;
		m_Loot.OnActivated += HandleLootOnActivated;
		for (int i = 0; i < m_Group.Length; i++)
		{
			LootConnectorGroup lootConnectorGroup = m_Group[i];
			for (int j = 0; j < m_Drawers.Length; j++)
			{
				LootDrawer lootDrawer = m_Drawers[j];
				if (lootConnectorGroup.LootDrawerType != lootDrawer.LootDrawerType)
				{
					continue;
				}
				lootConnectorGroup.Content.SetParent(lootDrawer.transform);
				Memo component = lootConnectorGroup.Content.GetComponent<Memo>();
				if (!(component != null))
				{
					continue;
				}
				ParticleSystem[] componentsInChildren = component.GetComponentsInChildren<ParticleSystem>();
				if (componentsInChildren != null)
				{
					foreach (ParticleSystem particleSystem in componentsInChildren)
					{
						m_Particles.Add(particleSystem);
						particleSystem.Stop();
					}
				}
			}
		}
	}

	private void HandleLootOnActivated(object sender, EventArgs e)
	{
		m_Loot.OnActivated -= HandleLootOnActivated;
		if (m_Particles != null && m_Particles.Count > 0)
		{
			for (int i = 0; i < m_Particles.Count; i++)
			{
				m_Particles[i].Play();
			}
		}
	}

	protected override void OnDisposed()
	{
		m_Loot.OnActivated -= HandleLootOnActivated;
		base.OnDisposed();
	}
}
