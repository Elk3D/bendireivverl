using TMPro;
using UnityEngine;

public class UIElementLabel : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_Label;

	private UIElementLabelDataVO m_DataVO;

	public TextMeshProUGUI Label => m_Label;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		m_DataVO = (UIElementLabelDataVO)_data;
		m_Label.text = m_DataVO.Label;
		m_Label.color = m_DataVO.Color;
		m_Label.fontSizeMax = m_DataVO.FontSize;
		base.rectTransform.sizeDelta = m_DataVO.SizeDelta;
	}
}
