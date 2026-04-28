using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioLogContent : ActionEventContent<AudioLogContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
	}

	private AudioLog m_AudioLog;

	private bool m_IsComplete;

	protected override void OnInitialize()
	{
		m_AudioLog = (AudioLog)base.Connectable;
		if (m_AudioLog.Data != null && m_Properties != null && m_Properties.Length != 0)
		{
			if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.AudioLogDirectory.ContainsKey(m_AudioLog.Data.ID))
			{
				Properties properties = m_Properties[0];
				properties.ActionEvent = CreateContent(m_AudioLog.Data.Interactable);
				if (!(properties.ActionEvent == null))
				{
					properties.ActionEvent.transform.SetParent(base.transform);
					properties.ActionEvent.transform.localPosition = Vector3.zero;
					properties.ActionEvent.transform.localEulerAngles = Vector3.zero;
					properties.ActionEvent.transform.localScale = Vector3.one;
				}
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
		string key = TextUtility.GetKey(m_AudioLog.Data.ID.ToString().Replace("AudioLog_", "AudioLogTitle_"));
		GameManager.Instance.ShowAudioLog(key, m_AudioLog.Data.AudioClip);
		m_IsComplete = true;
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.AudioLogDirectory.ContainsKey(m_AudioLog.Data.ID))
		{
			GameManager.Instance.GameData.CurrentSave.DataDirectories.AudioLogDirectory.Add(m_AudioLog.Data.ID, DataObject<AudioLogID, AudioLogDataObject>.Create(m_AudioLog.Data.ID));
		}
		Array values = Enum.GetValues(typeof(AudioLogID));
		List<AudioLogID> list = new List<AudioLogID>();
		for (int i = 0; i < values.Length; i++)
		{
			object value = values.GetValue(i);
			if (!value.ToString().ToLower().Contains("template"))
			{
				list.Add((AudioLogID)value);
			}
		}
		if (GameManager.Instance.GameData.CurrentSave.DataDirectories.AudioLogDirectory.Count >= list.Count)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.THE_WELL_OF_VOICES);
		}
	}

	private void Update()
	{
		if (!(GameManager.Instance.Player == null) && !m_IsComplete && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat)
			{
				CombatDisable();
			}
			else
			{
				CombatEnable();
			}
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
		m_AudioLog = null;
		base.OnDisposed();
	}
}
