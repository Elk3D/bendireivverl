using System;
using UnityEngine;

public class GentCardContent : ActionEventContent<GentCardContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
	}

	private GentCard m_GentCard;

	protected override void OnInitialize()
	{
		m_GentCard = (GentCard)base.Connectable;
	}

	public void InitializeContent()
	{
		RemoveListeners();
		if (m_GentCard.Data != null && m_Properties != null && m_Properties.Length != 0)
		{
			Properties properties = m_Properties[0];
			properties.ActionEvent = CreateContent(m_GentCard.Data.Interactable);
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
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_GENT_CARD"), "Icon/Collectables/Small/UIIcon_Card", 1);
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddCard();
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
		m_GentCard = null;
		base.OnDisposed();
	}
}
