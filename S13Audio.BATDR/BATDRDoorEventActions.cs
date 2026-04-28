using System;
using System.Collections.Generic;
using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRDoorEventActions : JMonoBehaviour
{
	[SerializeField]
	public DoorID m_DoorID;

	[SerializeField]
	public List<LocalAudioCommand> m_EventCallbacks = new List<LocalAudioCommand>();

	[SerializeField]
	public bool m_EnableFallback;

	[SerializeField]
	public int m_FallbackID;

	private MonoBehaviour m_Sender;

	private Dictionary<Delegate, string> m_Delegates;

	public override void OnEnable()
	{
		m_Sender = GetSenderDoor();
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
		else
		{
			if (!m_EnableFallback)
			{
				return;
			}
			for (int j = 0; j < m_EventCallbacks.Count; j++)
			{
				LocalAudioCommand localAudioCommand2 = m_EventCallbacks[j];
				if (localAudioCommand2.ID == m_FallbackID)
				{
					localAudioCommand2.Execute();
				}
			}
		}
	}

	public override void OnDisable()
	{
		ClearEventHandlers();
	}

	public MonoBehaviour GetSenderDoor()
	{
		if (m_DoorID != DoorID.NONE)
		{
			Door[] componentsInChildren = base.transform.root.GetComponentsInChildren<Door>(includeInactive: true);
			if (componentsInChildren != null)
			{
				foreach (Door door in componentsInChildren)
				{
					if (door.ID == m_DoorID)
					{
						return door;
					}
				}
			}
		}
		return null;
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
