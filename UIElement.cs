using UnityEngine;

public class UIElement : JMonoBehaviour, IUIElement
{
	private RectTransform m_RectTransform;

	public RectTransform rectTransform => m_RectTransform ?? (m_RectTransform = GetComponent<RectTransform>());

	public string PrefabKey { get; private set; }

	public virtual void Initialize(object _data)
	{
		if (_data != null)
		{
			PrefabKey = (_data as UIElementDataVO)?.PrefabKey;
		}
	}
}
