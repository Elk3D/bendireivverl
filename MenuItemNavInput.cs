using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemNavInput : JMonoBehaviour
{
	[Header("Text")]
	[SerializeField]
	private TextMeshProUGUI m_InputLbl;

	[SerializeField]
	private Localize m_InputDescriptionLbl;

	[Header("Images")]
	[SerializeField]
	private Image m_CircleImg;

	private UINavInputDataVO m_DataVO;

	private RectTransform m_RectTransform;

	public RectTransform rectTransform
	{
		get
		{
			if (m_RectTransform == null)
			{
				m_RectTransform = GetComponent<RectTransform>();
			}
			return m_RectTransform;
		}
	}

	public void Init(UINavInputDataVO vo)
	{
		m_DataVO = vo;
		m_InputLbl.text = m_DataVO.Input;
		m_InputDescriptionLbl.SetTerm(m_DataVO.Description);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
