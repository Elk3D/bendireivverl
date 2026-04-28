public class UIElementGameMenuSettingsButton : UIElementButtonLabel
{
	private GameMenuSettingsType m_SettingsType;

	public GameMenuSettingsType SettingsType => m_SettingsType;

	protected override bool m_ForceTextSize => false;

	public void SetType(GameMenuSettingsType settingsType)
	{
		m_SettingsType = settingsType;
	}
}
