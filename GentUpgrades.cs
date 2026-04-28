using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class GentUpgrades : JMonoBehaviour
{
	[Header("Interaction")]
	[SerializeField]
	private InteractableAnimationBase m_Interactable;

	[SerializeField]
	private MeshOutline m_MeshOutline;

	[Header("Lighting")]
	[SerializeField]
	private Renderer m_EmissionRenderer;

	[SerializeField]
	private GameObject m_Lighting;

	[SerializeField]
	private GameObject m_Particles;

	[Header("Prompts")]
	[SerializeField]
	private Sprite m_Prompt_LV3;

	[SerializeField]
	private Sprite m_Prompt_LV4;

	private GameObject m_Weapon;

	private Sequence m_Sequence;

	private bool m_WasUpgraded;

	public bool IsActive { get; protected set; }

	public event EventHandler OnUpgrade;

	public override void Start()
	{
		m_Particles.SetActive(value: false);
		m_MeshOutline.SetActive(active: false);
		SetActive(active: false);
		m_Interactable.SetActive(active: false);
		RemoveListeners();
		m_Interactable.OnInteract += HandleInteractableOnInteract;
		m_Interactable.OnInteractionComplete += HandleInteractableOnInteractionComplete;
	}

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat)
		{
			if (m_Interactable.IsActive)
			{
				m_Interactable.ForceDisable();
			}
			return;
		}
		if (IsActive && !m_Interactable.IsActive)
		{
			m_Interactable.SetActive(active: true);
			m_Interactable.ResetAction();
		}
		if (GameManager.Instance.Player.CurrentWeapon != null)
		{
			WeaponType iD = GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID;
			if (!IsActive)
			{
				CheckEnable();
				return;
			}
			int count = GameManager.Instance.GameData.CurrentSave.DataDirectories.GentSchematicDirectory.Count;
			if (count > 0)
			{
				if (iD == WeaponType.LEVEL_4)
				{
					Disable();
				}
				else if (count < 3 && iD == WeaponType.LEVEL_3)
				{
					Disable();
				}
				else if (count < 2 && iD == WeaponType.LEVEL_2)
				{
					Disable();
				}
			}
		}
		else if (IsActive)
		{
			Disable();
		}
	}

	private void CheckEnable()
	{
		int count = GameManager.Instance.GameData.CurrentSave.DataDirectories.GentSchematicDirectory.Count;
		WeaponType iD = GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID;
		if (count > 0)
		{
			if (iD == WeaponType.LEVEL_1)
			{
				Enable();
			}
			else if (count >= 3 && iD == WeaponType.LEVEL_3)
			{
				Enable();
			}
			else if (count >= 2 && iD == WeaponType.LEVEL_2)
			{
				Enable();
			}
		}
	}

	private void Enable()
	{
		m_MeshOutline.Enable();
		SetActive(active: true);
		m_Interactable.SetActive(active: true);
		m_Interactable.ResetAction();
	}

	private void Disable()
	{
		m_MeshOutline.SetActive(active: false);
		SetActive(active: false);
		m_Interactable.SetActive(active: false);
	}

	private void GetWeapon()
	{
		RemoveWeapon();
		m_Weapon = UnityEngine.Object.Instantiate(GameManager.Instance.Player.ActiveWeaponDuplicate);
		m_Weapon.transform.SetParent(GameManager.Instance.Player.BodyWeaponParent);
		m_Weapon.transform.localPosition = Vector3.zero;
		m_Weapon.transform.localEulerAngles = Vector3.zero;
		m_Weapon.transform.localScale = GameManager.Instance.Player.ActiveWeaponDuplicate.transform.localScale;
		MeshRenderer[] componentsInChildren = m_Weapon.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].shadowCastingMode = ShadowCastingMode.On;
		}
		m_Weapon.transform.SetParent(GameManager.Instance.Player.BodyWeaponParent.parent);
	}

	public void Upgrade()
	{
		WeaponType iD = GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID;
		switch (iD)
		{
		case WeaponType.LEVEL_1:
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveBatteryCasing(GentUpgradesRequirements.GetBatteryCasings(iD));
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveToolkits(GentUpgradesRequirements.GetToolkits(iD));
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveParts(GentUpgradesRequirements.GetParts(iD));
			break;
		case WeaponType.LEVEL_2:
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveBattery(GentUpgradesRequirements.GetBatteries(iD));
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveToolkits(GentUpgradesRequirements.GetToolkits(iD));
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveParts(GentUpgradesRequirements.GetParts(iD));
			GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetPower(6);
			break;
		case WeaponType.LEVEL_3:
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveBatteryCasing(GentUpgradesRequirements.GetBatteryCasings(iD));
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveBattery(GentUpgradesRequirements.GetBatteries(iD));
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveToolkits(GentUpgradesRequirements.GetToolkits(iD));
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveParts(GentUpgradesRequirements.GetParts(iD));
			GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetPower(6);
			break;
		}
		m_Particles.SetActive(value: true);
		ResetSequence();
		m_Sequence.InsertCallback(6f, delegate
		{
			m_Particles.SetActive(value: false);
		});
		m_Sequence.InsertCallback(5f, delegate
		{
			WeaponType iD2 = GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID;
			WeaponType type = iD2;
			string key = "NOTIFICATION_GENT_PIPE_GENT_LOCK";
			switch (iD2)
			{
			case WeaponType.LEVEL_1:
				type = WeaponType.LEVEL_2;
				break;
			case WeaponType.LEVEL_2:
				m_WasUpgraded = true;
				type = WeaponType.LEVEL_3;
				key = "NOTIFICATION_GENT_PIPE_SHOCK_PIPE";
				break;
			case WeaponType.LEVEL_3:
				m_WasUpgraded = true;
				type = WeaponType.LEVEL_4;
				key = "NOTIFICATION_GENT_PIPE_STUN_PIPE";
				GameManager.Instance.AchievementManager.SetAchievement(AchievementName.GENTS_FINEST);
				break;
			}
			GameManager.Instance.Player.SetNewWeapon(new WeaponData(type));
			GetWeapon();
			GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_GENT_PIPE_UPGRADED") + ": " + TextUtility.GetKey(key));
			GameManager.Instance.ClearGentUpgrades();
		});
		this.OnUpgrade.Send(this);
	}

	public void Close()
	{
		m_Interactable.OnInteractionExit -= HandleInteractableOnInteractionExit;
		m_Interactable.OnInteractionExit += HandleInteractableOnInteractionExit;
		m_Interactable.Exit();
	}

	private void HandleInteractableOnInteractionExit(object sender, EventArgs e)
	{
		m_Interactable.OnInteractionExit -= HandleInteractableOnInteractionExit;
		GameManager.Instance.Player.SetCollision(active: true);
		RemoveWeapon();
		CheckEnable();
		if (m_WasUpgraded)
		{
			string input = "TAB";
			if (GameManager.Instance.Player.CurrentWeapon.Weapon.Data.WeaponType == WeaponType.LEVEL_3)
			{
				GameManager.Instance.ShowInfoPopup(UIInfoPopupDataVO.Create(InfoPopupType.ShockPipe, input));
			}
			else if (GameManager.Instance.Player.CurrentWeapon.Weapon.Data.WeaponType == WeaponType.LEVEL_4)
			{
				GameManager.Instance.ShowInfoPopup(UIInfoPopupDataVO.Create(InfoPopupType.StunPipe, input));
			}
		}
		m_WasUpgraded = false;
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetCollision(active: false);
		UIManager.SetCursor(active: false);
		GetWeapon();
	}

	private void HandleInteractableOnInteractionComplete(object sender, EventArgs e)
	{
		GameManager.Instance.ShowGentUpgrades(this);
	}

	public void SetActive(bool active)
	{
		IsActive = active;
		float value = (IsActive ? 1f : 0f);
		m_EmissionRenderer.material.SetFloat("_Power", value);
		m_Lighting.SetActive(IsActive);
	}

	private void RemoveWeapon()
	{
		if (m_Weapon != null)
		{
			UnityEngine.Object.Destroy(m_Weapon);
			m_Weapon = null;
		}
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			m_Sequence.Kill();
			m_Sequence = null;
		}
	}

	private void RemoveListeners()
	{
		m_Interactable.OnInteract -= HandleInteractableOnInteract;
		m_Interactable.OnInteractionComplete -= HandleInteractableOnInteractionComplete;
		m_Interactable.OnInteractionExit -= HandleInteractableOnInteractionExit;
	}

	protected override void OnDisposed()
	{
		this.OnUpgrade = null;
		m_Weapon = null;
		KillSequence();
		RemoveListeners();
		base.OnDisposed();
	}
}
