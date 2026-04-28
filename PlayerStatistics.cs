using System;
using UnityEngine;

[Serializable]
public class PlayerStatistics
{
	[SerializeField]
	private int m_Health;

	[SerializeField]
	private int m_Life;

	[SerializeField]
	private int m_ILife;

	[SerializeField]
	private float m_Cooldown;

	[SerializeField]
	private int m_Food;

	[SerializeField]
	private int m_Slugs;

	[SerializeField]
	private int m_Spent;

	[SerializeField]
	private int m_Cards;

	[SerializeField]
	private int m_Toolkits;

	[SerializeField]
	private int m_Parts;

	[SerializeField]
	private int m_BatteryCasings;

	[SerializeField]
	private int m_Batteries;

	[SerializeField]
	private int m_DepositedBatteries;

	[SerializeField]
	private bool m_IsCrouched;

	[SerializeField]
	private int m_UpgradeLevelHealth;

	[SerializeField]
	private int m_UpgradeLevelStamina;

	[SerializeField]
	private int m_UpgradeLevelAbility;

	[SerializeField]
	private int m_Kills;

	[SerializeField]
	private int m_ShockKills;

	[SerializeField]
	private int m_BanishKill;

	[SerializeField]
	private CombatStatus m_CombatStatus;

	public int Health => m_Health;

	public int Life => m_Life;

	public int ILife => m_ILife;

	public float Cooldown => m_Cooldown;

	public int Food => m_Food;

	public int Slugs => m_Slugs;

	public int Spent => m_Spent;

	public int Cards => m_Cards;

	public int Toolkits => m_Toolkits;

	public int Parts => m_Parts;

	public int BatteryCasings => m_BatteryCasings;

	public int Batteries => m_Batteries;

	public int DepositedBatteries => m_DepositedBatteries;

	public bool IsCrouched => m_IsCrouched;

	public int UpgradeLevelHealth => m_UpgradeLevelHealth;

	public int UpgradeLevelStamina => m_UpgradeLevelStamina;

	public int UpgradeLevelAbility => m_UpgradeLevelAbility;

	public int Kills => m_Kills;

	public int ShockKills => m_ShockKills;

	public int BanishKill => m_BanishKill;

	public CombatStatus CombatStatus => m_CombatStatus;

	public void SetHealth(int amount)
	{
		m_Health = amount;
	}

	public void UpdateLife()
	{
		m_Life++;
	}

	public void UpdateILife()
	{
		m_ILife++;
		if (m_ILife >= 25)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.THE_DEMONS_DISMAY);
		}
	}

	public void AddFood()
	{
		m_Food++;
		if (m_Food >= 100)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.THE_FANCY_FOODIE);
		}
	}

	public void AddSlugs(int amount)
	{
		m_Slugs += amount;
	}

	public void RemoveSlugs(int amount)
	{
		m_Slugs -= amount;
		if (m_Slugs < 0)
		{
			m_Slugs = 0;
		}
		m_Spent += amount;
		if (m_Spent >= 500)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.BIG_SPENDER);
		}
	}

	public void AddCard()
	{
		m_Cards++;
	}

	public void RemoveCard()
	{
		m_Cards--;
		if (m_Cards < 0)
		{
			m_Cards = 0;
		}
	}

	public void AddToolkit()
	{
		m_Toolkits++;
	}

	public void RemoveToolkit()
	{
		m_Toolkits--;
		if (m_Toolkits < 0)
		{
			m_Toolkits = 0;
		}
	}

	public void RemoveToolkits(int amount)
	{
		m_Toolkits -= amount;
		if (m_Toolkits < 0)
		{
			m_Toolkits = 0;
		}
	}

	public void AddPart()
	{
		m_Parts++;
	}

	public void RemoveParts(int amount)
	{
		m_Parts -= amount;
		if (m_Parts < 0)
		{
			m_Parts = 0;
		}
	}

	public void AddBattery()
	{
		m_Batteries++;
	}

	public void DepositBattery()
	{
		m_DepositedBatteries += m_Batteries;
		m_Batteries = 0;
	}

	public void RemoveBattery(int amount)
	{
		m_Batteries -= amount;
		if (m_Batteries < 0)
		{
			m_Batteries = 0;
		}
	}

	public void AddBatteryCasing()
	{
		m_BatteryCasings++;
	}

	public void RemoveBatteryCasing(int amount)
	{
		m_BatteryCasings -= amount;
		if (m_BatteryCasings < 0)
		{
			m_BatteryCasings = 0;
		}
	}

	public void SetCrouched(bool isCrouched)
	{
		m_IsCrouched = isCrouched;
	}

	public void UpgradeHealth()
	{
		m_UpgradeLevelHealth++;
		if (m_UpgradeLevelHealth > 3)
		{
			m_UpgradeLevelHealth = 3;
		}
	}

	public void UpgradeStamina()
	{
		m_UpgradeLevelStamina++;
		if (m_UpgradeLevelStamina > 3)
		{
			m_UpgradeLevelStamina = 3;
		}
	}

	public void UpgradeAbility()
	{
		m_UpgradeLevelAbility++;
		if (m_UpgradeLevelAbility > 3)
		{
			m_UpgradeLevelAbility = 3;
		}
	}

	public void AddKill()
	{
		m_Kills++;
	}

	public void AddShockKill()
	{
		m_ShockKills++;
		if (m_ShockKills >= 50)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.HEAVY_HITTER);
		}
	}

	public void AddBanish()
	{
		m_BanishKill++;
		if (m_BanishKill >= 25)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.BACK_TO_THE_PUDDLES);
		}
	}

	public void SetCombatStatus(CombatStatus combatStatus)
	{
		m_CombatStatus = combatStatus;
	}
}
