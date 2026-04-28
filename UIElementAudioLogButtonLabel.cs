using System;
using DG.Tweening;
using S13Audio.BATDR;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementAudioLogButtonLabel : UIElementButton
{
	[Header("Button Label")]
	[SerializeField]
	protected UIElementLabel m_Label;

	[Header("Button")]
	[SerializeField]
	protected UIButton m_PlayButton;

	[SerializeField]
	private RectTransform m_PlayContainer;

	[SerializeField]
	private RectTransform m_StopContainer;

	[SerializeField]
	private Image m_PlayIcon;

	[SerializeField]
	private Image m_PlayIconController;

	[SerializeField]
	private TextMeshProUGUI m_PlayInputLabel;

	[SerializeField]
	private Image m_StopIcon;

	[SerializeField]
	private Image m_StopIconController;

	[SerializeField]
	private TextMeshProUGUI m_StopInputLabel;

	[Header("Filler")]
	[SerializeField]
	protected Image m_SelectImage;

	[Header("Label Settings")]
	[SerializeField]
	protected float m_EnterSize = 100f;

	[SerializeField]
	protected float m_ExitSize = 76f;

	[SerializeField]
	protected Color m_ActiveColor;

	[SerializeField]
	protected Color m_InactiveColor;

	private Sequencer m_Sequencer;

	private UIElementAudioLogDataVO m_DataVO;

	public string Label => m_Label.Label.text;

	public UIElementAudioLogDataVO DataVO => m_DataVO;

	public bool IsPlaying { get; private set; }

	public event EventHandler OnPlay;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		m_DataVO = (UIElementAudioLogDataVO)_data;
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
		m_PlayButton.gameObject.SetActive(value: false);
	}

	public void ShowPlayButton()
	{
		m_PlayButton.gameObject.SetActive(value: true);
		ShowPlayIcon();
		m_PlayButton.OnClick -= HandlePlayButtonOnClick;
		m_PlayButton.OnClick += HandlePlayButtonOnClick;
	}

	private void HandlePlayButtonOnClick(object sender, EventArgs e)
	{
		this.OnPlay.Send(this);
	}

	public void PlayAudioClip(BATDRAudioLogPlayer bATDRAudioLogPlayer)
	{
		GameManager.Instance.ClearAudioLog();
		BATDRAudioLogPlayer.Instance = bATDRAudioLogPlayer;
		AudioClip audioLogClip = GameManager.Instance.GetAudioLogClip(m_DataVO.ID);
		if (audioLogClip != null)
		{
			IsPlaying = true;
			ShowStopIcon();
			BATDRAudioLogPlayer.Play(audioLogClip);
			m_SelectImage.DOFillAmount(0f, audioLogClip.length).SetEase(Ease.Linear).SetUpdate(UpdateType.Normal, isIndependentUpdate: true)
				.OnComplete(StopAudioClip);
		}
	}

	public void StopAudioClip()
	{
		IsPlaying = false;
		ShowPlayIcon();
		BATDRAudioLogPlayer.ForceStop();
		m_SelectImage.DOKill();
		m_SelectImage.fillAmount = 1f;
	}

	private void ShowPlayIcon()
	{
		m_PlayContainer.gameObject.SetActive(value: true);
		m_PlayIconController.enabled = GameManager.Instance.HasController;
		m_PlayIcon.enabled = !GameManager.Instance.HasController;
		m_StopContainer.gameObject.SetActive(value: false);
		m_StopInputLabel.text = string.Empty;
		if (GameManager.Instance.HasController)
		{
			m_PlayInputLabel.text = "A";
		}
		else
		{
			m_PlayInputLabel.text = string.Empty;
		}
	}

	private void ShowStopIcon()
	{
		m_StopContainer.gameObject.SetActive(value: true);
		m_StopIconController.enabled = GameManager.Instance.HasController;
		m_StopIcon.enabled = !GameManager.Instance.HasController;
		m_PlayContainer.gameObject.SetActive(value: false);
		m_PlayInputLabel.text = string.Empty;
		if (GameManager.Instance.HasController)
		{
			m_StopInputLabel.text = "A";
		}
		else
		{
			m_StopInputLabel.text = string.Empty;
		}
	}

	private void CancelAudioClip()
	{
		m_PlayButton.OnClick -= HandlePlayButtonOnClick;
		m_PlayButton.gameObject.SetActive(value: false);
		StopAudioClip();
	}

	public override void ForceOnEnter()
	{
		m_Label.Label.color = m_ActiveColor;
		m_Label.Label.fontSize = m_EnterSize;
		if ((bool)m_SelectImage)
		{
			m_SelectImage.enabled = true;
			m_SelectImage.color = new Color(m_SelectImage.color.r, m_SelectImage.color.g, m_SelectImage.color.b, 1f);
		}
	}

	public override void ForceOnExit()
	{
		m_PlayButton.OnClick -= HandlePlayButtonOnClick;
		m_PlayButton.gameObject.SetActive(value: false);
		StopAudioClip();
		m_Label.Label.color = m_InactiveColor;
		m_Label.Label.fontSize = m_ExitSize;
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
	}

	protected override void InternalHandleButtonOnEnter(object sender, EventArgs e)
	{
		m_Sequencer?.CreateSequence(m_Label.Label.DOFontSize(m_EnterSize, 0.1f), m_Label.Label.DOColor(m_ActiveColor, 0.1f));
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
		CancelAudioClip();
		m_Sequencer?.Dispose();
		m_Sequencer = null;
		m_SelectImage.DOKill();
		base.OnDisposed();
	}
}
