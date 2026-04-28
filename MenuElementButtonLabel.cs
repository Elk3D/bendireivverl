using System;
using DG.Tweening;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuElementButtonLabel : MenuElementButton
{
	[Header("Button Label")]
	[SerializeField]
	protected TextMeshProUGUI m_Label;

	[Header("Label Settings")]
	[SerializeField]
	protected float m_EnterSize = 100f;

	[SerializeField]
	protected float m_ExitSize = 76f;

	[Space]
	[SerializeField]
	protected Color m_ActiveColor;

	[SerializeField]
	protected Color m_InactiveColor;

	[SerializeField]
	protected Color m_ActiveColorReal;

	[SerializeField]
	protected Color m_InactiveColorReal;

	[Header("Controller")]
	[SerializeField]
	private RectTransform m_ControllerButton;

	[SerializeField]
	private Image m_ControllerButtonIcon;

	[SerializeField]
	private Image m_ControllerButtonInput;

	protected Sequencer m_Sequencer;

	public TextMeshProUGUI Label => m_Label;

	protected Color ActiveColor
	{
		get
		{
			if (!GameManager.Instance.IsRealWorld)
			{
				return m_ActiveColor;
			}
			return m_ActiveColorReal;
		}
	}

	protected Color InactiveColor
	{
		get
		{
			if (!GameManager.Instance.IsRealWorld)
			{
				return m_InactiveColor;
			}
			return m_InactiveColorReal;
		}
	}

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		m_Label.text = (_data as UIElementButtonDataVO).Label;
		m_Label.alignment = (_data as UIElementButtonDataVO).Alignment;
		m_Label.color = InactiveColor;
		m_Label.fontSize = m_ExitSize;
		m_Sequencer = new Sequencer(isIndependent: true);
		if (m_ControllerButton != null)
		{
			m_ControllerButton.gameObject.SetActive(value: false);
			if (_data is UIElementButtonDataVO uIElementButtonDataVO && GameManager.Instance.HasController && uIElementButtonDataVO.InputControlType != InputControlType.None)
			{
				Vector2 sizeDelta = Label.rectTransform.sizeDelta;
				sizeDelta.x = Label.GetPreferredValues().x;
				Label.rectTransform.sizeDelta = sizeDelta;
				m_Button.GetComponent<RectTransform>().sizeDelta = sizeDelta;
				Vector2 zero = Vector2.zero;
				zero.x -= m_Button.GetComponent<RectTransform>().sizeDelta.x / 2f + m_ControllerButton.sizeDelta.x / 2f;
				m_ControllerButton.anchoredPosition = zero;
				m_ControllerButtonIcon.color = InactiveColor;
				m_ControllerButtonInput.color = ActiveColor;
				m_ControllerButtonInput.sprite = GameManager.Instance.ControllerInput.GetSprite(uIElementButtonDataVO.InputControlType);
				m_ControllerButton.gameObject.SetActive(value: true);
			}
		}
	}

	public override void ForceOnEnter()
	{
		m_Label.color = ActiveColor;
		m_Label.fontSize = m_EnterSize;
	}

	public override void ForceOnExit()
	{
		m_Label.color = InactiveColor;
		m_Label.fontSize = m_ExitSize;
	}

	protected override void HandleButtonOnExit(object sender, EventArgs e)
	{
		m_Sequencer?.CreateSequence(m_Label.DOFontSize(m_ExitSize, 0.1f), m_Label.DOColor(InactiveColor, 0.1f));
	}

	protected override void HandleButtonOnEnter(object sender, EventArgs e)
	{
		m_Sequencer?.CreateSequence(m_Label.DOFontSize(m_EnterSize, 0.1f), m_Label.DOColor(ActiveColor, 0.1f));
	}

	protected override void OnDisposed()
	{
		m_Sequencer?.Dispose();
		m_Sequencer = null;
		base.OnDisposed();
	}
}
