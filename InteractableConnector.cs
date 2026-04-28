using System;
using UnityEngine;

public class InteractableConnector : JMonoBehaviour
{
	[SerializeField]
	private bool m_IsActive;

	[SerializeField]
	private Interactable[] m_Interactables;

	public event EventHandler OnInteracted;

	public void SetActive(bool active)
	{
		m_IsActive = active;
	}

	public override void Start()
	{
		if (m_Interactables != null)
		{
			for (int i = 0; i < m_Interactables.Length; i++)
			{
				Interactable obj = m_Interactables[i];
				obj.OnInteract -= HandleInteractableOnInteract;
				obj.OnInteract += HandleInteractableOnInteract;
			}
		}
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		this.OnInteracted.Send(this);
	}

	private void RemoveListeners()
	{
		if (m_Interactables != null)
		{
			for (int i = 0; i < m_Interactables.Length; i++)
			{
				m_Interactables[i].OnInteract -= HandleInteractableOnInteract;
			}
		}
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		this.OnInteracted = null;
		base.OnDisposed();
	}
}
