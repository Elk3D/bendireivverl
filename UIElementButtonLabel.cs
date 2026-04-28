using System;
using DG.Tweening;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementButtonLabel : UIElementButton
{
	[Header("Button Label")]
	[SerializeField]
	protected UIElementLabel m_Label;

	[Header("Select Image")]
	[SerializeField]
	protected Image m_SelectImage;

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

	[SerializeField]
	protected Color m_DisabledColor;

	[Header("Controller")]
	[SerializeField]
	private RectTransform m_ControllerButton;

	[SerializeField]
	private Image m_ControllerButtonIcon;

	[SerializeField]
	private Image m_ControllerButtonInput;

	private Sequencer m_Sequencer;

	protected bool m_IsDisabled;

	public string Label => m_Label.Label.text;

	public TextMeshProUGUI LabelUI => m_Label.Label;

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

	protected Color DisabledColor
	{
		get
		{
			if (GameManager.Instance.IsRealWorld)
			{
				Color inactiveColorReal = m_InactiveColorReal;
				inactiveColorReal.a = 0.25f;
				return inactiveColorReal;
			}
			return m_DisabledColor;
		}
	}

	protected virtual bool m_ForceTextSize => true;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		if (m_ControllerButton != null)
		{
			m_ControllerButton.gameObject.SetActive(value: false);
		}
		m_Sequencer = new Sequencer(isIndependent: true);
		if (_data is UIElementButtonDataVO uIElementButtonDataVO)
		{
			if (uIElementButtonDataVO.LabelDataVO != null)
			{
				m_Label.Label.text = uIElementButtonDataVO.LabelDataVO.Label;
				m_Label.Label.color = uIElementButtonDataVO.LabelDataVO.Color;
				m_Label.Label.fontSizeMax = uIElementButtonDataVO.LabelDataVO.FontSize;
			}
			else
			{
				m_Label.Label.text = uIElementButtonDataVO.Label;
				m_Label.Label.color = InactiveColor;
				m_Label.Label.fontSizeMax = m_ExitSize;
			}
		}
		else if (_data is UIElementTitleMenuButtonDataVO uIElementTitleMenuButtonDataVO)
		{
			m_Label.Label.text = uIElementTitleMenuButtonDataVO.Label;
			m_Label.Label.color = InactiveColor;
			m_Label.Label.fontSizeMax = m_ExitSize;
		}
		else if (_data is UIElementGameMenuSettingsButtonDataVO uIElementGameMenuSettingsButtonDataVO)
		{
			m_Label.Label.text = uIElementGameMenuSettingsButtonDataVO.Label;
			m_Label.Label.color = InactiveColor;
			m_Label.Label.fontSizeMax = m_ExitSize;
			m_Label.Label.fontSize = m_ExitSize;
			m_Label.Label.alignment = TextAlignmentOptions.MidlineLeft;
		}
		else if (_data is UIElementGameMenuMenuButtonDataVO uIElementGameMenuMenuButtonDataVO)
		{
			m_Label.Label.text = uIElementGameMenuMenuButtonDataVO.Label;
			m_Label.Label.color = InactiveColor;
			m_Label.Label.fontSizeMax = m_ExitSize;
			m_Label.Label.fontSize = m_ExitSize;
			m_Label.Label.alignment = TextAlignmentOptions.MidlineLeft;
		}
		if ((bool)m_SelectImage)
		{
			m_SelectImage.enabled = false;
			m_SelectImage.color = new Color(m_SelectImage.color.r, m_SelectImage.color.g, m_SelectImage.color.b, 0f);
		}
		if (m_ForceTextSize)
		{
			Vector2 sizeDelta = m_Label.Label.rectTransform.sizeDelta;
			sizeDelta.x = m_Label.Label.GetPreferredValues().x;
			m_Label.Label.rectTransform.sizeDelta = sizeDelta;
			m_Button.GetComponent<RectTransform>().sizeDelta = sizeDelta;
		}
		if (_data is UIElementButtonDataVO uIElementButtonDataVO2 && GameManager.Instance.HasController && m_ControllerButton != null && uIElementButtonDataVO2.InputControlType != InputControlType.None)
		{
			Vector2 zero = Vector2.zero;
			zero.x -= m_Button.GetComponent<RectTransform>().sizeDelta.x / 2f + m_ControllerButton.sizeDelta.x / 2f;
			m_ControllerButton.anchoredPosition = zero;
			m_ControllerButtonIcon.color = InactiveColor;
			m_ControllerButtonInput.color = ActiveColor;
			m_ControllerButtonInput.sprite = GameManager.Instance.ControllerInput.GetSprite(uIElementButtonDataVO2.InputControlType);
			m_ControllerButton.gameObject.SetActive(value: true);
		}
	}

	public void SetDisabled(bool disabled)
	{
		m_IsDisabled = disabled;
		if (m_IsDisabled)
		{
			m_Label.Label.color = DisabledColor;
		}
	}

	public override void ForceOnEnter()
	{
		if (!m_IsDisabled)
		{
			m_Label.Label.color = ActiveColor;
			m_Label.Label.fontSize = m_EnterSize;
			if ((bool)m_SelectImage)
			{
				m_SelectImage.enabled = true;
				m_SelectImage.color = new Color(m_SelectImage.color.r, m_SelectImage.color.g, m_SelectImage.color.b, 1f);
			}
			GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[0], isIndependentUpdate: true);
		}
		else
		{
			m_Label.Label.color = DisabledColor;
			m_Label.Label.fontSize = m_EnterSize;
		}
		SendOnEnter();
	}

	public override void ForceOnExit()
	{
		if (!m_IsDisabled)
		{
			m_Label.Label.color = InactiveColor;
			m_Label.Label.fontSize = m_ExitSize;
			if ((bool)m_SelectImage)
			{
				m_SelectImage.enabled = true;
				m_SelectImage.color = new Color(m_SelectImage.color.r, m_SelectImage.color.g, m_SelectImage.color.b, 0f);
			}
		}
		else
		{
			m_Label.Label.color = DisabledColor;
			m_Label.Label.fontSize = m_ExitSize;
		}
		SendOnExit();
	}

	public override void ForceOnClik()
	{
		SendOnClick();
	}

	protected override void InternalHandleButtonOnExit(object sender, EventArgs e)
	{
		if (!m_IsDisabled)
		{
			m_Sequencer?.CreateSequence(m_Label.Label.DOFontSize(m_ExitSize, 0.1f), m_Label.Label.DOColor(InactiveColor, 0.1f));
			if ((bool)m_SelectImage)
			{
				m_Sequencer?.Insert(0f, m_SelectImage.DOFade(0f, 0.1f).OnComplete(delegate
				{
					m_SelectImage.enabled = true;
				}));
			}
		}
		else
		{
			m_Sequencer?.CreateSequence(m_Label.Label.DOFontSize(m_ExitSize, 0.1f), m_Label.Label.DOColor(DisabledColor, 0.1f));
		}
	}

	protected override void InternalHandleButtonOnEnter(object sender, EventArgs e)
	{
		if (!m_IsDisabled)
		{
			m_Sequencer?.CreateSequence(m_Label.Label.DOFontSize(m_EnterSize, 0.1f), m_Label.Label.DOColor(ActiveColor, 0.1f));
			if ((bool)m_SelectImage)
			{
				m_SelectImage.enabled = true;
				m_Sequencer?.Insert(0f, m_SelectImage.DOFade(1f, 0.1f));
			}
		}
		else
		{
			m_Sequencer?.CreateSequence(m_Label.Label.DOFontSize(m_EnterSize, 0.1f), m_Label.Label.DOColor(DisabledColor, 0.1f));
		}
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
