using System;
using UnityEngine;

public class MeshOutlineConnector : JComponent
{
	[SerializeField]
	private MeshOutline m_MeshOutline;

	[Header("Interactable Group")]
	[SerializeField]
	private Interactable[] m_Interactables;

	public override void Start()
	{
		if (m_MeshOutline != null)
		{
			AddListeners();
		}
	}

	private void HandleInteractableOnEnter(object sender, EventArgs e)
	{
		m_MeshOutline.Enter();
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		m_MeshOutline.Interact();
	}

	private void HandleInteractableOnExit(object sender, EventArgs e)
	{
		m_MeshOutline.Exit();
	}

	private void AddListeners()
	{
		for (int i = 0; i < m_Interactables.Length; i++)
		{
			Interactable obj = m_Interactables[i];
			obj.OnEnter -= HandleInteractableOnEnter;
			obj.OnEnter += HandleInteractableOnEnter;
			obj.OnInteract -= HandleInteractableOnInteract;
			obj.OnInteract += HandleInteractableOnInteract;
			obj.OnExit -= HandleInteractableOnExit;
			obj.OnExit += HandleInteractableOnExit;
		}
	}

	private void RemoveListeners()
	{
		for (int i = 0; i < m_Interactables.Length; i++)
		{
			Interactable obj = m_Interactables[i];
			obj.OnEnter -= HandleInteractableOnEnter;
			obj.OnInteract -= HandleInteractableOnInteract;
			obj.OnExit -= HandleInteractableOnExit;
		}
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		base.OnDisposed();
	}
}
