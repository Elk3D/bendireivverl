using System;
using UnityEngine;

public class Climb : JMonoBehaviour
{
	[SerializeField]
	private InteractableAnimationBase m_Interactable;

	public bool IsClimbing { get; private set; }

	public event EventHandler OnInteract;

	public event EventHandler OnComplete;

	public override void Awake()
	{
		RemoveListeners();
		AddListeners();
	}

	public void Inactive()
	{
		ActiveSetter[] componentsInChildren = base.transform.GetComponentsInChildren<ActiveSetter>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SetActive(active: true);
		}
		Disable();
	}

	public void Enable()
	{
		RemoveListeners();
		AddListeners();
		m_Interactable.SetActive(active: true);
		ActiveSetter[] componentsInChildren = base.transform.GetComponentsInChildren<ActiveSetter>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SetActive(active: true);
		}
	}

	public void Disable()
	{
		RemoveListeners();
		m_Interactable.SetActive(active: false);
	}

	public void ForceActivateComplete()
	{
		Enable();
	}

	private void HandleClimbOnInteract(object sender, EventArgs e)
	{
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.DisableTeleport();
		}
		GameManager.Instance.Player.SetCombatStatus(CombatStatus.Hide);
		GameManager.Instance.Player.SetCollision(active: false);
		GameManager.Instance.Player.ClearEnemies();
		IsClimbing = true;
		this.OnInteract.Send(this);
	}

	private void HandleClimbOnComplete(object sender, EventArgs e)
	{
		this.OnComplete.Send(this);
		IsClimbing = false;
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.EnableTeleport();
		}
		GameManager.Instance.Player.SetCollision(active: true);
		GameManager.Instance.Player.SetCombatStatus(CombatStatus.None);
	}

	private void AddListeners()
	{
		if (m_Interactable != null)
		{
			m_Interactable.OnInteract += HandleClimbOnInteract;
			m_Interactable.OnInteractionComplete += HandleClimbOnComplete;
		}
	}

	protected void RemoveListeners()
	{
		if (m_Interactable != null)
		{
			m_Interactable.OnInteract -= HandleClimbOnInteract;
			m_Interactable.OnInteractionComplete -= HandleClimbOnComplete;
		}
	}

	protected override void OnDisposed()
	{
		this.OnInteract = null;
		this.OnComplete = null;
		RemoveListeners();
		base.OnDisposed();
	}
}
