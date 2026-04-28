using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[DefaultExecutionOrder(110)]
public class ActionEventConnector : JMonoBehaviour
{
	[SerializeField]
	private ActionEventCommand[] m_Senders;

	[SerializeField]
	private ActionEventExecutionType m_ExecutionType;

	[SerializeField]
	private bool m_FailureOnReset;

	[SerializeField]
	private bool m_DisableOnSuccess;

	[SerializeField]
	private bool m_OnStart = true;

	[SerializeField]
	private bool m_ForceInactive;

	[SerializeField]
	private ActionEventCommand[] m_Recievers;

	[SerializeField]
	private bool m_HasExecutionDelay;

	[SerializeField]
	private float m_ExecutionDelay;

	[SerializeField]
	private bool m_IsExecutionDelaySingle;

	private List<bool> m_ExecutionSequence = new List<bool>();

	private int m_ExecutionSequenceOrder = -1;

	private Sequencer m_Sequencer;

	public ActionEventCommand[] Senders => m_Senders;

	public ActionEventCommand[] Recievers => m_Recievers;

	public ActionEventExecutionType ExecutionType => m_ExecutionType;

	public bool HasExecutionDelay => m_HasExecutionDelay;

	public event EventHandler OnEvent;

	public event EventHandler OnComplete;

	public override void Start()
	{
		for (int i = 0; i < m_Senders.Length; i++)
		{
			m_ExecutionSequence.Add(item: false);
		}
		AddListeners();
		m_Sequencer = new Sequencer();
		if (!m_OnStart)
		{
			Disable();
		}
		if (m_ForceInactive)
		{
			ForceInactive();
		}
	}

	private void HandleActionEvent(object sender, EventArgs e)
	{
		ActionEventCommand actionEventCommand = (ActionEventCommand)sender;
		actionEventCommand.isReady = true;
		bool flag = false;
		if (m_ExecutionType == ActionEventExecutionType.Any)
		{
			flag = true;
		}
		else if (m_ExecutionType == ActionEventExecutionType.All)
		{
			flag = CheckAllStatus(actionEventCommand.ID);
		}
		else if (m_ExecutionType == ActionEventExecutionType.Sequence)
		{
			flag = CheckSequenceStatus(actionEventCommand.ID);
		}
		if (flag)
		{
			ExecuteRecievers();
		}
		this.OnEvent.Send(actionEventCommand);
	}

	public void ForceInactive()
	{
		for (int i = 0; i < m_Senders.Length; i++)
		{
			m_Senders[i].ActionEventController.Content?.ForceInactive();
		}
	}

	public void ForceActive()
	{
		for (int i = 0; i < m_Senders.Length; i++)
		{
			m_Senders[i].ActionEventController.Content?.ForceActive();
		}
	}

	public void ForceActivateComplete()
	{
		for (int i = 0; i < m_Senders.Length; i++)
		{
			m_Senders[i].ActionEventController.Content?.ForceActivateComplete();
		}
	}

	public void Enable()
	{
		for (int i = 0; i < m_Senders.Length; i++)
		{
			m_Senders[i]?.ActionEventController.Content?.Enable();
		}
	}

	public void Disable()
	{
		if (m_Senders != null)
		{
			for (int i = 0; i < m_Senders.Length; i++)
			{
				m_Senders[i]?.ActionEventController.Content?.Disable();
			}
		}
	}

	public void DisableRecievers()
	{
		if (m_Recievers != null)
		{
			for (int i = 0; i < m_Recievers.Length; i++)
			{
				m_Recievers[i]?.ActionEventController.Content?.Disable();
			}
		}
	}

	private bool CheckAllStatus(int index)
	{
		bool result = true;
		for (int i = 0; i < m_ExecutionSequence.Count; i++)
		{
			if (!m_ExecutionSequence[i])
			{
				if (index == i)
				{
					m_ExecutionSequence[i] = true;
				}
				else
				{
					result = false;
				}
			}
		}
		return result;
	}

