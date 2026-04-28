using System;

public class CoffeeMachineContent : ActionEventContent<CoffeeMachineContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public LightFixture LightFixture;
	}

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (base.IsInactive && GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Slugs >= 15)
		{
			if (GameManager.Instance.Player.Health < UpgradeCheck.GetHealth())
			{
				Enable();
			}
		}
		else if (!base.IsInactive && (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Slugs < 15 || GameManager.Instance.Player.Health >= UpgradeCheck.GetHealth()))
		{
			Disable();
		}
	}

	protected override void InternalEnable()
	{
		EnableLighting();
	}

	private void EnableLighting()
	{
		if (m_Properties.Length != 0)
		{
			Properties properties = m_Properties[0];
			if (properties.LightFixture != null)
			{
				properties.LightFixture.SetEmission(1f);
			}
		}
	}

	protected override void InternalDisable()
	{
		DisableLighting();
	}

	private void DisableLighting()
	{
		if (m_Properties.Length != 0)
		{
			Properties properties = m_Properties[0];
			if (properties.LightFixture != null)
			{
				properties.LightFixture.SetEmission(0f);
			}
		}
	}

	protected override void OnActivate()
	{
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_LOOT_SPENT") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_SLUGS"), "Icon/Collectables/Small/UIIcon_Slug", 15);
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveSlugs(15);
		GameManager.Instance.Player.SetHealth((int)UpgradeCheck.GetHealth());
		Disable();
		DeactivateComplete();
	}
}
