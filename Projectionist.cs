using System;
using UnityEngine;

public class Projectionist : JMonoBehaviour, IHittable
{
	[SerializeField]
	private GameObject m_Lights;

	private bool m_IsHit;

	private float m_Timer;

	private float m_TimerRate = 10f;

	public bool IsPlayerBreakable => true;

	public bool IsBroken => false;

	public event EventHandler OnHit;

	public event EventHandler OnOff;

	public override void Start()
	{
		m_Lights.SetActive(value: false);
		m_Timer = m_TimerRate;
		m_IsHit = false;
	}

	private void Update()
	{
		if (m_IsHit && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			if (m_Timer >= m_TimerRate)
			{
				m_IsHit = false;
				m_Timer = 0f;
				m_Lights.SetActive(value: false);
				this.OnOff.Send(this);
			}
			else
			{
				m_Timer += Time.deltaTime;
			}
		}
	}

	public void Hit(RaycastHit hit)
	{
		GameManager.Instance.PoolingManager.GetFromPool("Impacts/Impact_Sparks", 8f).GetComponent<Impact>().Initialize(hit, isPooled: true);
		if (!m_IsHit)
		{
			m_IsHit = true;
			m_Lights.SetActive(value: true);
			this.OnHit.Send(this);
		}
	}

	protected override void OnDisposed()
	{
		this.OnHit = null;
		this.OnOff = null;
		base.OnDisposed();
	}
}
