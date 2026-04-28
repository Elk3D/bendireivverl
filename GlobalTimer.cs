using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTimer : JMonoBehaviour
{
	private static GlobalTimer m_Instance;

	private List<Timer> m_Timers = new List<Timer>();

	public bool isPaused { get; private set; }

	protected GlobalTimer()
	{
	}

	public static Timer SetTimer(float time)
	{
		CheckInstance();
		return m_Instance.InternalSetTime(time);
	}

	public static Timer SetTimer(float time, Action onComplete)
	{
		CheckInstance();
		return m_Instance.InternalSetTime(time, onComplete);
	}

	private static void CheckInstance()
	{
		if (m_Instance == null)
		{
			m_Instance = new GameObject("[GlobalTimer]").AddComponent<GlobalTimer>();
			UnityEngine.Object.DontDestroyOnLoad(m_Instance);
		}
	}

	private Timer InternalSetTime(float time)
	{
		Timer timer = new Timer(time);
		return ReturnTimer(timer);
	}

	private Timer InternalSetTime(float time, Action onComplete)
	{
		Timer timer = new Timer(time, onComplete);
		return ReturnTimer(timer);
	}

	private Timer ReturnTimer(Timer timer)
	{
		m_Timers.Add(timer);
		return timer;
	}

	public static void StartAll()
	{
		m_Instance.isPaused = false;
	}

	public static void PauseAll()
	{
		m_Instance.isPaused = true;
	}

	public static void ClearAll()
	{
		for (int num = m_Instance.m_Timers.Count - 1; num >= 0; num--)
		{
			Timer timer = m_Instance.m_Timers[num];
			if (timer != null)
			{
				timer.Pause();
				timer.Dispose();
			}
			m_Instance.m_Timers.RemoveAt(num);
		}
		m_Instance.m_Timers?.Clear();
	}

	private void Update()
	{
		if (base.IsDisposed || isPaused || GameManager.Instance.IsPaused)
		{
			return;
		}
		for (int num = m_Timers.Count - 1; num >= 0; num--)
		{
			Timer timer = m_Timers[num];
			if (timer == null)
			{
				m_Timers.RemoveAt(num);
			}
			else
			{
				timer?.Update();
				if (timer.isComplete)
				{
					m_Timers.RemoveAt(num);
					timer.Dispose();
				}
			}
		}
	}

	protected override void OnDisposed()
	{
		m_Instance = null;
		m_Timers?.Clear();
		m_Timers = null;
		base.OnDisposed();
	}
}
