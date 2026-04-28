using System;
using UnityEngine;
using UnityEngine.Events;

public class InteractionCounter : JMonoBehaviour
{
	[SerializeField]
	private Interactable m_Interactable;

	[SerializeField]
	private int m_MaxInteractions;

	[SerializeField]
	private float m_Delay;

	[SerializeField]
	private UnityEvent m_Event;

	private int m_Count;

	public override void Start()
	{
		if (!(m_Interactable == null))
		{
			m_Interactable.OnInteract += HandleInteractableOnInteract;
		}
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		m_Count++;
		if (m_Count >= m_MaxInteractions)
		{
			m_Interactable.OnInteract -= HandleInteractableOnInteract;
			Invoke("Action", m_Delay);
		}
	}

	private void Action()
	{
		m_Event?.Invoke();
	}
}
