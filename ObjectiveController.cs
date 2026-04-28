using System;
using UnityEngine;

public class ObjectiveController : JMonoBehaviour
{
	[Header("ID")]
	[SerializeField]
	private int m_ID;

	[Header("Data")]
	[SerializeField]
	private ObjectiveData m_Data;

	[Header("Commands")]
	[SerializeField]
	private ObjectiveCommand[] m_OnStartCommands;

	[SerializeField]
	private ObjectiveCommand[] m_OnExecuteCommands;

	[SerializeField]
	private ObjectiveCommand[] m_OnCompleteCommands;

	private int m_OnExecuteIndex;

	public int ID => m_ID;

	public ObjectiveData Data => m_Data;

	public bool IsActive { get; protected set; }

	public bool IsComplete { get; protected set; }

	public event EventHandler OnComplete;

	public void OnStart()
	{
		for (int i = 0; i < m_OnStartCommands.Length; i++)
		{
			m_OnStartCommands[i].Execute();
		}
	}

	public void Execute()
	{
		IsActive = true;
		if (m_OnExecuteCommands.Length != 0)
		{
			for (int i = 0; i < m_OnExecuteCommands.Length; i++)
			{
				ObjectiveCommand objectiveCommand = m_OnExecuteCommands[i];
				objectiveCommand.OnComplete += HandleCommandOnComplete;
				objectiveCommand.Execute();
				if (objectiveCommand.AutoComplete)
				{
					objectiveCommand.ForceComplete();
				}
			}
		}
		else
		{
			Complete();
		}
	}

	public void Complete()
	{
		for (int i = 0; i < m_OnCompleteCommands.Length; i++)
		{
			m_OnCompleteCommands[i].Execute();
		}
		SetComplete();
		GameManager.Instance.CompleteObjective(m_Data);
	}

	public void ForceComplete()
	{
		for (int i = 0; i < m_OnExecuteCommands.Length; i++)
		{
			ObjectiveCommand obj = m_OnExecuteCommands[i];
			obj.OnComplete -= HandleCommandOnComplete;
			obj.ForceComplete();
		}
		for (int j = 0; j < m_OnCompleteCommands.Length; j++)
		{
			m_OnCompleteCommands[j].ForceComplete();
		}
		SetComplete();
	}

	private void SetComplete()
	{
		IsComplete = true;
		this.OnComplete.Send(this);
	}

	private void HandleCommandOnComplete(object sender, EventArgs e)
	{
		((ObjectiveCommand)sender).OnComplete -= HandleCommandOnComplete;
		m_OnExecuteIndex++;
		if (m_OnExecuteIndex >= m_OnExecuteCommands.Length)
		{
			Complete();
		}
	}

	protected override void OnDisposed()
	{
		this.OnComplete = null;
		base.OnDisposed();
	}
}
