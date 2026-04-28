using System;

public class TriggerContent : ActionEventContent<TriggerContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public bool OnExit;
	}

	protected override void OnInitialized()
	{
		for (int i = 0; i < m_Properties.Length; i++)
		{
			Properties properties = m_Properties[i];
			if (properties.OnExit)
			{
				properties.ActionEvent.OnExit += HandleActionEventOnExit;
				break;
			}
		}
	}

	protected override void OnActivate()
	{
	}

	private void HandleActionEventOnExit(object sender, EventArgs e)
	{
		RemoveListeners();
		ResetActionEvents();
		AddListeners();
	}
}
