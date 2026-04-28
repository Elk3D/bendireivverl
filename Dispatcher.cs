using System;
using System.Collections.Generic;
using UnityEngine;

public class Dispatcher : JMonoBehaviour
{
	private List<Action> m_Pending = new List<Action>();

	private static Dispatcher m_Instance;

	public static Dispatcher Instance => Initialize();

	public static Dispatcher Initialize()
	{
		if (m_Instance == null)
		{
			m_Instance = new GameObject("[Dispatcher]").AddComponent<Dispatcher>();
			UnityEngine.Object.DontDestroyOnLoad(m_Instance);
		}
		return m_Instance;
	}

	public void Invoke(Action action)
	{
		lock (m_Pending)
		{
			m_Pending.Add(action);
		}
	}

	private void Update()
	{
		InvokePending();
	}

	private void InvokePending()
	{
		lock (m_Pending)
		{
			foreach (Action item in m_Pending)
			{
				item();
			}
			m_Pending.Clear();
		}
	}

	protected override void OnDisposed()
	{
		if (m_Pending != null)
		{
			m_Pending.Clear();
			m_Pending = null;
		}
		base.OnDisposed();
	}
}
