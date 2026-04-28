using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementGentUpgradeComponent : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_AmountLabel;

	[SerializeField]
	private Image m_Icon;

	[SerializeField]
	private GameObject m_Off;

	[SerializeField]
	private GameObject m_On;

	[SerializeField]
	private CanvasGroup m_CanvasGroup;

	private UIElementGentUpgradeComponentDataVO m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		m_DataVO = (UIElementGentUpgradeComponentDataVO)_data;
		m_AmountLabel.text = m_DataVO.Amount + " / " + m_DataVO.Max;
		bool flag = m_DataVO.Amount >= m_DataVO.Max;
		m_Icon.sprite = m_DataVO.Icon;
		if ((bool)m_On)
		{
			m_On.SetActive(flag);
		}
		if ((bool)m_Off)
		{
			m_Off.SetActive(!flag);
		}
		if ((bool)m_CanvasGroup)
		{
			m_CanvasGroup.alpha = (flag ? 1f : 0.2f);
		}
	}

	protected override void OnDisposed()
	{
		m_DataVO = null;
		base.OnDisposed();
	}
}
