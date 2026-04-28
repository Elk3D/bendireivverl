using TMPro;
using UnityEngine;

public class UIElementMemory : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_MemoLabel;

	private UIElementMemoryDataVO m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		m_DataVO = (UIElementMemoryDataVO)_data;
		m_MemoLabel.text = TextUtility.GetKey(m_DataVO.ID.ToString());
	}

	protected override void OnDisposed()
	{
		m_DataVO = null;
		base.OnDisposed();
	}
}
