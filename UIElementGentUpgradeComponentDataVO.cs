using UnityEngine;

public class UIElementGentUpgradeComponentDataVO
{
	public Sprite Icon;

	public int Amount;

	public int Max;

	public UIElementGentUpgradeComponentDataVO(string icon, int amount, int max)
	{
		Icon = GameManager.Instance.AssetManager.GetAsset<IconData>(icon).Icon;
		Amount = amount;
		Max = max;
	}
}
