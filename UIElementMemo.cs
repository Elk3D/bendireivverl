using TMPro;
using UnityEngine;

public class UIElementMemo : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_MemoLabel;

	private UIElementMemoDataVO m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		m_DataVO = (UIElementMemoDataVO)_data;
		m_MemoLabel.text = TextUtility.GetKey(m_DataVO.ID.ToString());
	}

	protected override void OnDisposed()
	{
		m_DataVO = null;
		base.OnDisposed();
	}
}
