using System;
using UnityEngine;

public class ObjectiveHide : Objective
{
	[SerializeField]
	private InteractableHide m_InteractableHide;

	[SerializeField]
	private bool m_ControlLock = true;

	[SerializeField]
	private float m_Delay;

	protected override void InternalInitialize()
	{
		Enable();
	}

	protected override void InternalEnable()
	{
		Invoke("ActualEnable", m_Delay);
	}

	private void ActualEnable()
	{
		if (!(m_InteractableHide == null))
		{
			m_InteractableHide.OnEnter -= HandleInteractableHideOnEnter;
			m_InteractableHide.OnEnter += HandleInteractableHideOnEnter;
		}
	}

	private void HandleInteractableHideOnEnter(object sender, EventArgs e)
	{
		m_InteractableHide.OnEnter -= HandleInteractableHideOnEnter;
		if (m_ControlLock)
		{
			(m_InteractableHide.Interactable as InteractableAnimationEnter).SetLock(isLocked: true);
		}
		SendOnComplete();
	}

	protected override void InternalInactive()
	{
		Disable();
	}

	protected override void InternalDisable()
	{
		Invoke("ActualDisable", m_Delay);
	}

	private void ActualDisable()
	{
		if (!(m_InteractableHide == null))
		{
			if (m_ControlLock)
			{
				(m_InteractableHide.Interactable as InteractableAnimationEnter).SetLock(isLocked: false);
			}
			m_InteractableHide.OnExit -= HandleInteractableHideOnExit;
			m_InteractableHide.OnExit += HandleInteractableHideOnExit;
		}
	}

	private void HandleInteractableHideOnExit(object sender, EventArgs e)
	{
		m_InteractableHide.OnExit -= HandleInteractableHideOnExit;
		SendOnComplete();
	}

	protected override void RemoveListeners()
	{
		if (m_InteractableHide != null)
		{
			m_InteractableHide.OnEnter -= HandleInteractableHideOnEnter;
		}
		if (m_InteractableHide != null)
		{
			m_InteractableHide.OnExit -= HandleInteractableHideOnExit;
		}
	}
}
