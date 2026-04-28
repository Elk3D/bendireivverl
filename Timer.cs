using System;
using UnityEngine;

public class Timer : JDisposable
{
	private Action m_OnComplete;

	private float m_SetTime;

	public float time { get; private set; }

	public bool isPaused { get; private set; }

	public bool isComplete { get; private set; }

	public event EventHandler OnComplete;

	public Timer(float _time)
	{
		time = _time;
	}

	public Timer(float _time, Action onComplete)
	{
		m_OnComplete = onComplete;
		m_SetTime = _time;
		time = m_SetTime;
	}

	public void Start()
	{
		isPaused = false;
	}

	public void Stop()
	{
		isComplete = true;
	}

	public void Pause()
	{
		isPaused = true;
	}

	public void Reset()
	{
		time = m_SetTime;
	}

	public void Update()
	{
		if (!base.IsDisposed && !isComplete && !isPaused)
		{
			time -= Time.deltaTime;
			if (time <= 0f)
			{
				m_OnComplete?.Invoke();
				this.OnComplete.Send(this);
				isComplete = true;
			}
		}
	}

	protected override void OnDisposed()
	{
		this.OnComplete = null;
		m_OnComplete = null;
		base.OnDisposed();
	}
}
