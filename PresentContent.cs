using System;

public class PresentContent : ActionEventContent<PresentContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
	}

	private Present m_Present;

	protected override void OnInitialize()
	{
		m_Present = (Present)base.Connectable;
		InitializeContent();
	}

	public void InitializeContent()
	{
		RemoveListeners();
		AddListeners();
		base.Connectable.OnActivate -= HandleConnectableOnActivate;
		base.Connectable.OnActivate += HandleConnectableOnActivate;
	}

	private void HandleConnectableOnActivate(object sender, EventArgs e)
	{
		base.Connectable.OnActivate -= HandleConnectableOnActivate;
		base.Connectable.Dispose();
	}

	protected override void OnActivate()
	{
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey(m_Present.Data.PresentName));
		PresentCheck.Collect();
	}

	protected override void OnDisposed()
	{
		if (base.Connectable != null)
		{
			base.Connectable.OnActivate -= HandleConnectableOnActivate;
		}
		m_Present = null;
		base.OnDisposed();
	}
}
