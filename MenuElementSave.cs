using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuElementSave : MenuElementButtonLabel
{
	[Header("Save Labels")]
	[SerializeField]
	private TextMeshProUGUI m_DateTimeLabel;

	[SerializeField]
	private TextMeshProUGUI m_ObjectiveLabel;

	[Header("Save Images")]
	[SerializeField]
	private RawImage m_SaveImage;

	[SerializeField]
	private Image m_FrameImage;

	[SerializeField]
	private Image m_DefaultImage;

	[Header("Frame")]
	[SerializeField]
	private Image m_SlotFrameImage;

	private UIElementSaveDataVO m_Data;

	public string DateTimeLabel => m_DateTimeLabel.text;

	public string ObjectiveLabel => m_ObjectiveLabel.text;

	public UIElementSaveDataVO Data => m_Data;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		m_Data = (UIElementSaveDataVO)_data;
		m_Label.text = m_Data.Label;
		m_Label.color = base.InactiveColor;
		m_DateTimeLabel.text = m_Data.DateTimeLabel;
		m_DateTimeLabel.color = base.InactiveColor;
		m_ObjectiveLabel.text = m_Data.MessageLabel;
		m_ObjectiveLabel.color = base.InactiveColor;
		m_FrameImage.color = base.InactiveColor;
		Color inactiveColor = base.InactiveColor;
		inactiveColor.a = 0f;
		m_SlotFrameImage.color = inactiveColor;
		if (m_Data.Image != null)
		{
			m_SaveImage.texture = m_Data.Image;
			m_SaveImage.enabled = true;
			m_DefaultImage.enabled = false;
		}
		m_Sequencer = new Sequencer(isIndependent: true);
	}

	protected override void HandleButtonOnEnter(object sender, EventArgs e)
	{
		ForceOnEnter();
	}

	protected override void HandleButtonOnExit(object sender, EventArgs e)
	{
		ForceOnExit();
	}

	public override void ForceOnEnter()
	{
		m_Sequencer?.CreateSequence(m_Label.DOColor(base.ActiveColor, 0.1f), m_DateTimeLabel.DOColor(base.ActiveColor, 0.1f), m_ObjectiveLabel.DOColor(base.ActiveColor, 0.1f), m_FrameImage.DOColor(base.ActiveColor, 0.1f), m_SlotFrameImage.DOColor(base.ActiveColor, 0.1f));
		GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[0]);
	}

	public override void ForceOnExit()
	{
		Color inactiveColor = base.InactiveColor;
		inactiveColor.a = 0f;
		m_Sequencer?.CreateSequence(m_Label.DOColor(base.InactiveColor, 0.1f), m_DateTimeLabel.DOColor(base.InactiveColor, 0.1f), m_ObjectiveLabel.DOColor(base.InactiveColor, 0.1f), m_FrameImage.DOColor(base.InactiveColor, 0.1f), m_SlotFrameImage.DOColor(inactiveColor, 0.1f));
	}

	protected override void OnDisposed()
	{
		m_Data = null;
		base.OnDisposed();
	}
}
