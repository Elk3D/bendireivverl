using System;
using UnityEngine;

public class GentBatteryRefillContent : ActionEventContent<GentBatteryRefillContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public GameObject Lighting;

		public LightFixture LightFixture;
	}

	protected override void OnInitialized()
	{
		InternalRemoveListeners();
		InternalAddListeners();
	}

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		Properties properties = m_Properties[0];
		if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat)
		{
			if (properties.ActionEvent.IsActive)
			{
				if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Batteries >= 5)
				{
					Disable();
				}
				properties.ActionEvent.ForceDisable();
			}
			return;
		}
		if (!base.IsInactive && !properties.ActionEvent.IsActive && GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Batteries < 5)
		{
			Enable();
		}
		if (base.IsInactive && !properties.ActionEvent.IsActive)
		{
			if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Batteries < 5)
			{
				Enable();
			}
		}
		else if (!base.IsInactive && properties.ActionEvent.IsActive && GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Batteries >= 5)
		{
			Disable();
		}
	}

	protected override void InternalEnable()
	{
		base.IsActivated = false;
		for (int i = 0; i < m_Properties.Length; i++)
		{
			Properties obj = m_Properties[i];
			obj.LightFixture.SetEmission(1f);
			obj.Lighting.SetActive(value: true);
		}
	}

	protected override void InternalDisable()
	{
		for (int i = 0; i < m_Properties.Length; i++)
		{
			Properties obj = m_Properties[i];
			obj.LightFixture.SetEmission(0f);
			obj.Lighting.SetActive(value: false);
		}
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetCollision(active: false);
	}

	private void HandleInteractableOnInteractionComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetCollision(active: true);
		Disable();
		int num = 5 - GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Batteries;
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_GENT_BATTERIES"), "Icon/Collectables/Small/UIIcon_Battery", num);
		for (int i = 0; i < num; i++)
		{
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddBattery();
		}
	}

	private void InternalAddListeners()
	{
		for (int i = 0; i < m_Properties.Length; i++)
		{
			InteractableAnimationBase interactableAnimationBase = (InteractableAnimationBase)m_Properties[i].ActionEvent;
			if (interactableAnimationBase != null)
			{
				interactableAnimationBase.OnInteract += HandleInteractableOnInteract;
				interactableAnimationBase.OnInteractionComplete += HandleInteractableOnInteractionComplete;
			}
		}
	}

	private void InternalRemoveListeners()
	{
		for (int i = 0; i < m_Properties.Length; i++)
		{
			InteractableAnimationBase interactableAnimationBase = (InteractableAnimationBase)m_Properties[i].ActionEvent;
			if (interactableAnimationBase != null)
			{
				interactableAnimationBase.OnInteract -= HandleInteractableOnInteract;
				interactableAnimationBase.OnInteractionComplete -= HandleInteractableOnInteractionComplete;
			}
		}
	}

	protected override void OnDisposed()
	{
		InternalRemoveListeners();
		base.OnDisposed();
	}
}
