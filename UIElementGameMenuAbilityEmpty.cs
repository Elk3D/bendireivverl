using TMPro;
using UnityEngine;

public class UIElementGameMenuAbilityEmpty : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_Label;

	private UIElementGameMenuAbilityEmptyDataVO m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		m_DataVO = (UIElementGameMenuAbilityEmptyDataVO)_data;
		m_Label.text = m_DataVO.Label;
	}
}
