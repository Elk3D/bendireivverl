using System;
using UnityEngine;

[Serializable]
public class Command<T, O> : Command where T : IConvertible where O : ICommand
{
	[SerializeField]
	protected T m_CommandType;

	[SerializeField]
	protected O m_CommandObject;

	[SerializeField]
	protected bool m_AutoComplete;

	public sealed override object CommandType => m_CommandType;

	public sealed override object CommandObject => m_CommandObject;

	public sealed override bool AutoComplete => m_AutoComplete;

	public sealed override void Execute()
	{
		if (!base.isExecuted && !base.isComplete && m_CommandObject != null)
		{
			InternalExecute();
			SendOnExecuted();
		}
	}

	protected virtual void InternalExecute()
	{
	}

	public sealed override void ForceComplete()
	{
		InternalForceComplete();
		SendOnComplete();
	}

	protected virtual void InternalForceComplete()
	{
	}
}
public abstract class Command
{
	public bool isExecuted { get; private set; }

	public bool isComplete { get; private set; }

	public abstract object CommandType { get; }

	public abstract object CommandObject { get; }

	public abstract bool AutoComplete { get; }

	public event EventHandler OnExecuted;

	public event EventHandler OnComplete;

	public void SendOnExecuted()
	{
		isExecuted = true;
		this.OnExecuted.Send(this);
	}

	public void SendOnComplete()
	{
		isExecuted = true;
		isComplete = true;
		this.OnComplete.Send(this);
	}

	public abstract void Execute();

	public abstract void ForceComplete();
}
