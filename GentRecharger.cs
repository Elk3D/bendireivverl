using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class GentRecharger : JMonoBehaviour
{
	[SerializeField]
	private InteractableAnimationBase m_InteractableBatteries;

	[SerializeField]
	private InteractableAnimationBase m_InteractableCharger;

	[Header("Lighting")]
	[SerializeField]
	private Renderer m_EmissionRenderer;

	[SerializeField]
	private GameObject m_Lighting;

	[Header("Temp")]
	[SerializeField]
	private GameObject m_Electricity;

	[SerializeField]
	private Renderer m_ChargerEmissions;

	[SerializeField]
	private GameObject[] m_PowerLevels;

	[SerializeField]
	private GameObject[] m_Batteries;

	private Sequence m_BatterySequence;

	private Sequence m_ChargerSequence;

	private GameObject m_Weapon;

	private bool m_DisableGentRecharger;

	private bool m_IsBatteryActive;

	private bool m_IsChargerActive;

	private int m_BatteryMax = 5;

	private int m_BatteryCount;

	public InteractableAnimationBase InteractableBatteries => m_InteractableBatteries;

	public InteractableAnimationBase InteractableCharger => m_InteractableCharger;

	public bool IsActive { get; protected set; }

	public int BatteryCount => m_BatteryCount;

	public event EventHandler OnDeposit;

	public event EventHandler OnBatteriesComplete;

	public event EventHandler OnCharged;

	public void Initialize(int batteries)
	{
		m_BatteryCount = batteries;
		for (int i = 0; i < m_Batteries.Length; i++)
		{
			GameObject gameObject = m_Batteries[i];
			if (i >= m_BatteryCount)
			{
				gameObject.SetActive(value: false);
			}
			else
			{
				gameObject.SetActive(value: true);
			}
		}
		for (int j = 0; j < m_PowerLevels.Length; j++)
		{
			m_PowerLevels[j].SetActive(value: false);
		}
		m_ChargerEmissions.material.SetFloat("_Power", 0f);
		m_Electricity.SetActive(value: false);
		SetActive(active: false);
		RemoveListeners();
		m_InteractableBatteries.OnInteract += HandleInteractableBatteriesOnInteract;
		m_InteractableBatteries.OnInteractionComplete += HandleInteractableBatteriesOnInteractionComplete;
		m_InteractableBatteries.SetActive(active: false);
		m_InteractableCharger.OnInteract += HandleInteractableCharacterOnInteract;
		m_InteractableCharger.OnInteractionComplete += HandleInteractableCharacterOnInteractionComplete;
		m_InteractableCharger.OnInteractionExit += HandleInteractableCharacterOnInteractionExit;
		m_InteractableCharger.SetActive(active: false);
		if (m_BatteryCount >= m_BatteryMax)
		{
			m_IsBatteryActive = true;
			m_IsChargerActive = true;
			SetActive(active: true);
		}
	}

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (!m_DisableGentRecharger)
		{
			if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat)
			{
				if (m_InteractableBatteries.IsActive)
				{
					m_InteractableBatteries.ForceDisable();
				}
				if (m_InteractableCharger.IsActive)
				{
					m_InteractableCharger.ForceDisable();
				}
				return;
			}
			if (m_IsBatteryActive && !m_IsChargerActive && !m_InteractableBatteries.IsActive)
			{
				CheckBatteryDeposit();
			}
			if (!m_IsBatteryActive && !m_IsChargerActive && !m_InteractableBatteries.IsActive)
			{
				CheckBatteryDeposit();
			}
			else if (m_IsBatteryActive && m_IsChargerActive && !m_InteractableCharger.IsActive && !m_InteractableCharger.IsActive && GameManager.Instance.Player.CurrentWeapon != null && GameManager.Instance.Player.CurrentWeapon.Weapon.Data.WeaponType != WeaponType.LEVEL_1 && GameManager.Instance.Player.CurrentWeapon.Weapon.Data.WeaponType != WeaponType.NONE && GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.Power < 6)
			{
				m_InteractableCharger.SetActive(active: true);
				m_InteractableCharger.ResetAction();
			}
		}
		else
		{
			if (m_InteractableBatteries.IsActive)
			{
				m_InteractableBatteries.ForceDisable();
			}
			if (m_InteractableCharger.IsActive)
			{
				m_InteractableCharger.ForceDisable();
			}
		}
	}

	public void SetGentRecharger(bool active)
	{
		m_DisableGentRecharger = !active;
	}

	private void CheckBatteryDeposit()
	{
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Batteries > 0 && m_BatteryCount < m_BatteryMax)
		{
			m_IsBatteryActive = true;
			m_InteractableBatteries.SetActive(active: true);
			m_InteractableBatteries.ResetAction();
		}
	}

	private void HandleInteractableCharacterOnInteractionExit(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetCollision(active: true);
		if (m_Weapon != null)
		{
			UnityEngine.Object.Destroy(m_Weapon);
			m_Weapon = null;
		}
	}

	private void HandleInteractableCharacterOnInteract(object sender, EventArgs e)
	{
		if (GameManager.Instance.Player.ActiveWeaponDuplicate != null)
		{
			GameManager.Instance.Player.SetCollision(active: false);
			m_Weapon = UnityEngine.Object.Instantiate(GameManager.Instance.Player.ActiveWeaponDuplicate);
			m_Weapon.transform.SetParent(GameManager.Instance.Player.BodyWeaponParent);
			m_Weapon.transform.localPosition = Vector3.zero;
			m_Weapon.transform.localEulerAngles = Vector3.zero;
			m_Weapon.transform.localScale = GameManager.Instance.Player.ActiveWeaponDuplicate.transform.localScale;
			MeshRenderer[] componentsInChildren = m_Weapon.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].shadowCastingMode = ShadowCastingMode.On;
			}
			m_Weapon.transform.SetParent(GameManager.Instance.Player.BodyWeaponParent.parent);
		}
	}

	private void HandleInteractableBatteriesOnInteract(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetCollision(active: false);
		m_InteractableBatteries.SetActive(active: false);
		int batteries = GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Batteries;
		for (int i = 0; i < batteries; i++)
		{
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveBattery(1);
			m_BatteryCount++;
			if (m_BatteryCount >= m_BatteryMax)
			{
				break;
			}
		}
		ResetBatterySequence();
		float num = 2f;
		for (int j = 0; j < m_Batteries.Length; j++)
		{
			if (j >= m_BatteryCount)
			{
				break;
			}
			GameObject batteries2 = m_Batteries[j];
			num += (float)j * 0.03f;
			m_BatterySequence.InsertCallback(num, delegate
			{
				batteries2.SetActive(value: true);
			});
		}
		num += 0.25f;
		m_BatterySequence.InsertCallback(num, BatteriesOnComplete);
	}

	private void BatteriesOnComplete()
	{
		this.OnDeposit.Send(this);
		if (m_BatteryCount >= m_BatteryMax)
		{
			m_IsChargerActive = true;
			SetActive(active: true);
			this.OnBatteriesComplete.Send(this);
		}
		else
		{
			m_IsBatteryActive = false;
		}
	}

	private void HandleInteractableBatteriesOnInteractionComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetCollision(active: true);
	}

	private void HandleInteractableCharacterOnInteractionComplete(object sender, EventArgs e)
	{
		m_ChargerEmissions.material.DOFloat(100f, "_Power", 3f);
		ResetChargerSequence();
		float num = 1.2f;
		float num2 = num - 0.1f;
		m_ChargerSequence.InsertCallback(1f, delegate
		{
			m_Electricity.SetActive(value: true);
		});
		for (int num3 = m_Batteries.Length - 1; num3 >= 0; num3--)
		{
			GameObject batteries = m_Batteries[num3];
			num2 += (float)num3 * 0.05f;
			m_ChargerSequence.InsertCallback(num2, delegate
			{
				batteries.SetActive(value: false);
			});
		}
		for (int num4 = 0; num4 < m_PowerLevels.Length; num4++)
		{
			GameObject powerlevel = m_PowerLevels[num4];
			num += (float)num4 * 0.045f;
			m_ChargerSequence.InsertCallback(num, delegate
			{
				powerlevel.SetActive(value: true);
			});
		}
		num += 0.1f;
		m_ChargerSequence.InsertCallback(num, ShutOff);
	}

	private void ShutOff()
	{
		m_InteractableCharger.SetActive(active: false);
		m_InteractableCharger.Exit();
		for (int i = 0; i < m_PowerLevels.Length; i++)
		{
			m_PowerLevels[i].SetActive(value: false);
		}
		m_ChargerEmissions.material.SetFloat("_Power", 0f);
		m_Electricity.SetActive(value: false);
		SetActive(active: false);
		m_BatteryCount = 0;
		m_IsBatteryActive = false;
		m_IsChargerActive = false;
		GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetPower(6);
		GentPipePowerIndicator[] componentsInChildren = GameManager.Instance.Player.gameObject.GetComponentsInChildren<GentPipePowerIndicator>(includeInactive: true);
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			componentsInChildren[j].SetPowerLevel(6);
		}
		this.OnCharged.Send(this);
	}

	public void SetActive(bool active)
	{
		IsActive = active;
		float value = (IsActive ? 1f : 0f);
		m_EmissionRenderer.material.SetFloat("_Power", value);
		m_Lighting.SetActive(IsActive);
	}

	private void ResetBatterySequence()
	{
		KillBatterySequence();
		m_BatterySequence = DOTween.Sequence();
	}

	private void KillBatterySequence()
	{
		if (m_BatterySequence != null)
		{
			m_BatterySequence.Kill();
			m_BatterySequence = null;
		}
	}

	private void ResetChargerSequence()
	{
		KillChargerSequence();
		m_ChargerSequence = DOTween.Sequence();
	}

	private void KillChargerSequence()
	{
		if (m_ChargerSequence != null)
		{
			m_ChargerSequence.Kill();
			m_ChargerSequence = null;
		}
	}

	private void RemoveListeners()
	{
		m_InteractableCharger.OnInteract -= HandleInteractableCharacterOnInteract;
		m_InteractableBatteries.OnInteractionComplete -= HandleInteractableBatteriesOnInteractionComplete;
		m_InteractableBatteries.OnInteract -= HandleInteractableBatteriesOnInteract;
		m_InteractableCharger.OnInteractionComplete -= HandleInteractableCharacterOnInteractionComplete;
		m_InteractableCharger.OnInteractionExit -= HandleInteractableCharacterOnInteractionExit;
	}

	protected override void OnDisposed()
	{
		this.OnDeposit = null;
		this.OnBatteriesComplete = null;
		this.OnCharged = null;
		m_Weapon = null;
		KillBatterySequence();
		KillChargerSequence();
		RemoveListeners();
		base.OnDisposed();
	}
}
