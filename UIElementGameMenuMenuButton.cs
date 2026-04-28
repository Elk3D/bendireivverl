public class UIElementGameMenuMenuButton : UIElementButtonLabel
{
	private GameMenuMenuType m_MenuType;

	public GameMenuMenuType MenuType => m_MenuType;

	protected override bool m_ForceTextSize => false;

	public void SetType(GameMenuMenuType menuType)
	{
		m_MenuType = menuType;
	}
}
