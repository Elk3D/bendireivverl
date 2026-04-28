using System;

public class BossPhase : JMonoBehaviour
{
	public event EventHandler OnBegin;

	public event EventHandler OnEnd;

	public void Initialize()
	{
		InternalInitialize();
		this.OnBegin.Send(this);
	}

	protected virtual void InternalInitialize()
	{
	}

	public void Complete()
	{
		InternalComplete();
		this.OnEnd.Send(this);
	}

	protected virtual void InternalComplete()
	{
	}

	protected override void OnDisposed()
	{
		this.OnBegin = null;
		this.OnEnd = null;
		base.OnDisposed();
	}
}
