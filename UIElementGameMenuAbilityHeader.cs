using TMPro;
using UnityEngine;

public class UIElementGameMenuAbilityHeader : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_HeaderLabel;

	[SerializeField]
	private TextMeshProUGUI m_DescriptionLabel;

	private UIElementGameMenuAbilityHeaderDataVO m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		m_DataVO = (UIElementGameMenuAbilityHeaderDataVO)_data;
		m_HeaderLabel.text = m_DataVO.HeaderLabel;
		m_DescriptionLabel.text = m_DataVO.DescriptionLabel;
	}
}
