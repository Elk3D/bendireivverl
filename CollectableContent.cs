using System;

public class CollectableContent : ActionEventContent<CollectableContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public InteractableAnimation InteractableAnimation;

		public bool DisposeOnAction;
	}

	protected override void OnInitialized()
	{
		base.Connectable.OnActivate -= HandleConnectableOnActivate;
		base.Connectable.OnActivate += HandleConnectableOnActivate;
	}

	private void HandleConnectableOnActivate(object sender, EventArgs e)
	{
		base.Connectable.OnActivate -= HandleConnectableOnActivate;
		bool flag = false;
		for (int i = 0; i < m_Properties.Length; i++)
		{
			Properties properties = m_Properties[i];
			if (!flag)
			{
				flag = properties.DisposeOnAction;
			}
		}
		if (flag)
		{
			base.Connectable.Dispose();
		}
	}

	protected override void InternalEnable()
	{
		for (int i = 0; i < m_Properties.Length; i++)
		{
			m_Properties[i].InteractableAnimation?.SetActive(active: true);
		}
	}
}
