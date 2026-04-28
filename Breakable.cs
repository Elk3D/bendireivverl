using System;
using UnityEngine;

public class Breakable : JMonoBehaviour, IHittable
{
	[SerializeField]
	private bool m_IsActive = true;

	[SerializeField]
	private bool m_IsCollision;

	[Header("Breakable Base Settings")]
	[SerializeField]
	private bool m_IsPlayerBreakable = true;

	[SerializeField]
	private bool m_DisposeOnHit = true;

	[SerializeField]
	private ActiveSetter m_ActiveSetter;

	private bool m_IsTriggered;

	public bool IsBroken { get; private set; }

	public bool IsPlayerBreakable => m_IsPlayerBreakable;

	public event EventHandler OnHit;

	public event EventHandler OnBroken;

	public void SetActive(bool active)
	{
		m_IsActive = active;
	}

	public void Hit(RaycastHit hit)
	{
		if (m_IsActive && !IsBroken)
		{
			InternalHit(hit);
		}
	}

	protected virtual void InternalHit(RaycastHit hit)
	{
		HitOnComplete();
		if (m_ActiveSetter != null)
		{
			m_ActiveSetter.SetActive(active: true);
			Collider[] components = GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].enabled = false;
			}
		}
		if (m_IsTriggered)
		{
			Rigidbody component = GetComponent<Rigidbody>();
			if (component != null)
			{
				component.detectCollisions = false;
			}
		}
		BrokenOnComplete();
		if (m_DisposeOnHit)
		{
			Dispose();
		}
	}

	protected void HitOnComplete()
	{
		if (!base.IsDisposed)
		{
			SendOnHit();
		}
	}

	protected void BrokenOnComplete()
	{
		if (!base.IsDisposed)
		{
			IsBroken = true;
			SendOnBroken();
		}
	}

	public void SendOnHit()
	{
		this.OnHit.Send(this);
	}

	public void SendOnBroken()
	{
		this.OnBroken.Send(this);
	}

	public void ForceComplete()
	{
		if (m_ActiveSetter != null)
		{
			m_ActiveSetter.ForceComplete();
		}
		Collider[] components = GetComponents<Collider>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].enabled = false;
		}
		IsBroken = true;
		InternalForceComplete();
		if (m_DisposeOnHit)
		{
			Dispose();
		}
	}

	protected virtual void InternalForceComplete()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (m_IsCollision && !IsBroken && !base.IsDisposed && other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			RaycastHit hit = new RaycastHit
			{
				point = base.transform.position
			};
			m_IsTriggered = true;
			InternalHit(hit);
		}
	}

	protected override void OnDisposed()
	{
		this.OnHit = null;
		this.OnBroken = null;
		base.OnDisposed();
	}
}