	private bool CheckSequenceStatus(int index)
	{
		bool result = true;
		m_ExecutionSequence[index] = true;
		m_ExecutionSequenceOrder++;
		if (m_ExecutionSequenceOrder < m_ExecutionSequence.Count)
		{
			if (m_ExecutionSequenceOrder != index)
			{
				ResetExecutionSequence();
				result = false;
			}
			else
			{
				for (int i = 0; i < m_ExecutionSequence.Count; i++)
				{
					bool flag = m_ExecutionSequence[i];
					if (i <= m_ExecutionSequenceOrder)
					{
						if (!flag)
						{
							result = false;
							ResetExecutionSequence();
							break;
						}
						continue;
					}
					result = false;
					break;
				}
			}
		}
		return result;
	}

	private void ResetExecutionSequence()
	{
		for (int i = 0; i < m_ExecutionSequence.Count; i++)
		{
			m_ExecutionSequence[i] = false;
		}
		if (m_FailureOnReset)
		{
			m_ExecutionSequenceOrder = -1;
			ActionEventCommand[] senders = m_Senders;
			foreach (ActionEventCommand obj in senders)
			{
				obj.isReady = false;
				obj.ActionEventController.Content.ForceDeactivate();
			}
		}
	}

	private void ExecuteRecievers()
	{
		if (m_DisableOnSuccess)
		{
			RemoveListeners();
			for (int i = 0; i < m_Senders.Length; i++)
			{
				m_Senders[i].ActionEventController.Content.Disable();
			}
		}
		if (m_HasExecutionDelay)
		{
			m_Sequencer.New();
		}
		if (m_Recievers == null || m_Recievers.Length == 0)
		{
			SendOnComplete();
			return;
		}
		for (int j = 0; j < m_Recievers.Length; j++)
		{
			ActionEventCommand actionEventCommand = m_Recievers[j];
			if (m_HasExecutionDelay)
			{
				float atPosition = m_ExecutionDelay;
				if (!m_IsExecutionDelaySingle)
				{
					atPosition = m_ExecutionDelay + m_ExecutionDelay * (float)j;
				}
				m_Sequencer.Insert(atPosition, actionEventCommand.Execute);
				if (j == m_Recievers.Length - 1)
				{
					m_Sequencer.Insert(atPosition, SendOnComplete);
				}
			}
			else
			{
				actionEventCommand.Execute();
				SendOnComplete();
			}
		}
	}

	private void SendOnComplete()
	{
		this.OnComplete.Send(this);
	}

	private void AddListeners()
	{
		for (int i = 0; i < m_Senders.Length; i++)
		{
			ActionEventCommand obj = m_Senders[i];
			obj.OnEvent += HandleActionEvent;
			obj.AddListeners();
		}
	}

	private void RemoveListeners()
	{
		for (int i = 0; i < m_Senders.Length; i++)
		{
			ActionEventCommand obj = m_Senders[i];
			obj.OnEvent -= HandleActionEvent;
			obj.RemoveListeners();
		}
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		base.OnDisposed();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "ActionEventConnector Icon", allowScaling: true);
	}

	private void OnDrawGizmosSelected()
	{
		if (m_Senders != null)
		{
			Gizmos.color = Color.white;
			for (int i = 0; i < m_Senders.Length; i++)
			{
				if (m_Senders[i].ActionEventController != null)
				{
					Gizmos.DrawLine(base.transform.position, m_Senders[i].ActionEventController.InteractableTransform.position);
				}
			}
		}
		if (m_Recievers == null)
		{
			return;
		}
		Gizmos.color = Color.cyan;
		for (int j = 0; j < m_Recievers.Length; j++)
		{
			if (m_Recievers[j].ActionEventController != null)
			{
				Gizmos.DrawLine(base.transform.position, m_Recievers[j].ActionEventController.InteractableTransform.position);
			}
		}
	}
}
