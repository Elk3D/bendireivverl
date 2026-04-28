using System;
using UnityEngine;

public class ActionEventControllerRequirement : JMonoBehaviour
{
	[SerializeField]
	private ActionEventController m_Controller;

	[SerializeField]
	private Requirements m_Requirements;

	public event EventHandler OnAction;

	public override void Start()
	{
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		m_Controller.OnDeactivate -= HandleControllerOnDeactivate;
		if (!CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
			m_Controller.OnDeactivate += HandleControllerOnDeactivate;
		}
	}

	private bool CheckStatus()
	{
		bool flag = true;
		if (m_Requirements != null)
		{
			flag = m_Requirements.IsComplete();
		}
		if (flag)
		{
			GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
			m_Controller.OnDeactivate -= HandleControllerOnDeactivate;
		}
		return flag;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("ActionEventControllerRequirement :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
		CheckStatus();
	}

	private void HandleControllerOnDeactivate(object sender, EventArgs e)
	{
		JDebug.Log("ActionEventControllerRequirement :: HandleControllerOnActivated", this, JDebug.JDebugType.Objectives);
		this.OnAction.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnAction = null;
		base.OnDisposed();
	}
}
