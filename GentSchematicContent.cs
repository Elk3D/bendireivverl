using System;
using UnityEngine;

public class GentSchematicContent : ActionEventContent<GentSchematicContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public Interactable GentSchematicContent_Default;
	}

	private GentSchematic m_GentSchematic;

	private bool m_IsComplete;

	protected override void OnInitialize()
	{
		m_GentSchematic = (GentSchematic)base.Connectable;
		if (!(m_GentSchematic.Data != null) || m_Properties == null || m_Properties.Length == 0)
		{
			return;
		}
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.GentSchematicDirectory.ContainsKey(m_GentSchematic.Data.ID))
		{
			Properties properties = m_Properties[0];
			properties.ActionEvent = CreateContent(properties.GentSchematicContent_Default);
			if (!(properties.ActionEvent == null))
			{
				properties.ActionEvent.transform.SetParent(base.transform);
				properties.ActionEvent.transform.localPosition = Vector3.zero;
				properties.ActionEvent.transform.localEulerAngles = Vector3.zero;
				properties.ActionEvent.transform.localScale = Vector3.one;
			}
		}
		else
		{
			m_IsComplete = true;
		}
	}

	protected override void OnInitialized()
	{
		if (!m_IsComplete)
		{
			base.Connectable.OnActivate -= HandleConnectableOnActivate;
			base.Connectable.OnActivate += HandleConnectableOnActivate;
		}
	}

	private void HandleConnectableOnActivate(object sender, EventArgs e)
	{
		base.Connectable.OnActivate -= HandleConnectableOnActivate;
		base.Connectable.Dispose();
	}

	protected override void OnActivate()
	{
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_SCHEMATIC"));
		m_IsComplete = true;
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.GentSchematicDirectory.ContainsKey(m_GentSchematic.Data.ID))
		{
			GameManager.Instance.GameData.CurrentSave.DataDirectories.GentSchematicDirectory.Add(m_GentSchematic.Data.ID, DataObject<GentSchematicID, GentSchematicDataObject>.Create(m_GentSchematic.Data.ID));
		}
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
		m_GentSchematic = null;
		base.OnDisposed();
	}
}
