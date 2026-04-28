using System;
using UnityEngine;

public class FoodContent : ActionEventContent<FoodContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
	}

	private Food m_Food;

	protected override void OnInitialize()
	{
		m_Food = (Food)base.Connectable;
	}

	public void InitializeContent()
	{
		RemoveListeners();
		if (m_Food.Data != null && m_Properties != null && m_Properties.Length != 0)
		{
			Properties properties = m_Properties[0];
			properties.ActionEvent = CreateContent(m_Food.Data.Interactable);
			if (properties.ActionEvent == null)
			{
				return;
			}
			properties.ActionEvent.transform.SetParent(base.transform);
			properties.ActionEvent.transform.localPosition = Vector3.zero;
			properties.ActionEvent.transform.localEulerAngles = Vector3.zero;
			properties.ActionEvent.transform.localScale = Vector3.one;
		}
		AddListeners();
		base.Connectable.OnActivate -= HandleConnectableOnActivate;
		base.Connectable.OnActivate += HandleConnectableOnActivate;
	}

	private void HandleConnectableOnActivate(object sender, EventArgs e)
	{
		base.Connectable.OnActivate -= HandleConnectableOnActivate;
		base.Connectable.Dispose();
	}

	protected override void OnActivate()
	{
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_LOOT_EAT") + " " + TextUtility.GetKey(m_Food.Data.FoodName), "Icon/Collectables/Small/UIIcon_Food", 1);
		int amount = (int)(UpgradeCheck.GetHealth() * ((float)m_Food.Data.Value / 100f));
		GameManager.Instance.Player.Heal(amount);
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddFood();
	}

	protected override void OnForceActivateComplete()
	{
		if (base.Connectable != null)
		{
			base.Connectable.OnActivate -= HandleConnectableOnActivate;
		}
		RemoveListeners();
		m_ActiveProperties.Clear();
		foreach (Properties value in m_PropertiesDictionary.Values)
		{
			m_ActiveProperties.Add(value);
		}
		Disable();
		base.IsActivated = true;
	}

	private Interactable CreateContent(Interactable interactable)
	{
		return GameManager.Instance.AssetManager.CreateAsset<Interactable>(interactable);
	}

	protected override void OnDisposed()
	{
		if (base.Connectable != null)
		{
			base.Connectable.OnActivate -= HandleConnectableOnActivate;
		}
		m_Food = null;
		base.OnDisposed();
	}
}
