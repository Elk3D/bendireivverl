using DG.Tweening;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UICredits : UIController
{
	[SerializeField]
	private RectTransform m_Container;

	[SerializeField]
	private Image m_BlackImage;

	[SerializeField]
	private Image[] m_Logos;

	[SerializeField]
	private AudioSource m_AudioSource;

	[Header("Skip")]
	[SerializeField]
	private CanvasGroup m_Canvas;

	[SerializeField]
	private Image m_SkipIcon;

	[SerializeField]
	private TextMeshProUGUI m_InputLabel;

	[SerializeField]
	private Image m_InputImage;

	[SerializeField]
	private TextMeshProUGUI m_ExitLabel;

	private TextMeshProUGUI[] m_AllText;

	private bool m_Data;

	private bool m_CanExit;

	private float m_InputTimer;

	private float m_InputTimerMax = 1.25f;

	private bool m_HasController;

	public bool IsMainMenu { get; private set; }

	public void SetMainMenu(bool isMainMenu)
	{
		IsMainMenu = isMainMenu;
	}

	protected override void OnInitialized(object _data)
	{
		if (_data != null)
		{
			m_Data = (bool)_data;
		}
		m_ExitLabel.text = TextUtility.GetKey("UI_EXIT").ToUpper();
		m_InputLabel.text = "E";
		m_InputImage.sprite = GameManager.Instance.ControllerInput.GetSprite(InputControlType.Action1);
		if (m_Data)
		{
			m_BlackImage.gameObject.SetActive(value: false);
			m_Canvas.alpha = 0f;
			m_SkipIcon.fillAmount = 0f;
		}
		else
		{
			m_Canvas.gameObject.SetActive(value: false);
			m_AllText = m_Container.GetComponentsInChildren<TextMeshProUGUI>();
		}
	}

	public override void PlayInComplete()
	{
		base.PlayInComplete();
		if (!m_Data)
		{
			GameManager.Instance.HideScreenBlocker(0f);
			m_AudioSource.Play();
			Color endValue = new Color(0.725f, 0.533f, 0.13f, 1f);
			float num = m_AudioSource.clip.length / 2f;
			float delay = num / 2f;
			for (int i = 0; i < m_AllText.Length; i++)
			{
				m_AllText[i].DOColor(endValue, num).SetDelay(delay).SetEase(Ease.Linear);
			}
			if (m_Logos != null)
			{
				for (int j = 0; j < m_Logos.Length; j++)
				{
					m_Logos[j].DOColor(endValue, num).SetDelay(delay).SetEase(Ease.Linear);
				}
			}
		}
		m_Container.DOAnchorPosY(m_Container.sizeDelta.y, m_AudioSource.clip.length).SetEase(Ease.Linear).OnComplete(CreditsOnComplete);
	}

	private void CreditsOnComplete()
	{
		if (m_Data)
		{
			m_HasController = GameManager.Instance.HasController;
			if (m_HasController)
			{
				m_InputLabel.gameObject.SetActive(value: false);
				m_InputImage.gameObject.SetActive(value: true);
			}
			else
			{
				m_InputImage.gameObject.SetActive(value: false);
				m_InputLabel.gameObject.SetActive(value: true);
			}
			m_Container.gameObject.SetActive(value: false);
			m_Canvas.DOFade(1f, 2f).OnComplete(delegate
			{
				m_CanExit = true;
			});
		}
		else
		{
			PlayOut();
		}
	}

	private void Update()
	{
		if (base.IsDisposed)
		{
			return;
		}
		if (IsMainMenu && PlayerInput.Pause())
		{
			PlayOut();
		}
		if (!m_CanExit)
		{
			return;
		}
		if (m_HasController)
		{
			if (PlayerInput.ControllerAction1Up())
			{
				ClearTimer(setActive: true);
			}
			else if (PlayerInput.ControllerAction1Hold())
			{
				CheckInputTimer();
			}
		}
		else if (PlayerInput.InteractOnReleased())
		{
			ClearTimer(setActive: true);
		}
		else if (PlayerInput.InteractOnHold())
		{
			CheckInputTimer();
		}
	}

	private void CheckInputTimer()
	{
		if (m_InputTimer < m_InputTimerMax)
		{
			m_InputTimer += Time.unscaledDeltaTime;
			float num = m_InputTimer / m_InputTimerMax;
			if (num >= 1f)
			{
				num = 1f;
				PlayOut();
			}
			m_SkipIcon.fillAmount = num;
		}
	}

	private void ClearTimer(bool setActive)
	{
		m_InputTimer = 0f;
		m_SkipIcon.fillAmount = 0f;
		m_Canvas.gameObject.SetActive(setActive);
	}

	protected override void OnDisposed()
	{
		m_Container.DOKill();
		if (!m_Data)
		{
			for (int i = 0; i < m_AllText.Length; i++)
			{
				TextMeshProUGUI textMeshProUGUI = m_AllText[i];
				if (textMeshProUGUI != null)
				{
					textMeshProUGUI.DOKill();
				}
			}
			if (m_Logos != null)
			{
				for (int j = 0; j < m_Logos.Length; j++)
				{
					Image image = m_Logos[j];
					if (image != null)
					{
						image.DOKill();
					}
				}
			}
		}
		m_AllText = null;
		m_Logos = null;
		SceneManager.LoadScene("Reset");
		base.OnDisposed();
	}
}
