using UnityEngine;

public class UIElementGameMenuWeaponDataVO : UIElementDataVO
{
	public string HeaderLabel;

	public string DescriptionLabel;

	public Sprite UpgradeIcon;

	public UIElementGameMenuWeaponDataVO(string prefabKey, string headerLabel, string descriptionLabel, Sprite upgradeIcon)
		: base(prefabKey)
	{
		HeaderLabel = headerLabel;
		DescriptionLabel = descriptionLabel;
		UpgradeIcon = upgradeIcon;
	}
}
