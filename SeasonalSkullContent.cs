using System;

public class SeasonalSkullContent : ActionEventContent<SeasonalSkullContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
	}

	private SeasonalSkull m_Skull;

	protected override void OnInitialize()
	{
		m_Skull = (SeasonalSkull)base.Connectable;
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
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey(m_Skull.Data.SkullName));
		SeasonalSkullCheck.Collect();
	}

	protected override void OnDisposed()
	{
		if (base.Connectable != null)
		{
			base.Connectable.OnActivate -= HandleConnectableOnActivate;
		}
		m_Skull = null;
		base.OnDisposed();
	}
}
