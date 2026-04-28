using System;
using UnityEngine;

public class Peek : JMonoBehaviour
{
	[DisplayWithoutEdit]
	[SerializeField]
	private int m_ID;

	[SerializeField]
	private InteractableAnimationEnter[] m_Interactions;

	private bool m_IsInactive;

	public int ID => m_ID;

	public int CurrentIndex { get; private set; }

	public event EventHandler OnEnter;

	public event EventHandler OnExit;

	public void SetID(int id)
	{
		m_ID = id;
	}

	public void Initialize()
	{
		for (int i = 0; i < m_Interactions.Length; i++)
		{
			InteractableAnimationEnter obj = m_Interactions[i];
			obj.OnInteract -= HandleInteractionOnInteract;
			obj.OnInteract += HandleInteractionOnInteract;
			obj.OnInteractionComplete -= HandleInteractionOnInteractionComplete;
			obj.OnInteractionComplete += HandleInteractionOnInteractionComplete;
			obj.OnInteractionExit -= HandleInteractionOnInteractionExit;
			obj.OnInteractionExit += HandleInteractionOnInteractionExit;
		}
	}

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		bool flag = GameManager.Instance.Player.CombatStatus == CombatStatus.Combat;
		for (int i = 0; i < m_Interactions.Length; i++)
		{
			InteractableAnimationEnter interactableAnimationEnter = m_Interactions[i];
			if (flag)
			{
				if (!m_IsInactive)
				{
					if (interactableAnimationEnter.IsInteracted)
					{
						interactableAnimationEnter.OnInteractionExit -= HandleInteractionCombatOnInteractionExit;
						interactableAnimationEnter.OnInteractionExit += HandleInteractionCombatOnInteractionExit;
						interactableAnimationEnter.Exit();
						this.OnExit.Send(this);
					}
					if (interactableAnimationEnter.IsActive)
					{
						interactableAnimationEnter.SetActive(active: false);
					}
				}
			}
			else if (m_IsInactive && !interactableAnimationEnter.IsActive)
			{
				interactableAnimationEnter.SetActive(active: true);
			}
		}
		m_IsInactive = flag;
	}

	public void ForceEnter(PeekDataObject dataObject)
	{
		CurrentIndex = dataObject.Index;
		m_Interactions[CurrentIndex].ForceEnter(dataObject.RotationX, dataObject.RotationY);
	}

	private void HandleInteractionOnInteractionComplete(object sender, EventArgs e)
	{
		InteractableAnimationEnter interactableAnimationEnter = sender as InteractableAnimationEnter;
		for (int i = 0; i < m_Interactions.Length; i++)
		{
			InteractableAnimationEnter interactableAnimationEnter2 = m_Interactions[i];
			if (interactableAnimationEnter == interactableAnimationEnter2)
			{
				CurrentIndex = i;
				break;
			}
		}
		this.OnEnter.Send(this);
	}

	private void HandleInteractionOnInteractionExit(object sender, EventArgs e)
	{
		this.OnExit.Send(this);
	}

	private void HandleInteractionCombatOnInteractionExit(object sender, EventArgs e)
	{
		InteractableAnimationEnter obj = sender as InteractableAnimationEnter;
		obj.OnInteractionExit -= HandleInteractionCombatOnInteractionExit;
		obj.SetActive(active: false);
	}

	private void HandleInteractionOnInteract(object sender, EventArgs e)
	{
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.DisableTeleport();
		}
	}

	private void RemoveListeners()
	{
		for (int i = 0; i < m_Interactions.Length; i++)
		{
			InteractableAnimationEnter obj = m_Interactions[i];
			obj.OnInteract -= HandleInteractionOnInteract;
			obj.OnInteractionComplete -= HandleInteractionOnInteractionComplete;
			obj.OnInteractionExit -= HandleInteractionOnInteractionExit;
			obj.OnInteractionExit -= HandleInteractionCombatOnInteractionExit;
		}
	}

	protected override void OnDisposed()
	{
		this.OnExit = null;
		this.OnEnter = null;
		RemoveListeners();
		base.OnDisposed();
	}
}
