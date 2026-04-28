using System;
using UnityEngine;

public class ToxicGasController : JMonoBehaviour
{
	[SerializeField]
	private EventTrigger m_EventTrigger;

	private bool m_IsActive;

	private bool m_IsDamage;

	private float m_Timer = -10f;

	private float m_TimerRate = 2f;

	public event EventHandler OnEnter;

	public event EventHandler OnExit;

	public event EventHandler OnCough;

	public override void Start()
	{
		m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		m_EventTrigger.OnEnter += HandleEventTriggerOnEnter;
		m_EventTrigger.OnExit -= HandleEventTriggerOnExit;
		m_EventTrigger.OnExit += HandleEventTriggerOnExit;
	}

	private void HandleEventTriggerOnExit(object sender, EventArgs e)
	{
		m_IsActive = false;
		this.OnExit.Send(this);
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		m_IsActive = true;
		this.OnEnter.Send(this);
	}

	private void Update()
	{
		if (!m_IsActive)
		{
			return;
		}
		m_Timer += Time.deltaTime;
		if (m_Timer >= m_TimerRate)
		{
			m_Timer = 0f;
			int num = (m_IsDamage ? 1 : 0);
			if (GameManager.Instance.Player.Health <= 2f)
			{
				num = 0;
			}
			float num2 = GameManager.Instance.Player.Health - (float)num;
			GameManager.Instance.Player.ShowHealthBar();
			CameraEffects.ShakeRotation(1f, 0.5f);
			CameraEffects.Damage(2f + (UpgradeCheck.GetHealth() - num2) / 2f);
			GameManager.Instance.Player.SetHealth((int)num2, isSilent: true);
			m_IsDamage = !m_IsDamage;
			this.OnCough.Send(this);
		}
	}

	protected override void OnDisposed()
	{
		this.OnEnter = null;
		this.OnExit = null;
		this.OnCough = null;
		m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		m_EventTrigger.OnExit -= HandleEventTriggerOnExit;
		base.OnDisposed();
	}
}
