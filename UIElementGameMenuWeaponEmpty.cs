using TMPro;
using UnityEngine;

public class UIElementGameMenuWeaponEmpty : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_Label;

	private UIElementGameMenuWeaponEmptyDataVO m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		m_DataVO = (UIElementGameMenuWeaponEmptyDataVO)_data;
		m_Label.text = m_DataVO.Label;
	}
}
