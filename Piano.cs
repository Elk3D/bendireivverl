using System;
using UnityEngine;

public class Piano : JMonoBehaviour
{
	[SerializeField]
	private InteractableConnector m_InteractableConnector;

	[SerializeField]
	private MovableContent m_MovableContent;

	[SerializeField]
	private Transform m_SuccessLocation;

	private bool m_IsSuccess;

	private bool m_IsComplete;

	public event EventHandler OnFail;

	public event EventHandler OnSuccess;

	public void Initialize()
	{
		RemoveListeners();
		m_MovableContent.OnExit += HandleMovableContentOnExit;
		m_InteractableConnector.OnInteracted += HandleInteractableConnectorOnInteracted;
		CheckSuccess();
	}

	private void HandleInteractableConnectorOnInteracted(object sender, EventArgs e)
	{
		if (!m_IsComplete)
		{
			if (m_IsSuccess)
			{
				RemoveListeners();
				m_IsComplete = true;
				this.OnSuccess.Send(this);
			}
			else
			{
				this.OnFail.Send(this);
			}
		}
	}

	private void HandleMovableContentOnExit(object sender, EventArgs e)
	{
		CheckSuccess();
	}

	private void CheckSuccess()
	{
		if (m_MovableContent.transform.position == m_SuccessLocation.position)
		{
			m_IsSuccess = true;
		}
		else
		{
			m_IsSuccess = false;
		}
	}

	private void RemoveListeners()
	{
		m_MovableContent.OnExit -= HandleMovableContentOnExit;
		m_InteractableConnector.OnInteracted -= HandleInteractableConnectorOnInteracted;
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		this.OnFail = null;
		this.OnSuccess = null;
		base.OnDisposed();
	}
}
