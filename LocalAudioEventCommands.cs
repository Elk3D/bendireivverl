using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LocalAudioEventCommands : JMonoBehaviour
{
	[SerializeField]
	public MonoBehaviour m_Sender;

	[SerializeField]
	public List<LocalAudioCommand> m_EventCallbacks = new List<LocalAudioCommand>();

	private Dictionary<Delegate, string> m_Delegates;

	public override void OnEnable()
	{
		if (m_Sender != null)
		{
			m_Delegates = new Dictionary<Delegate, string>();
			for (int i = 0; i < m_EventCallbacks.Count; i++)
			{
				LocalAudioCommand localAudioCommand = m_EventCallbacks[i];
				Delegate key = DelegateUtility.AddEventHandler(m_Sender, localAudioCommand.EventHandler, localAudioCommand.Execute);
				m_Delegates.Add(key, localAudioCommand.EventHandler);
			}
		}
	}

	public override void OnDisable()
	{
		ClearEventHandlers();
	}

	private void ClearEventHandlers()
	{
		if (m_Sender != null && m_Delegates != null)
		{
			foreach (Delegate key in m_Delegates.Keys)
			{
				DelegateUtility.RemoveEventHandler(m_Sender, m_Delegates[key], key);
			}
		}
		if (m_Delegates != null)
		{
			m_Delegates.Clear();
			m_Delegates = null;
		}
	}

	protected override void OnDisposed()
	{
		ClearEventHandlers();
		base.OnDisposed();
	}
}
