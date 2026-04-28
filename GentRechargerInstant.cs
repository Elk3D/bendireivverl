using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class GentRechargerInstant : JMonoBehaviour
{
	[SerializeField]
	private InteractableAnimationSingle m_Interactable;

	[SerializeField]
	private ParticleSystem[] m_Particles;

	[SerializeField]
	private GameObject m_ActiveContent;

	[SerializeField]
	private bool m_ActiveOnStart;

	private GameObject m_Weapon;

	private Sequence m_Sequence;

	private bool m_IsActive;

	public override void Start()
	{
		RemoveListeners();
		m_Interactable.OnInteract += HandleInteractableOnInteract;
		m_Interactable.OnInteractionComplete += HandleInteractableOnInteractionComplete;
		SetActive(m_ActiveOnStart);
		m_Interactable.SetActive(active: false);
	}

	private void Update()
	{
		if (!m_IsActive || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (GameManager.Instance.Player.CurrentWeapon != null && GameManager.Instance.Player.CurrentWeapon.Weapon.Data.WeaponType != WeaponType.LEVEL_1)
		{
			int power = GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.Power;
			if (!m_Interactable.IsActive)
			{
				if (power < 6)
				{
					m_Interactable.SetActive(active: true);
				}
			}
			else if (power >= 6)
			{
				m_Interactable.SetActive(active: false);
			}
		}
		else if (m_Interactable.IsActive)
		{
			m_Interactable.SetActive(active: false);
		}
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
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
		GentPipePowerIndicator[] componentsInChildren2 = GameManager.Instance.Player.GetComponentsInChildren<GentPipePowerIndicator>(includeInactive: true);
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			componentsInChildren2[j].SetPowerLevel(GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.Power);
		}
		ResetSequence();
		m_Sequence.InsertCallback(0.9f, SetPowerIndicator);
	}

	public void SetActive(bool active)
	{
		m_IsActive = active;
		m_ActiveContent.SetActive(m_IsActive);
	}

	private void HandleInteractableOnInteractionComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetCollision(active: true);
		if (m_Weapon != null)
		{
			UnityEngine.Object.Destroy(m_Weapon);
			m_Weapon = null;
		}
	}

	private void SetPowerIndicator()
	{
		if (m_Particles != null)
		{
			for (int i = 0; i < m_Particles.Length; i++)
			{
				m_Particles[i].Play();
			}
		}
		GentPipePowerIndicator[] componentsInChildren = GameManager.Instance.Player.gameObject.GetComponentsInChildren<GentPipePowerIndicator>(includeInactive: true);
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			componentsInChildren[j].SetPowerLevel(6);
		}
		GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetPower(6);
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
	}

	protected override void OnDisposed()
	{
		m_Weapon = null;
		KillSequence();
		RemoveListeners();
		base.OnDisposed();
	}
}
