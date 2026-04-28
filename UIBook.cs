using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBook : UIController
{
	[Header("Images")]
	[SerializeField]
	private Image m_BG;

	[Header("Labels")]
	[SerializeField]
	private TextMeshProUGUI m_BookLabel;

	[Header("Pages")]
	[SerializeField]
	private RectTransform m_Pages;

	[Header("Pages Left")]
	[SerializeField]
	private RectTransform m_PageLeft;

	[SerializeField]
	private TextMeshProUGUI m_PagesLeftActionLabel;

	[SerializeField]
	private RectTransform m_PagesLeftKeyboard;

	[SerializeField]
	private RectTransform m_PagesLeftController;

	[SerializeField]
	private TextMeshProUGUI m_PagesLeftControllerLabel;

	[Header("Pages Right")]
	[SerializeField]
	private RectTransform m_PageRight;

	[SerializeField]
	private TextMeshProUGUI m_PagesRightActionLabel;

	[SerializeField]
	private RectTransform m_PagesRightKeyboard;

	[SerializeField]
	private RectTransform m_PagesRightController;

	[SerializeField]
	private TextMeshProUGUI m_PagesRightControllerLabel;

	private UIBookDataVO m_DataVO;

	private float m_InputDelay = 0.25f;

	private float m_InputTimer;

	private bool m_HasPages;

	private int m_PageIndex;

	public event EventHandler OnPageChange;

	protected override void OnInitialized(object _data)
	{
		m_DataVO = (UIBookDataVO)_data;
		m_DataVO.Book.Content.gameObject.SetActive(value: false);
		m_HasPages = m_DataVO.BookType == BookType.MugAndTheMaiden;
		m_Pages.gameObject.SetActive(m_HasPages);
		if (m_HasPages)
		{
			SetupPages();
		}
		else
		{
			m_BookLabel.text = m_DataVO.BookLabel;
		}
	}

	private void Update()
	{
		if (base.IsDisposed)
		{
			return;
		}
		if (m_InputTimer >= m_InputDelay)
		{
			if (PlayerInput.InteractOnReleased())
			{
				m_DataVO.Book.Content.gameObject.SetActive(value: true);
				m_DataVO.Book.Content.ForceDeactivate();
				GameManager.Instance.ShowCrosshair();
				GameManager.Instance.HideBook();
			}
			else
			{
				if (!m_HasPages)
				{
					return;
				}
				if (GameManager.Instance.HasController)
				{
					if (PlayerInput.ControllerLeftTrigger())
					{
						GetPreviousPage();
					}
					else if (PlayerInput.ControllerRightTrigger())
					{
						GetNextPage();
					}
				}
				else if (Input.GetKeyDown(KeyCode.A))
				{
					GetPreviousPage();
				}
				else if (Input.GetKeyDown(KeyCode.D))
				{
					GetNextPage();
				}
			}
		}
		else
		{
			m_InputTimer += Time.unscaledDeltaTime;
		}
	}

	private void SetupPages()
	{
		if (!GameManager.Instance.HasController)
		{
			m_PagesLeftController.gameObject.SetActive(value: false);
			m_PagesRightController.gameObject.SetActive(value: false);
		}
		else
		{
			m_PagesLeftKeyboard.gameObject.SetActive(value: false);
			m_PagesRightKeyboard.gameObject.SetActive(value: false);
			string text = "LT";
			string text2 = "RT";
			m_PagesLeftControllerLabel.text = text;
			m_PagesRightControllerLabel.text = text2;
		}
		m_PagesLeftActionLabel.text = TextUtility.GetKey("UI_ADDITIONAL_PAGE_PREVIOUS");
		m_PagesRightActionLabel.text = TextUtility.GetKey("UI_ADDITIONAL_PAGE_NEXT");
		GetPage();
	}

	private void GetNextPage()
	{
		m_PageIndex++;
		if (m_PageIndex >= m_DataVO.BookPages.Length)
		{
			m_PageIndex = m_DataVO.BookPages.Length - 1;
		}
		else
		{
			this.OnPageChange.Send(this);
		}
		GetPage();
	}

	private void GetPreviousPage()
	{
		m_PageIndex--;
		if (m_PageIndex <= 0)
		{
			m_PageIndex = 0;
		}
		else
		{
			this.OnPageChange.Send(this);
		}
		GetPage();
	}

	private void GetPage()
	{
		m_BookLabel.text = m_DataVO.BookPages[m_PageIndex];
		m_PageLeft.gameObject.SetActive(m_PageIndex != 0);
		m_PageRight.gameObject.SetActive(m_PageIndex < m_DataVO.BookPages.Length - 1);
	}

	protected override void OnDisposed()
	{
		this.OnPageChange = null;
		m_DataVO = null;
		base.OnDisposed();
	}
}
