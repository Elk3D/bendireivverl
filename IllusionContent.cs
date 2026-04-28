using System;
using System.Collections.Generic;
using UnityEngine;

public class IllusionContent : ActionEventContent<IllusionContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public Interactable IllusionContent_Default;
	}

	private Illusion m_Illusion;

	private bool m_IsComplete;

	protected override void OnInitialize()
	{
		m_Illusion = (Illusion)base.Connectable;
		if (!(m_Illusion.Data != null) || m_Properties == null || m_Properties.Length == 0)
		{
			return;
		}
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.IllusionDirectory.ContainsKey(m_Illusion.Data.ID))
		{
			Properties properties = m_Properties[0];
			properties.ActionEvent = CreateContent(properties.IllusionContent_Default);
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
		m_IsComplete = true;
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.IllusionDirectory.ContainsKey(m_Illusion.Data.ID))
		{
			GameManager.Instance.GameData.CurrentSave.DataDirectories.IllusionDirectory.Add(m_Illusion.Data.ID, DataObject<IllusionID, IllusionDataObject>.Create(m_Illusion.Data.ID));
		}
		Array values = Enum.GetValues(typeof(IllusionID));
		List<IllusionID> list = new List<IllusionID>();
		for (int i = 0; i < values.Length; i++)
		{
			object value = values.GetValue(i);
			if (!value.ToString().ToLower().Contains("template"))
			{
				list.Add((IllusionID)value);
			}
		}
		if (GameManager.Instance.GameData.CurrentSave.DataDirectories.IllusionDirectory.Count >= list.Count)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.THE_INSANE_READER);
		}
		GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[7]);
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
		m_Illusion = null;
		base.OnDisposed();
	}
}
