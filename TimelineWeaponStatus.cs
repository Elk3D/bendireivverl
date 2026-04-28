using UnityEngine;

public class TimelineWeaponStatus : JMonoBehaviour
{
	[SerializeField]
	private WeaponStatus m_WeaponStatus;

	public void Action()
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
	}
}
