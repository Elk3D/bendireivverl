using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIElementMemoButtonLabel : UIElementButton
{
	[Header("Button Label")]
	[SerializeField]
	protected UIElementLabel m_Label;

	[Header("Label Settings")]
	[SerializeField]
	protected float m_EnterSize = 100f;

	[SerializeField]
	protected float m_ExitSize = 76f;

	[SerializeField]
	protected Color m_ActiveColor;

	[SerializeField]
	protected Color m_InactiveColor;

	[SerializeField]
	protected Material m_FontMaterialEnter;

	[SerializeField]
	protected Material m_FontMaterialExit;

	[SerializeField]
	protected Image m_SelectImage;

	private Sequencer m_Sequencer;

	private UIElementMemoDataVO m_DataVO;

	public string Label => m_Label.Label.text;

	public UIElementMemoDataVO DataVO => m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		m_DataVO = (UIElementMemoDataVO)_data;
		m_Sequencer = new Sequencer(isIndependent: true);
		if (m_DataVO.Button.LabelDataVO != null)
		{
			m_Label.Label.text = m_DataVO.Button.LabelDataVO.Label;
			m_Label.Label.color = m_DataVO.Button.LabelDataVO.Color;
			m_Label.Label.fontSizeMax = m_DataVO.Button.LabelDataVO.FontSize;
		}
		else
		{
			m_Label.Label.text = m_DataVO.Button.Label;
			m_Label.Label.color = m_InactiveColor;
			m_Label.Label.fontSizeMax = m_ExitSize;
		}
		SelectImageOff();
	}

	public override void ForceOnEnter()
	{
		m_Label.Label.color = m_ActiveColor;
		m_Label.Label.fontSize = m_EnterSize;
		if ((bool)m_FontMaterialEnter)
		{
			m_Label.Label.fontSharedMaterial = m_FontMaterialEnter;
		}
		if ((bool)m_SelectImage)
		{
			m_SelectImage.enabled = true;
			m_SelectImage.color = new Color(m_SelectImage.color.r, m_SelectImage.color.g, m_SelectImage.color.b, 1f);
		}
	}

	public override void ForceOnExit()
	{
		m_Label.Label.color = m_InactiveColor;
		m_Label.Label.fontSize = m_ExitSize;
		if ((bool)m_FontMaterialExit)
		{
			m_Label.Label.fontSharedMaterial = m_FontMaterialExit;
		}
		SelectImageOff();
	}

	private void SelectImageOff()
	{
		if ((bool)m_SelectImage)
		{
			m_SelectImage.enabled = false;
			m_SelectImage.color = new Color(m_SelectImage.color.r, m_SelectImage.color.g, m_SelectImage.color.b, 0f);
		}
	}

	protected override void InternalHandleButtonOnExit(object sender, EventArgs e)
	{
		m_Sequencer?.CreateSequence(m_Label.Label.DOFontSize(m_ExitSize, 0.1f), m_Label.Label.DOColor(m_InactiveColor, 0.1f));
		if ((bool)m_FontMaterialExit)
		{
			m_Label.Label.fontSharedMaterial = m_FontMaterialExit;
		}
	}

	protected override void InternalHandleButtonOnEnter(object sender, EventArgs e)
	{
		m_Sequencer?.CreateSequence(m_Label.Label.DOFontSize(m_EnterSize, 0.1f), m_Label.Label.DOColor(m_ActiveColor, 0.1f));
		if ((bool)m_FontMaterialEnter)
		{
			m_Label.Label.fontSharedMaterial = m_FontMaterialEnter;
		}
	}

	protected override void InteralResetButton()
	{
		ForceOnExit();
		ResetListeners();
	}

	public void ResetListeners()
	{
		RemoveListeners();
		AddListeners();
	}

	protected override void OnDisposed()
	{
		m_Sequencer?.Dispose();
		m_Sequencer = null;
		base.OnDisposed();
	}
}
