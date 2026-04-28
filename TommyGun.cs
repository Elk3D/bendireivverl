using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TommyGun : JMonoBehaviour
{
	[Header("Character")]
	[SerializeField]
	private CharacterContent m_CharacterContent;

	[SerializeField]
	private Transform m_CharacterForward;

	[SerializeField]
	private AnimationClip m_ReloadAnimationClip;

	[SerializeField]
	private AnimationClipOverrideGroup[] m_AnimationClipOverrideGroup;

	[Header("Weapon")]
	[SerializeField]
	private Transform m_ProjectileLocation;

	[SerializeField]
	private GameObject m_Muzzle;

	[SerializeField]
	private float m_FireRate = 0.075f;

	[SerializeField]
	private float m_FireRateTarget = 0.1f;

	[SerializeField]
	private float m_FireSpread = 15f;

	[SerializeField]
	private float m_FireSpreadTarget = 1f;

	[SerializeField]
	private int m_AmmoCount = 250;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_HitPercentage;

	[SerializeField]
	private LayerMask m_LayerMask;

	[Header("Tracking")]
	[SerializeField]
	private float m_VisionRate = 0.5f;

	[SerializeField]
	private float m_VisionLostRate = 2f;

	[SerializeField]
	private float m_TrackingSpeed = 15f;

	[SerializeField]
	private Transform m_PivotBone;

	[SerializeField]
	private Vector3 m_ForwardOffset = Vector3.zero;

	[SerializeField]
	private Transform m_ArmLBone;

	[SerializeField]
	private Vector3 m_ArmLOffset = Vector3.zero;

	[SerializeField]
	private Transform m_ArmRBone;

	[SerializeField]
	private Vector3 m_ArmROffset = Vector3.zero;

	private Quaternion m_LastLookRotation;

	private Quaternion m_LastTargetRotation;

	private Quaternion m_LastArmLRotation;

	private Quaternion m_LastArmRRotation;

	private Vector3 m_LastKnownLocation;

	private float m_LostTime;

	private float m_LostTimeRate = 10f;

	private float m_FireTime;

	private float m_VisionTime;

	private float m_VisionLostTime;

	private int m_Ammo;

	private bool m_InRange;

	private bool m_HasTarget;

	private bool m_IsInVision;

	private bool m_IsVisible;

	public bool IsActive { get; private set; }

	public bool IsReloading { get; private set; }

	public event EventHandler OnFire;

	public event EventHandler OnReload;

	public event EventHandler OnLost;

	public event EventHandler OnSpotted;

	public override void OnEnable()
	{
		m_CharacterContent.GenericAnimationEvents.SetReciever(this);
		for (int i = 0; i < m_AnimationClipOverrideGroup.Length; i++)
		{
			m_AnimationClipOverrideGroup[i].Initialize();
		}
		m_LastLookRotation = m_CharacterContent.transform.rotation;
		m_LastTargetRotation = m_PivotBone.rotation;
		m_LastArmLRotation = m_ArmLBone.rotation;
		m_LastArmRRotation = m_ArmRBone.rotation;
	}

	private void FixedUpdate()
	{
		if (IsReloading || m_CharacterContent == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		Vector3 vector = m_CharacterContent.transform.position + Vector3.up * 6f;
		Vector3 direction = GameManager.Instance.GameCamera.transform.position - vector;
		Vector3 position = GameManager.Instance.GameCamera.transform.position;
		position.y = m_CharacterContent.transform.position.y;
		Vector3 to = position - m_CharacterContent.transform.position;
		m_InRange = false;
		if (Physics.SphereCast(vector, 0.25f, direction, out var hitInfo, float.PositiveInfinity, m_LayerMask, QueryTriggerInteraction.Ignore))
		{
			if (GameManager.Instance.Player != null && hitInfo.transform == GameManager.Instance.Player.transform)
			{
				if (GameManager.Instance.Player.CombatStatus != CombatStatus.Stealth && GameManager.Instance.Player.CombatStatus != CombatStatus.Hide)
				{
					SetTarget(active: true);
				}
				else if (!m_HasTarget)
				{
					if (IsInView(m_CharacterContent.transform.forward, to, 65f))
					{
						SetTarget(active: true);
					}
				}
				else
				{
					m_InRange = true;
				}
			}
			else
			{
				SetTarget(active: false);
			}
		}
		if (!IsActive)
		{
			return;
		}
		CheckVision();
		if (m_IsVisible)
		{
			Fire();
			return;
		}
		m_LostTime += Time.deltaTime;
		if (m_LostTime > m_LostTimeRate)
		{
			m_LostTime = 0f;
			this.OnLost.Send(this);
		}
	}

	private void LateUpdate()
	{
		if (!(GameManager.Instance.Player == null) && !(m_CharacterContent == null) && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			float num = Vector3.Distance(m_CharacterContent.transform.position, GameManager.Instance.Player.transform.position);
			Vector3 vector = GameManager.Instance.GameCamera.transform.position;
			if (num < 15f)
			{
				vector = GameManager.Instance.Player.transform.position + Vector3.up * 2.5f;
			}
			Vector3 vector2 = ((!m_HasTarget) ? m_LastKnownLocation : vector);
			vector2.y = m_CharacterContent.transform.position.y;
			Vector3 vector3 = vector2 - m_CharacterContent.transform.position;
			Quaternion b = (m_IsVisible ? Quaternion.LookRotation(vector3) : m_CharacterForward.rotation);
			Quaternion b2 = m_PivotBone.rotation;
			Quaternion b3 = m_ArmLBone.rotation;
			Quaternion b4 = m_ArmRBone.rotation;
			if (m_InRange && m_IsInVision && IsInView(m_CharacterContent.transform.forward, vector3, 45f))
			{
				Vector3 obj = ((!m_HasTarget || IsReloading) ? m_LastKnownLocation : GameManager.Instance.GameCamera.transform.position);
				b2 = Quaternion.LookRotation(obj - m_PivotBone.position) * Quaternion.Euler(m_ForwardOffset);
				b3 = Quaternion.LookRotation(obj - m_ArmLBone.position) * Quaternion.Euler(m_ArmLOffset);
				b4 = Quaternion.LookRotation(obj - m_ArmRBone.position) * Quaternion.Euler(m_ArmROffset);
			}
			m_CharacterContent.transform.rotation = Quaternion.Slerp(m_LastLookRotation, b, m_TrackingSpeed * Time.deltaTime);
			m_LastLookRotation = m_CharacterContent.transform.rotation;
			m_PivotBone.rotation = Quaternion.Slerp(m_LastTargetRotation, b2, m_TrackingSpeed * Time.deltaTime);
			m_LastTargetRotation = m_PivotBone.rotation;
			m_ArmLBone.rotation = Quaternion.Slerp(m_LastArmLRotation, b3, m_TrackingSpeed * Time.deltaTime);
			m_LastArmLRotation = m_ArmLBone.rotation;
			m_ArmRBone.rotation = Quaternion.Slerp(m_LastArmRRotation, b4, m_TrackingSpeed * Time.deltaTime);
			m_LastArmRRotation = m_ArmRBone.rotation;
		}
	}

	private void CheckVision()
	{
		if (!m_IsInVision)
		{
			if (m_InRange && m_HasTarget)
			{
				m_VisionTime += Time.deltaTime;
				if (m_VisionTime > m_VisionRate)
				{
					if (GameManager.Instance.Player.CombatStatus != CombatStatus.Stealth && GameManager.Instance.Player.CombatStatus != CombatStatus.Hide)
					{
						m_IsInVision = true;
					}
					else
					{
						SetTarget(active: false);
						m_IsInVision = false;
						m_VisionTime = 0f;
					}
				}
			}
			else
			{
				m_VisionTime = 0f;
			}
		}
		else
		{
			m_LastKnownLocation = GameManager.Instance.GameCamera.transform.position;
			if (!m_InRange || !m_HasTarget)
			{
				SetTarget(active: false);
				m_IsInVision = false;
				m_VisionTime = 0f;
			}
		}
		if (m_IsInVision && !m_IsVisible)
		{
			m_IsVisible = true;
			m_VisionLostTime = m_VisionLostRate;
			UpdateAnimationClips("Fire");
			this.OnSpotted.Send(this);
		}
		else if (!m_IsInVision && m_IsVisible)
		{
			m_VisionLostTime -= Time.deltaTime;
			if (m_VisionLostTime <= 0f)
			{
				m_VisionLostTime = m_VisionLostRate;
				m_IsVisible = false;
				m_LostTime = 0f;
				UpdateAnimationClips("Idle");
				this.OnLost.Send(this);
			}
		}
	}

	private void Fire()
	{
		float num = (m_InRange ? m_FireRateTarget : m_FireRate);
		m_FireTime += Time.deltaTime;
		if (!(m_FireTime > num))
		{
			return;
		}
		m_FireTime = 0f;
		m_Ammo++;
		if (m_Ammo > m_AmmoCount)
		{
			ForceReload();
			return;
		}
		StopCoroutine(Muzzle());
		StartCoroutine(Muzzle());
		float num2 = (m_InRange ? m_FireSpreadTarget : m_FireSpread);
		float x = UnityEngine.Random.Range(0f - num2, num2);
		float y = UnityEngine.Random.Range(0f - num2, num2);
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
					GameManager.Instance.Player.Hit(hitInfo, null, GetDamage());
				}
			}
			else if (componentInParent != null && !componentInParent.IsPlayerBreakable)
			{
				componentInParent.Hit(hitInfo);
			}
			GameManager.Instance.PoolingManager.GetFromPool("Impacts/Impact_Bullet", 5f).GetComponent<Impact>().Initialize(hitInfo, isPooled: true);
		}
		this.OnFire.Send(this);
	}

	private int GetDamage()
	{
		int result = 1;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Easy:
			result = 1;
			break;
		case DifficultyLevel.Normal:
			result = 1;
			break;
		case DifficultyLevel.Hard:
			result = 2;
			break;
		case DifficultyLevel.Impossible:
			result = 3;
			break;
		}
		if (UnityEngine.Random.value < m_HitPercentage / 2f)
		{
			result = 0;
		}
		return result;
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

	private void Reload()
	{
		if (!IsReloading)
		{
			IsActive = false;
			IsReloading = true;
			m_Ammo = 0;
			UpdateAnimationClips("Reload");
			m_CharacterContent.Animator.SetTrigger("Interact");
			this.OnReload.Send(this);
		}
	}

	public void ReloadComplete()
	{
		IsActive = true;
		IsReloading = false;
	}

	public void ForceReload()
	{
		Reload();
	}

	private void SetTarget(bool active)
	{
		m_InRange = active;
		m_HasTarget = active;
	}

	public bool IsInView(Vector3 from, Vector3 to, float angle)
	{
		return Vector3.Angle(from, to) < angle;
	}

	public void SetActive(bool active)
	{
		IsActive = active;
	}

	public void UpdateAnimationClips(string name)
	{
		List<AnimationClip> list = new List<AnimationClip>();
		for (int i = 0; i < m_AnimationClipOverrideGroup.Length; i++)
		{
			AnimationClipOverrideGroup animationClipOverrideGroup = m_AnimationClipOverrideGroup[i];
			if (animationClipOverrideGroup.Name == name)
			{
				for (int j = 0; j < animationClipOverrideGroup.AnimationClipGroups.Length; j++)
				{
					list.Add(animationClipOverrideGroup.AnimationClipGroups[j].AnimationClip);
				}
				break;
			}
		}
		m_CharacterContent.UpdateClipOverrides(list.ToArray());
	}

	protected override void OnDisposed()
	{
		this.OnFire = null;
		this.OnReload = null;
		this.OnLost = null;
		this.OnSpotted = null;
		base.OnDisposed();
	}
}
