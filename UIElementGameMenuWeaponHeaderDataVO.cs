public class UIElementGameMenuWeaponHeaderDataVO : UIElementDataVO
{
	public string HeaderLabel;

	public string DescriptionLabel;

	public UIElementGameMenuWeaponHeaderDataVO(string prefabKey, string headerLabel, string descriptionLabel)
		: base(prefabKey)
	{
		HeaderLabel = headerLabel;
		DescriptionLabel = descriptionLabel;
	}
}
