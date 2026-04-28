using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementGameMenuAbility : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_HeaderLabel;

	[SerializeField]
	private TextMeshProUGUI m_DescriptionLabel;

	[SerializeField]
	private Image m_AbilityIcon;

	private UIElementGameMenuAbilityDataVO m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		m_DataVO = (UIElementGameMenuAbilityDataVO)_data;
		m_HeaderLabel.text = m_DataVO.HeaderLabel;
		m_DescriptionLabel.text = m_DataVO.DescriptionLabel;
		m_AbilityIcon.sprite = m_DataVO.AbilityIcon;
	}
}
