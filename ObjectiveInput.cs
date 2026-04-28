using System;
using UnityEngine;

public class ObjectiveInput : Objective
{
	[Header("Action Event")]
	[SerializeField]
	private ActionEvent m_ActionEvent;

	[Header("Input Options")]
	[SerializeField]
	private InteractionType m_InteractionType;

	[Header("Real World Check")]
	[SerializeField]
	private bool m_IsReal;

	private bool m_IsInitialized;

	protected override void InternalInitialize()
	{
		if (m_ActionEvent != null)
		{
			AddListeners();
		}
		else
		{
			DisplayInput();
		}
	}

	protected override void InternalInactive()
	{
		RemoveListeners();
		ClearInteraction();
	}

	protected override void InternalComplete()
	{
		RemoveListeners();
		ClearInteraction();
	}

	private void ClearInteraction()
	{
		GameManager.Instance.Player.Interaction.ResetInteraction();
		GameManager.Instance.ClearInteraction();
	}

	protected override void InternalUpdate()
	{
		if (m_IsInitialized && PlayerInput.InteractOnReleased())
		{
			m_IsInitialized = false;
			SendOnComplete();
		}
	}

	private void DisplayInput()
	{
		string text = TextUtility.GetKey(m_InteractionType.ToString());
		if (m_IsReal)
		{
			text = text.ToUpper();
		}
		GameManager.Instance.ShowInteraction(text, m_IsReal);
		m_IsInitialized = true;
	}

	private void HandleActionEventOnEnter(object sender, EventArgs e)
	{
		DisplayInput();
	}

	private void HandleActionEventOnExit(object sender, EventArgs e)
	{
		m_ActionEvent.ResetAction();
		GameManager.Instance.HideInteraction();
	}

	protected void AddListeners()
	{
		if (m_ActionEvent != null)
		{
			m_ActionEvent.OnEnter += HandleActionEventOnEnter;
			m_ActionEvent.OnExit += HandleActionEventOnExit;
			m_ActionEvent.SetActive(active: true);
		}
	}

	protected override void RemoveListeners()
	{
		if (m_ActionEvent != null)
		{
			m_ActionEvent.OnEnter -= HandleActionEventOnEnter;
			m_ActionEvent.OnExit -= HandleActionEventOnExit;
			m_ActionEvent.SetActive(active: false);
		}
	}
}
