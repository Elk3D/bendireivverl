using System;

[Serializable]
public class ActionEventCommand
{
	public int ID;

	public int EventHandlerID;

	public string EventHandler;

	public ActionEventController ActionEventController;

	public ActionEventCommandType ActionEventCommandType;

	public bool isReady;

	public bool isExecuted;

	public event EventHandler OnEvent;

	public void Execute()
	{
		if (ActionEventController == null)
		{
			return;
		}
		if (ActionEventCommandType == ActionEventCommandType.Action)
		{
			ActionEventController.Content.Action();
		}
		else if (ActionEventCommandType == ActionEventCommandType.Activate)
		{
			ActionEventController.Content.Activate();
		}
		else if (ActionEventCommandType == ActionEventCommandType.ForceActivate)
		{
			ActionEventController.Content.ForceActivate();
		}
		else if (ActionEventCommandType == ActionEventCommandType.Deactivate)
		{
			ActionEventController.Content.Deactivate();
		}
		else if (ActionEventCommandType == ActionEventCommandType.ForceDeactivate)
		{
			ActionEventController.Content.ForceDeactivate();
		}
		else if (ActionEventCommandType == ActionEventCommandType.Disable)
		{
			ActionEventController.Content.Disable();
		}
		else if (ActionEventCommandType == ActionEventCommandType.Enable)
		{
			ActionEventController.Content.Enable();
		}
		else
		{
			if (ActionEventCommandType != ActionEventCommandType.ForceInactive)
			{
				return;
			}
			ActionEventController.Content.ForceInactive();
		}
		isExecuted = true;
	}

	private void HandleOnEvent(object sender, EventArgs e)
	{
		this.OnEvent.Send(this);
	}

	public void AddListeners()
	{
		if (EventHandler == "OnActivate")
		{
			ActionEventController.OnActivate += HandleOnEvent;
		}
		else if (EventHandler == "OnActivated")
		{
			ActionEventController.OnActivated += HandleOnEvent;
		}
		else if (EventHandler == "OnDeactivate")
		{
			ActionEventController.OnDeactivate += HandleOnEvent;
		}
		else if (EventHandler == "OnDeactivated")
		{
			ActionEventController.OnDeactivated += HandleOnEvent;
		}
		else if (EventHandler == "OnInactive")
		{
			ActionEventController.OnInactive += HandleOnEvent;
		}
		else if (EventHandler == "OnDisabled")
		{
			ActionEventController.OnDisabled += HandleOnEvent;
		}
		else if (EventHandler == "OnEnabled")
		{
			ActionEventController.OnEnabled += HandleOnEvent;
		}
	}

	public void RemoveListeners()
	{
		ActionEventController.OnActivate -= HandleOnEvent;
		ActionEventController.OnActivated -= HandleOnEvent;
		ActionEventController.OnDeactivate -= HandleOnEvent;
		ActionEventController.OnDeactivated -= HandleOnEvent;
		ActionEventController.OnInactive -= HandleOnEvent;
		ActionEventController.OnDisabled -= HandleOnEvent;
		ActionEventController.OnEnabled -= HandleOnEvent;
	}
}
