using System;
using UnityEngine;

public class MenuElementButton : UIElement
{
	[Header("Button")]
	[SerializeField]
	protected UIButton m_Button;

	public UIButton Button => m_Button;

	public Action Callback { get; protected set; }

	public string InvokeCallback { get; protected set; }

	public event EventHandler OnEnter;

	public event EventHandler OnExit;

	public event EventHandler OnClick;

	public event EventHandler OnCallbackInvoked;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		Callback = (_data as UIElementButtonDataVO).Callback;
		InvokeCallback = (_data as UIElementButtonDataVO).InvokeCallback;
		AddListeners();
	}

	public virtual void ForceOnEnter()
	{
	}

	public virtual void ForceOnExit()
	{
	}

	protected virtual void HandleButtonOnEnter(object sender, EventArgs e)
	{
		this.OnEnter.Send(this);
	}

	protected virtual void HandleButtonOnExit(object sender, EventArgs e)
	{
		this.OnExit.Send(this);
	}

	protected virtual void HandleButtonOnClick(object sender, EventArgs e)
	{
		RemoveListeners();
		GetCallback();
		this.OnClick.Send(this);
	}

	public void GetCallback()
	{
		if (Callback != null)
		{
			Callback();
			this.OnCallbackInvoked.Send(this);
		}
	}

	public virtual void AddListeners()
	{
		m_Button.OnEnter += HandleButtonOnEnter;
		m_Button.OnExit += HandleButtonOnExit;
		m_Button.OnClick += HandleButtonOnClick;
	}

	public virtual void RemoveListeners()
	{
		m_Button.OnEnter -= HandleButtonOnEnter;
		m_Button.OnExit -= HandleButtonOnExit;
		m_Button.OnClick -= HandleButtonOnClick;
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		this.OnEnter = null;
		this.OnExit = null;
		this.OnClick = null;
		this.OnCallbackInvoked = null;
		Callback = null;
		base.OnDisposed();
	}
}
