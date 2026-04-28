using UnityEngine;

public class ObjectiveWeaponStatus : Objective
{
	[SerializeField]
	private WeaponStatus m_WeaponStatus;

	protected override void InternalInitialize()
	{
		GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetStatus(m_WeaponStatus);
		if (m_WeaponStatus == WeaponStatus.Active)
		{
			GameManager.Instance.Player.EquipWeapon(new WeaponData(GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID));
		}
		else if (m_WeaponStatus == WeaponStatus.Inactive)
		{
			GameManager.Instance.Player.RemoveWeapon();
		}
		SendOnComplete();
	}
}
