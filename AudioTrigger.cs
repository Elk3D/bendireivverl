using System;
using UnityEngine;

public class AudioTrigger : JMonoBehaviour
{
	[SerializeField]
	private EventTrigger m_EventTrigger;

	[SerializeField]
	private float m_TimerMin = 900f;

	[SerializeField]
	private float m_TimerMax = 900f;

	private float m_Timer;

	private float m_TimerLimit;

	public bool IsEntered { get; private set; }

	public event EventHandler OnTriggered;

	public override void Start()
	{
		m_TimerLimit = UnityEngine.Random.Range(m_TimerMin, m_TimerMax);
		RemoveListeners();
		AddListeners();
	}

	private void Update()
	{
		if (IsEntered && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			if (m_Timer >= m_TimerLimit)
			{
				m_Timer = 0f;
				this.OnTriggered.Send(this);
			}
			else
			{
				m_Timer += Time.deltaTime;
			}
		}
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		IsEntered = true;
	}

	private void HandleEventTriggerOnExit(object sender, EventArgs e)
	{
		IsEntered = false;
		m_Timer = 0f;
	}

	private void AddListeners()
	{
		m_EventTrigger.OnEnter += HandleEventTriggerOnEnter;
		m_EventTrigger.OnExit += HandleEventTriggerOnExit;
	}

	private void RemoveListeners()
	{
		m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		m_EventTrigger.OnExit -= HandleEventTriggerOnExit;
	}

	protected override void OnDisposed()
	{
		this.OnTriggered = null;
		RemoveListeners();
		base.OnDisposed();
	}
}
