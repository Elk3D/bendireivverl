using System;
using UnityEngine;

public class CompanionNode : CharacterNode
{
	[SerializeField]
	private CompanionNodeType m_NodeType;

	[SerializeField]
	private Transform m_SitLocation;

	[SerializeField]
	private ActionEventController m_ActionEventController;

	[SerializeField]
	private string m_EventCompleteReaction = "Negative";

	public CompanionNodeType NodeType => m_NodeType;

	public Transform SitLocation => m_SitLocation;

	public ActionEventController ActionEventController => m_ActionEventController;

	public string EventCompleteReaction => m_EventCompleteReaction;

	public event EventHandler OnComplete;

	public void Enable()
	{
		if (m_ActionEventController != null)
		{
			m_ActionEventController.Content.Enable();
		}
	}

	public void Disable()
	{
		if (m_ActionEventController != null && m_ActionEventController.Content != null && !m_ActionEventController.Content.IsActivated)
		{
			m_ActionEventController.Content.Disable();
		}
	}

	protected override void InternalOnNodeReached()
	{
		if (m_NodeType == CompanionNodeType.Point && m_ActionEventController != null)
		{
			m_ActionEventController.OnActivate -= HandleActionEventControllerOnActivate;
			m_ActionEventController.OnActivate += HandleActionEventControllerOnActivate;
			Enable();
		}
	}

	private void HandleActionEventControllerOnActivate(object sender, EventArgs e)
	{
		m_ActionEventController.OnActivate -= HandleActionEventControllerOnActivate;
		this.OnComplete.Send(this);
	}
}
