using System;
using UnityEngine;

public class InteractableHide : JMonoBehaviour
{
	[DisplayWithoutEdit]
	[SerializeField]
	private int m_ID;

	[Header("Interactable Animation")]
	[SerializeField]
	private InteractableAnimationBase m_Interactable;

	[Header("Enable In Combat")]
	[SerializeField]
	private bool m_EnableInCombat = true;

	public int ID => m_ID;

	public InteractableAnimationBase Interactable => m_Interactable;

	public bool IsHiding { get; private set; }

	public event EventHandler OnEnter;

	public event EventHandler OnExit;

	public void SetID(int id)
	{
		m_ID = id;
	}

	public void Initialize()
	{
		RemoveListeners();
		AddListeners();
	}

	private void Update()
	{
		if (m_EnableInCombat || GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat)
		{
			if (m_Interactable.IsActive)
			{
				m_Interactable.SetActive(active: false);
			}
		}
		else if (!m_Interactable.IsActive)
		{
			m_Interactable.SetActive(active: true);
		}
	}

	public void ForceEnter(HideDataObject dataObject)
	{
		m_Interactable.ForceEnter(dataObject.RotationX, dataObject.RotationY);
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.DisableTeleport();
		}
		GameManager.Instance.Player.SetCollision(active: false);
		GameManager.Instance.Player.SetCombatStatus(CombatStatus.Hide);
		GameManager.Instance.Player.ClearEnemies();
		IsHiding = true;
		this.OnEnter.Send(this);
	}

	private void HandleInteractableOnInteractionComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.S13SetHiding(isHiding: true);
	}

	private void HandleInteractableOnInteractionExit(object sender, EventArgs e)
	{
		this.OnExit.Send(this);
		IsHiding = false;
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.EnableTeleport();
		}
		if (GameManager.Instance.Player.IsCrouched)
		{
			GameManager.Instance.Player.SetCombatStatus(CombatStatus.Stealth);
		}
		else
		{
			GameManager.Instance.Player.SetCombatStatus(CombatStatus.None);
		}
		GameManager.Instance.Player.SetCollision(active: true);
		GameManager.Instance.Player.S13SetHiding(isHiding: false);
	}

	private void AddListeners()
	{
		m_Interactable.OnInteract += HandleInteractableOnInteract;
		m_Interactable.OnInteractionComplete += HandleInteractableOnInteractionComplete;
		m_Interactable.OnInteractionExit += HandleInteractableOnInteractionExit;
	}

	private void RemoveListeners()
	{
		m_Interactable.OnInteract -= HandleInteractableOnInteract;
		m_Interactable.OnInteractionComplete -= HandleInteractableOnInteractionComplete;
		m_Interactable.OnInteractionExit -= HandleInteractableOnInteractionExit;
	}

	protected override void OnDisposed()
	{
		this.OnEnter = null;
		this.OnExit = null;
		RemoveListeners();
		base.OnDisposed();
	}
}
