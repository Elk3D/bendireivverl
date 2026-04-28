using DG.Tweening;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICutsceneBars : UIController
{
	[Header("Cutscene Bars")]
	[SerializeField]
	private RectTransform m_TopBar;

	[SerializeField]
	private RectTransform m_BottomBar;

	[Header("Skip Content")]
	[SerializeField]
	private RectTransform m_SkipContainer;

	[SerializeField]
	private Image m_SkipFiller;

	[SerializeField]
	private Image m_SkipControllerInputImage;

	[SerializeField]
	private TextMeshProUGUI m_SkipInputLabel;

	[SerializeField]
	private TextMeshProUGUI m_SkipLabel;

	private CutsceneDirector m_CutsceneDirector;

	private Sequence m_Sequence;

	private Vector2 m_TopStartPosition;

	private Vector2 m_TopEndPosition;

	private Vector2 m_BottomStartPosition;

	private Vector2 m_BottomEndPosition;

	private bool m_IsActive;

	private bool m_CanSkip;

	private bool m_IsSkipping;

	private bool m_DisableSkip;

	private float m_InputTimer;

	private float m_InputTimerMax = 1.25f;

	private int m_SkipCount;

	private float m_SkipTimer;

	private float m_SkipTimerMax = 2f;

	private bool m_HasController;

	protected override void OnInitialized(object _data)
	{
		m_CutsceneDirector = (CutsceneDirector)_data;
		m_TopStartPosition = m_TopBar.anchoredPosition;
		m_BottomStartPosition = m_BottomBar.anchoredPosition;
		m_TopEndPosition = m_TopBar.anchoredPosition;
		m_TopEndPosition.y += m_TopBar.sizeDelta.y;
		m_TopBar.anchoredPosition = m_TopEndPosition;
		m_BottomEndPosition = m_BottomBar.anchoredPosition;
		m_BottomEndPosition.y -= m_BottomBar.sizeDelta.y;
		m_BottomBar.anchoredPosition = m_BottomEndPosition;
		m_SkipInputLabel.text = "E";
		m_SkipInputLabel.gameObject.SetActive(value: false);
		m_SkipControllerInputImage.sprite = GameManager.Instance.ControllerInput.GetSprite(InputControlType.Action1);
		m_SkipControllerInputImage.gameObject.SetActive(value: false);
		m_SkipLabel.text = TextUtility.GetKey("UI_SKIP").ToUpper();
		ClearTimer(setActive: false);
	}

	public void SkipPlayIn()
	{
		KillSequece();
		m_TopBar.anchoredPosition = m_TopStartPosition;
		m_BottomBar.anchoredPosition = m_BottomStartPosition;
		PlayInComplete();
	}

	public void DisableSkip()
	{
		m_DisableSkip = true;
	}

	public override void PlayIn()
	{
		ResetSequence();
		m_Sequence.Insert(0f, m_TopBar.DOAnchorPos(m_TopStartPosition, 1f).SetEase(Ease.OutSine));
		m_Sequence.Insert(0f, m_BottomBar.DOAnchorPos(m_BottomStartPosition, 1f).SetEase(Ease.OutSine));
		m_Sequence.InsertCallback(1.5f, PlayInComplete);
	}

	public override void PlayInComplete()
	{
		m_IsActive = true;
		base.PlayInComplete();
	}

	public override void PlayOut()
	{
		m_IsActive = false;
		ClearTimer(setActive: false);
		ResetSequence();
		m_Sequence.Insert(0f, m_TopBar.DOAnchorPos(m_TopEndPosition, 1f).SetEase(Ease.InSine));
		m_Sequence.Insert(0f, m_BottomBar.DOAnchorPos(m_BottomEndPosition, 1f).SetEase(Ease.InSine));
		m_Sequence.OnComplete(PlayOutComplete);
	}

	private void Update()
	{
		if (!m_IsActive || m_CutsceneDirector == null || m_IsSkipping || base.IsDisposed || GameManager.Instance.IsPaused || m_DisableSkip)
		{
			return;
		}
		if (!m_CanSkip)
		{
			if (PlayerInput.Any())
			{
				m_SkipCount++;
				if (m_SkipCount >= 2)
				{
					m_HasController = GameManager.Instance.HasController;
					m_CanSkip = true;
					ClearTimer(setActive: true);
				}
			}
		}
		else if (m_HasController)
		{
			m_SkipInputLabel.gameObject.SetActive(value: false);
			m_SkipControllerInputImage.gameObject.SetActive(value: true);
			if (PlayerInput.ControllerAction1Up())
			{
				ClearTimer(setActive: true);
			}
			else if (PlayerInput.ControllerAction1Hold())
			{
				CheckInputTimer();
			}
			else
			{
				CheckSkipTimer();
			}
		}
		else
		{
			m_SkipControllerInputImage.gameObject.SetActive(value: false);
			m_SkipInputLabel.gameObject.SetActive(value: true);
			if (PlayerInput.InteractOnReleased())
			{
				ClearTimer(setActive: true);
			}
			else if (PlayerInput.InteractOnHold())
			{
				CheckInputTimer();
			}
			else
			{
				CheckSkipTimer();
			}
		}
		if (m_IsSkipping)
		{
			ClearTimer(setActive: false);
			m_CutsceneDirector.Skip();
		}
	}

	private void CheckInputTimer()
	{
		m_SkipTimer = 0f;
		if (m_InputTimer < m_InputTimerMax)
		{
			m_InputTimer += Time.unscaledDeltaTime;
			float num = m_InputTimer / m_InputTimerMax;
			GameManager.Instance.TriggerRumble(0.05f, num);
			if (num >= 1f)
			{
				num = 1f;
				m_IsSkipping = true;
			}
			m_SkipFiller.fillAmount = num;
		}
	}

	private void CheckSkipTimer()
	{
		if (m_SkipTimer >= m_SkipTimerMax)
		{
			m_SkipCount = 0;
			m_CanSkip = false;
			ClearTimer(setActive: false);
		}
		else
		{
			m_SkipTimer += Time.unscaledDeltaTime;
		}
	}

	private void ClearTimer(bool setActive)
	{
		m_InputTimer = 0f;
		m_SkipFiller.fillAmount = 0f;
		m_SkipContainer.gameObject.SetActive(setActive);
	}

	private void ResetSequence()
	{
		KillSequece();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequece()
	{
		if (m_Sequence != null)
		{
			m_Sequence.Kill();
			m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		KillSequece();
		m_CutsceneDirector = null;
		base.OnDisposed();
	}
}
