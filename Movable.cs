using System;
using UnityEngine;

public class Movable : ActionEventController<MovableContent, MovableData>
{
	[Header("Section Identifier")]
	[SerializeField]
	private SectionID m_SectionID;

	[Header("Movable Identifier")]
	[SerializeField]
	private MovableID m_ID;

	[Header("Movable Path")]
	[SerializeField]
	private MovablePath m_Path;

	public SectionID SectionID => m_SectionID;

	public MovableID ID => m_ID;

	public MovablePath Path => m_Path;

	public event EventHandler OnExit;

	public event EventHandler OnFrontEnter;

	public event EventHandler OnBackEnter;

	protected override void OnInitialized()
	{
		if (base.Content != null)
		{
			(base.Content as MovableContent).OnFrontEnter += HandleContentOnFrontEnter;
			(base.Content as MovableContent).OnBackEnter += HandleContentOnBackEnter;
			(base.Content as MovableContent).OnExit += HandleContentOnExit;
		}
	}

	public void SetStartLocation()
	{
		MovableContent movableContent = base.Content as MovableContent;
		movableContent.SetStartLocation(m_Path.Path, m_Path.Path[base.Data.CurrentIndex]);
		if (base.Data.IsActive)
		{
			movableContent.Enable();
		}
		else
		{
			movableContent.Disable();
		}
	}

	public void ForceEnterFront(MovableDataObject dataObject)
	{
		(base.Content as MovableContent).ForceEnterFront(dataObject);
	}

	public void ForceEnterBack(MovableDataObject dataObject)
	{
		(base.Content as MovableContent).ForceEnterBack(dataObject);
	}

	private void HandleContentOnFrontEnter(object sender, EventArgs e)
	{
		base.Data.SetMoveState(canMove: true);
		base.Data.SetSide(0);
		this.OnFrontEnter.Send(this);
	}

	private void HandleContentOnBackEnter(object sender, EventArgs e)
	{
		base.Data.SetMoveState(canMove: true);
		base.Data.SetSide(1);
		this.OnBackEnter.Send(this);
	}

	private void HandleContentOnExit(object sender, EventArgs e)
	{
		base.Data.SetMoveState(canMove: false);
		base.Data.SetSide(0);
		this.OnExit.Send(this);
	}

	protected override void OnDisposed()
	{
		if (base.Content != null)
		{
			(base.Content as MovableContent).OnFrontEnter -= HandleContentOnFrontEnter;
			(base.Content as MovableContent).OnBackEnter -= HandleContentOnBackEnter;
			(base.Content as MovableContent).OnExit -= HandleContentOnExit;
		}
		this.OnExit = null;
		this.OnFrontEnter = null;
		this.OnBackEnter = null;
		base.OnDisposed();
	}
}
