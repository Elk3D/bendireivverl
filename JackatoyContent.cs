using System;
using UnityEngine;

public class JackatoyContent : ActionEventContent<JackatoyContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public Interactable Content;
	}

	private Jackatoy m_Jackatoy;

	private bool m_IsComplete;

	protected override void OnInitialize()
	{
		m_Jackatoy = (Jackatoy)base.Connectable;
		if (!(m_Jackatoy.Data != null) || m_Properties == null || m_Properties.Length == 0)
		{
			return;
		}
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.JackatoyDirectory.ContainsKey(m_Jackatoy.Data.ID))
		{
			Properties properties = m_Properties[0];
			properties.ActionEvent = CreateContent(properties.Content);
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
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.JackatoyDirectory.ContainsKey(m_Jackatoy.Data.ID))
		{
			GameManager.Instance.GameData.CurrentSave.DataDirectories.JackatoyDirectory.Add(m_Jackatoy.Data.ID, DataObject<JackatoyID, JackatoyDataObject>.Create(m_Jackatoy.Data.ID));
		}
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.PLAYTHING);
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
		m_Jackatoy = null;
		base.OnDisposed();
	}
}
