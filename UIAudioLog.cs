using DG.Tweening;
using InControl;
using S13Audio.BATDR;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAudioLog : UIController
{
	[Header("Content")]
	[SerializeField]
	private Image m_Filler;

	[SerializeField]
	private RectTransform m_Reel_01;

	[SerializeField]
	private RectTransform m_Reel_02;

	[Header("Labels")]
	[SerializeField]
	private TextMeshProUGUI m_Label;

	[SerializeField]
	private TextMeshProUGUI m_ListenLaterLabel;

	[Header("Keyboard")]
	[SerializeField]
	private GameObject m_KeyboardContainer;

	[SerializeField]
	private TextMeshProUGUI m_KeyboardInputLabel;

	[Header("Controller")]
	[SerializeField]
	private GameObject m_ControllerContainer;

	[SerializeField]
	private Image m_ControllerInputImage;

	private Sequence m_Sequence;

	private bool m_CanCancel;

	private bool m_IsPlayingOut;

	protected override void OnInitialized(object _data)
	{
	}

	public void Show(string name, AudioClip audioClip)
	{
		CheckInput();
		m_ListenLaterLabel.text = TextUtility.GetKey("UI_AUDIO_LOG_LISTEN_LATER");
		m_Label.text = name;
		m_Filler.fillAmount = 0f;
		m_Reel_01.DOKill();
		m_Reel_02.DOKill();
		m_Reel_01.DOLocalRotate(new Vector3(0f, 0f, -360f), 0.75f, RotateMode.LocalAxisAdd).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
		m_Reel_02.DOLocalRotate(new Vector3(0f, 0f, 360f), 0.75f, RotateMode.LocalAxisAdd).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
		ResetSequence();
		m_Sequence.Insert(0f, m_Filler.DOFillAmount(1f, audioClip.length).SetEase(Ease.Linear));
		m_Sequence.InsertCallback(0.5f, delegate
		{
			m_CanCancel = true;
		});
		m_Sequence.OnComplete(PlayOut);
		BATDRAudioLogPlayer.Play(audioClip);
	}

	public override void PlayOut()
	{
		m_CanCancel = false;
		m_IsPlayingOut = true;
		m_Reel_01.DOKill();
		m_Reel_02.DOKill();
		KillSequence();
		BATDRAudioLogPlayer.Stop(InternalPlayOutComplete);
	}

	private void InternalPlayOutComplete()
	{
		if (!base.IsDisposed)
		{
			PlayOutComplete();
		}
	}

	private void Update()
	{
		if (!m_IsPlayingOut && m_CanCancel && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			CheckInput();
			if (PlayerInput.Cancel())
			{
				PlayOut();
			}
		}
	}

	private void CheckInput()
	{
		if (!GameManager.Instance.HasController)
		{
			if (m_ControllerContainer != null)
			{
				m_ControllerContainer.SetActive(value: false);
			}
			if (m_KeyboardContainer != null)
			{
				m_KeyboardContainer.SetActive(value: true);
			}
			m_KeyboardInputLabel.text = PlayerInputConstants.CANCEL;
		}
		else
		{
			if (m_KeyboardContainer != null)
			{
				m_KeyboardContainer.SetActive(value: false);
			}
			if (m_ControllerContainer != null)
			{
				m_ControllerContainer.SetActive(value: true);
			}
			m_ControllerInputImage.sprite = GameManager.Instance.ControllerInput.GetSprite(InputControlType.Action2);
		}
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			m_Sequence.Kill();
			m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		m_Reel_01.DOKill();
		m_Reel_02.DOKill();
		KillSequence();
		base.OnDisposed();
	}
}
