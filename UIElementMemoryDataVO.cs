using UnityEngine;

public class UIElementMemoryDataVO : UIElementDataVO
{
	public MemoryID ID;

	public UIElementButtonDataVO Button;

	public Sprite Sprite;

	public UIElementMemoryDataVO(string prefabKey, MemoryID id, UIElementButtonDataVO button, Sprite sprite)
		: base(prefabKey)
	{
		ID = id;
		Button = button;
		Sprite = sprite;
	}
}
