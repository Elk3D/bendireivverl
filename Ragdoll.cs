using System;
using UnityEngine;

public class Ragdoll : JMonoBehaviour, IHittable
{
	private Rigidbody m_Rigidbody;

	private Collider m_Collider;

	private bool m_IsActive;

	private bool m_IsColor;

	public Rigidbody Rigidbody => m_Rigidbody;

	public Collider Collider => m_Collider;

	private bool m_HasRigidbody => m_Rigidbody != null;

	private bool m_HasCollider => m_Collider != null;

	public bool IsBroken { get; private set; }

	public bool IsPlayerBreakable => true;

	public bool IsActive => m_IsActive;

	public event EventHandler OnHit;

	public override void Awake()
	{
		m_Rigidbody = GetComponent<Rigidbody>();
		m_Collider = GetComponent<Collider>();
		DisableRigidobdy();
		DisableCollider();
	}

	public void Enable()
	{
		EnableRigidbody();
		EnableCollider();
	}

	public void Disable()
	{
		DisableRigidobdy();
	}

	public Ragdoll Activate(bool isColor = false)
	{
		m_IsColor = isColor;
		EnableCollider();
		EnableRigidbody();
		return this;
	}

	public void AddForce(float force, Vector3 position, float radius, float upwardsModifier)
	{
		m_Rigidbody.AddExplosionForce(force * 30f, position, radius, upwardsModifier, ForceMode.Force);
	}

	public void EnableCollider()
	{
		if (m_HasCollider)
		{
			m_Collider.enabled = true;
		}
	}

	public void DisableCollider()
	{
		if (m_HasCollider)
		{
			m_Collider.enabled = false;
		}
	}

	public void EnableRigidbody()
	{
		if (m_HasRigidbody)
		{
			m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			m_Rigidbody.isKinematic = false;
			m_IsActive = true;
		}
	}

	public void DisableRigidobdy()
	{
		if (m_HasRigidbody)
		{
			m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			m_Rigidbody.isKinematic = true;
			m_Rigidbody.Sleep();
			m_IsActive = false;
		}
	}

	public void Hit(RaycastHit hit)
	{
		string prefab = "Impacts/Impact_Ink";
		if (m_IsColor)
		{
			prefab = "Impacts/Impact_Ink_Color";
		}
		GameManager.Instance.PoolingManager.GetFromPool(prefab, 6f).GetComponent<Impact>().Initialize(hit, isPooled: true);
		this.OnHit.Send(this);
		AddForce(50f, hit.point, 20f, 2f);
	}

	protected override void OnDisposed()
	{
		m_Rigidbody = null;
		m_Collider = null;
		base.OnDisposed();
	}
}
