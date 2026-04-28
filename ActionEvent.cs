using System;
using UnityEngine;

[DefaultExecutionOrder(100)]
public abstract class ActionEvent : DataMonoBehaviour<int, ActionEventData>
{
	[SerializeField]
	protected int m_ActionEventID = -1;

	[Header("Interact Options")]
	[SerializeField]
	protected bool m_IsActive = true;

	[SerializeField]
	protected bool m_IsSingleAction = true;

	[SerializeField]
	protected bool m_DisposeOnAction = true;

	protected override int m_ID => m_ActionEventID;

	public bool IsActive => m_IsActive;

	public bool IsActioned { get; private set; }

	public event EventHandler OnEnter;

	public event EventHandler OnExit;

	public event EventHandler OnInteract;

	protected override void InternalInitialize()
	{
	}

	public virtual void SetActive(bool active)
	{
		m_IsActive = active;
	}

	public virtual void SetSingleAction(bool value)
	{
		m_IsSingleAction = value;
	}

	public virtual void SetDisposeOnAction(bool value)
	{
		m_DisposeOnAction = value;
	}

	public void ResetAction()
	{
		IsActioned = false;
	}

	protected void ActivateAction(bool _activated, params Action[] _actions)
	{
		IsActioned = _activated;
		for (int i = 0; i < _actions.Length; i++)
		{
			_actions[i]?.Invoke();
		}
	}

	protected void SendOnEnter(object _this = null)
	{
		this.OnEnter.Send((_this != null) ? _this : this);
	}

	protected void SendOnExit(object _this = null)
	{
		this.OnExit.Send((_this != null) ? _this : this);
	}

	protected void SendOnInteract(object _this = null)
	{
		this.OnInteract.Send((_this != null) ? _this : this);
	}

	public void ForceDisable()
	{
		SendOnExit(this);
		SetActive(active: false);
		GameManager.Instance.Player.Interaction.ResetInteraction();
		GameManager.Instance.ClearInteraction();
	}

	protected void Clear()
	{
		IsActioned = true;
		if (m_DisposeOnAction && InternalClear())
		{
			Dispose();
		}
	}

	protected virtual bool InternalClear()
	{
		return true;
	}

	protected override void OnDisposed()
	{
		this.OnInteract = null;
		this.OnEnter = null;
		this.OnExit = null;
		base.OnDisposed();
	}
}
