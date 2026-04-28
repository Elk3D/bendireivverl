using System;
using UnityEngine;

[DefaultExecutionOrder(1050)]
public class TimelineWeaponChanger : JMonoBehaviour
{
	[Serializable]
	public class WeaponSelector
	{
		public GameObject Weapon;

		public MeshOutline WeaponOutline;
	}

	[SerializeField]
	private WeaponSelector m_WeaponLv2;

	[SerializeField]
	private WeaponSelector m_WeaponLv4;

	public override void Start()
	{
		switch (GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID)
		{
		case WeaponType.LEVEL_2:
		case WeaponType.LEVEL_3:
			if (m_WeaponLv4 != null)
			{
				if (m_WeaponLv4.Weapon != null)
				{
					UnityEngine.Object.Destroy(m_WeaponLv4.Weapon);
				}
				if (m_WeaponLv4.WeaponOutline != null)
				{
					m_WeaponLv4.WeaponOutline.Dispose();
				}
				m_WeaponLv4 = null;
			}
			break;
		case WeaponType.LEVEL_4:
			if (m_WeaponLv2 != null)
			{
				if (m_WeaponLv2.Weapon != null)
				{
					UnityEngine.Object.Destroy(m_WeaponLv2.Weapon);
				}
				if (m_WeaponLv2.WeaponOutline != null)
				{
					m_WeaponLv2.WeaponOutline.Dispose();
				}
				m_WeaponLv2 = null;
			}
			break;
		}
	}
}
