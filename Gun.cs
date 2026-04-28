using System;
using System.Collections;
using UnityEngine;

public class Gun : JMonoBehaviour
{
	[Header("Weapon")]
	[SerializeField]
	private Transform m_ProjectileLocation;

	[SerializeField]
	private GameObject m_Muzzle;

	[SerializeField]
	private float m_FireRate = 0.125f;

	[SerializeField]
	private float m_FireSpread = 15f;

	[SerializeField]
	private int m_AmmoCount = -1;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_HitPercentage = 0.2f;

	[SerializeField]
	private LayerMask m_LayerMask;

	private float m_FireTime;

	private int m_Ammo;

	public bool IsActive { get; private set; }

	public bool IsReloading { get; private set; }

	public event EventHandler OnFire;

	public event EventHandler OnReload;

	private void FixedUpdate()
	{
		if (!IsReloading && IsActive && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			Fire();
		}
	}

	private void Fire()
	{
		float fireRate = m_FireRate;
		m_FireTime += Time.deltaTime;
		if (m_FireTime > fireRate)
		{
			m_FireTime = 0f;
			m_Ammo++;
			if (m_AmmoCount > -1 && m_Ammo > m_AmmoCount)
			{
				ForceReload();
			}
			else
			{
				InternalFire();
			}
		}
	}

	public void ForceFire(bool hasSound)
	{
		InternalFire(hasSound);
	}

	private void InternalFire(bool hasSound = true)
	{
		StopCoroutine(Muzzle());
		StartCoroutine(Muzzle());
		float fireSpread = m_FireSpread;
		float x = UnityEngine.Random.Range(0f - fireSpread, fireSpread);
		float y = UnityEngine.Random.Range(0f - fireSpread, fireSpread);
		m_ProjectileLocation.localEulerAngles = new Vector3(x, y, 0f);
		if (Physics.Raycast(m_ProjectileLocation.position, m_ProjectileLocation.forward, out var hitInfo, float.PositiveInfinity, m_LayerMask, QueryTriggerInteraction.Ignore))
		{
			Vector3 vector = Vector3.Lerp(m_ProjectileLocation.position, hitInfo.point, 0.8f);
			LineRenderer component = GameManager.Instance.PoolingManager.GetFromPool("Projectiles/Projectile_Bullet_Line", 0.02f).transform.GetComponent<LineRenderer>();
			component.transform.position = m_ProjectileLocation.position;
			component.SetPositions(new Vector3[2] { m_ProjectileLocation.position, vector });
			IHittable componentInParent = hitInfo.transform.GetComponentInParent<IHittable>();
			if (GameManager.Instance.Player != null && hitInfo.transform == GameManager.Instance.Player.transform)
			{
				if (UnityEngine.Random.value < m_HitPercentage)
				{
					int damage = ((UnityEngine.Random.value < m_HitPercentage) ? 1 : 0);
					GameManager.Instance.Player.Hit(hitInfo, null, damage);
				}
			}
			else if (componentInParent != null && !componentInParent.IsPlayerBreakable)
			{
				componentInParent.Hit(hitInfo);
			}
			GameManager.Instance.PoolingManager.GetFromPool("Impacts/Impact_Bullet", 5f).GetComponent<Impact>().Initialize(hitInfo, isPooled: true);
		}
		if (hasSound)
		{
			this.OnFire.Send(this);
		}
	}

	private IEnumerator Muzzle()
	{
		m_Muzzle.SetActive(value: true);
		m_Muzzle.transform.localEulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 360f));
		yield return new WaitForSeconds(0.015f);
		if (base.IsDisposed)
		{
			yield return null;
		}
		m_Muzzle.SetActive(value: false);
	}

	public void ForceReload()
	{
		if (!IsReloading)
		{
			StartCoroutine(Reload());
		}
	}

	private IEnumerator Reload()
	{
		IsReloading = true;
		IsActive = false;
		m_Ammo = 0;
		this.OnReload.Send(this);
		yield return new WaitForSeconds(3f);
		if (base.IsDisposed)
		{
			yield return null;
		}
		IsActive = true;
		IsReloading = false;
	}

	public bool IsInView(Vector3 from, Vector3 to, float angle)
	{
		return Vector3.Angle(from, to) < angle;
	}

	public void SetActive(bool active)
	{
		IsActive = active;
	}

	protected override void OnDisposed()
	{
		this.OnFire = null;
		this.OnReload = null;
		base.OnDisposed();
	}
}
