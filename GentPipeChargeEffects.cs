using System;
using UnityEngine;

public class GentPipeChargeEffects : JMonoBehaviour
{
	[Header("Charges")]
	[SerializeField]
	private Transform m_ChargeLevel;

	[Header("Particles")]
	[SerializeField]
	private ParticleSystem m_ChargeParticles;

	[SerializeField]
	private ParticleSystem m_OverchargeParticles;

	private float m_ChargeLength = 6f;

	private float m_ChargeTimer;

	private bool m_IsCharged;

	private bool m_IsOvercharged;

	public Transform ChargeLevel => m_ChargeLevel;

	public bool IsCharged => m_IsCharged;

	public bool IsOvercharged => m_IsOvercharged;

	public event EventHandler OnCharge;

	public event EventHandler OnEnd;

	public event EventHandler OnOvercharge;

	public override void Awake()
	{
		Disable();
	}

	private void Update()
	{
		if (base.IsDisposed || GameManager.Instance.IsPaused || m_ChargeLevel == null)
		{
			return;
		}
		if (m_IsCharged)
		{
			Vector3 localScale = m_ChargeLevel.localScale;
			localScale.y = m_ChargeTimer / m_ChargeLength;
			m_ChargeLevel.localScale = localScale;
			if (m_ChargeTimer <= 0f)
			{
				m_IsCharged = false;
				m_IsOvercharged = false;
				m_ChargeTimer = 0f;
				Disable();
				this.OnEnd.Send(this);
				return;
			}
		}
		m_ChargeTimer -= Time.deltaTime;
	}

	public void SetChargeLevel(Transform chargeLevel)
	{
		m_ChargeLevel = chargeLevel;
		Disable();
	}

	public void Enable()
	{
		if (m_ChargeLevel != null)
		{
			if (m_IsCharged)
			{
				m_IsOvercharged = true;
			}
			else
			{
				m_IsOvercharged = false;
			}
			m_IsCharged = true;
			m_ChargeTimer = m_ChargeLength;
			m_ChargeLevel.localScale = Vector3.one;
			m_ChargeLevel.gameObject.SetActive(value: true);
		}
		if (m_IsOvercharged)
		{
			if (m_OverchargeParticles != null)
			{
				m_OverchargeParticles.Play();
			}
			this.OnOvercharge.Send(this);
		}
		else if (m_IsCharged)
		{
			if (m_ChargeParticles != null)
			{
				m_ChargeParticles.Play();
			}
			this.OnCharge.Send(this);
		}
	}

	public void Disable()
	{
		if (m_ChargeParticles != null)
		{
			m_ChargeParticles.Stop();
		}
		if (m_OverchargeParticles != null)
		{
			m_OverchargeParticles.Stop();
		}
		if (m_ChargeLevel != null)
		{
			m_ChargeLevel.gameObject.SetActive(value: false);
		}
	}

	public void Hit()
	{
		m_ChargeTimer -= 0.5f;
		if (m_ChargeTimer <= 0f)
		{
			m_ChargeTimer = 0.0001f;
		}
	}

	protected override void OnDisposed()
	{
		this.OnCharge = null;
		this.OnEnd = null;
		this.OnOvercharge = null;
		base.OnDisposed();
	}
}
