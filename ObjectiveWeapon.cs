using System;
using UnityEngine;

public class ObjectiveWeapon : Objective
{
	[Serializable]
	public enum WeaponCheckType
	{
		Has = 0,
		Get = 3,
		Remove = 100
	}

	[SerializeField]
	private WeaponType m_WeaponType;

	[SerializeField]
	private WeaponCheckType m_CheckType;

	[SerializeField]
	private GameObject m_Target;

	[SerializeField]
	private WeaponData m_WeaponData;

	private bool m_IsChecking;

	protected override void InternalInitialize()
	{
		if (m_CheckType == WeaponCheckType.Has)
		{
			m_IsChecking = true;
		}
		else if (m_CheckType == WeaponCheckType.Get)
		{
			GameManager.Instance.Player.EquipWeapon(m_WeaponData);
			GameManager.Instance.ShowCrosshair();
			SendOnComplete();
		}
		else if (m_CheckType == WeaponCheckType.Remove)
		{
			GameManager.Instance.Player.RemoveWeapon();
			GameManager.Instance.ShowCrosshair();
			SendOnComplete();
		}
	}

	protected override void InternalComplete()
	{
		Cancel();
	}

	protected override void InternalForceComplete()
	{
		Cancel();
	}

	private void Update()
	{
		if (m_IsChecking && !base.IsDisposed && !base.IsComplete && !GameManager.Instance.IsPaused && m_CheckType == WeaponCheckType.Has && GameManager.Instance.Player.CurrentWeapon != null && GameManager.Instance.Player.CurrentWeapon.Weapon.Data.WeaponType == m_WeaponType)
		{
			SendOnComplete();
		}
	}

	private void Cancel()
	{
		m_IsChecking = false;
	}
}
