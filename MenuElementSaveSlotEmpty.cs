using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuElementSaveSlotEmpty : UIElement
{
	[Header("Labels")]
	[SerializeField]
	private TextMeshProUGUI m_SaveSlotLabel;

	[SerializeField]
	private TextMeshProUGUI m_NewGameSaveLabel;

	[Header("Images")]
	[SerializeField]
	private Image[] m_Images;

	[Header("Colors")]
	[SerializeField]
	protected Color m_Color;

	[SerializeField]
	protected Color m_ColorReal;

	private UIElementSaveSlotEmptyDataVO m_DataVO;

	protected Color Color
	{
		get
		{
			if (!GameManager.Instance.IsRealWorld)
			{
				return m_Color;
			}
			return m_ColorReal;
		}
	}

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		m_DataVO = _data as UIElementSaveSlotEmptyDataVO;
		Image[] images = m_Images;
		for (int i = 0; i < images.Length; i++)
		{
			images[i].color = Color;
		}
		m_SaveSlotLabel.text = m_DataVO.SaveSlotLabel;
		m_SaveSlotLabel.color = Color;
		m_NewGameSaveLabel.text = m_DataVO.NewGameSaveLabel;
		m_NewGameSaveLabel.color = Color;
	}

	protected override void OnDisposed()
	{
		m_DataVO = null;
		base.OnDisposed();
	}
}
