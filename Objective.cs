using System;

public abstract class Objective : JMonoBehaviour, ICommand
{
	public bool IsActive { get; protected set; }

	public bool IsComplete { get; protected set; }

	public event EventHandler OnComplete;

	private void Update()
	{
		if (IsActive && !GameManager.Instance.IsPaused && !base.IsDisposed)
		{
			if (IsComplete)
			{
				InternalUpdateComplete();
			}
			else
			{
				InternalUpdate();
			}
		}
	}

	protected virtual void InternalUpdate()
	{
	}

	protected virtual void InternalUpdateComplete()
	{
	}

	public void Initialize()
	{
		IsActive = true;
		InternalInitialize();
	}

	protected virtual void InternalInitialize()
	{
	}

	public void Enable()
	{
		InternalEnable();
	}

	protected virtual void InternalEnable()
	{
	}

	public void Disable()
	{
		InternalDisable();
	}

	protected virtual void InternalDisable()
	{
	}

	public void Inactive()
	{
		InternalInactive();
	}

	protected virtual void InternalInactive()
	{
	}

	public void ForceComplete()
	{
		IsComplete = true;
		InternalForceComplete();
	}

	protected virtual void InternalForceComplete()
	{
	}

	protected void SendOnComplete()
	{
		IsComplete = true;
		InternalComplete();
		this.OnComplete.Send(this);
	}

	protected virtual void InternalComplete()
	{
	}

	protected virtual void RemoveListeners()
	{
	}

	public sealed override void Start()
	{
	}

	public sealed override void Awake()
	{
	}

	public sealed override void OnEnable()
	{
	}

	public sealed override void OnDisable()
	{
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		this.OnComplete = null;
		base.OnDisposed();
	}
}
