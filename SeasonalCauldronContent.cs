using System;

public class SeasonalCauldronContent : ActionEventContent<SeasonalCauldronContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
	}

	private SeasonalCauldron m_Cauldron;

	protected override void OnInitialize()
	{
		m_Cauldron = (SeasonalCauldron)base.Connectable;
		InitializeContent();
	}

	public void InitializeContent()
	{
		RemoveListeners();
		AddListeners();
	}

	private void HandleConnectableOnActivate(object sender, EventArgs e)
	{
	}

	protected override void OnDisposed()
	{
		m_Cauldron = null;
		base.OnDisposed();
	}
}
