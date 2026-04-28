using System;
using I2.Loc;
using TMPro;
using UnityEngine;

public class MenuItemButton : JMonoBehaviour
{
	[Header("TextMeshProUGUI")]
	[SerializeField]
	private Localize m_Label;

	[SerializeField]
	private TextMeshProUGUI m_LabelUGUI;

	[Header("Images")]
	[SerializeField]
	private GameObject m_Arrows;

	[Header("Button")]
	[SerializeField]
	private UIButton m_Button;

	private Action m_Callback;

	private bool m_IsClicked;

	public UIButton Button => m_Button;

	public string Label => m_Label.Term;

	public event EventHandler OnSelected;

	public event EventHandler OnClick;

	public void Init(UIMenuItemButtonDataVO vo)
	{
		if ((bool)m_Label)
		{
			m_Label.SetTerm(vo.Label);
		}
		if ((bool)m_LabelUGUI)
		{
			m_LabelUGUI.text = vo.Label;
		}
		m_Callback = vo.Callback;
		if ((bool)m_Arrows)
		{
			m_Arrows.SetActive(value: false);
		}
		AddListeners();
	}

	public void InitBasic()
	{
		m_Button.OnClick += HandleButtonBasicOnClick;
	}

	private void HandleButtonOnSelected(object sender, EventArgs e)
	{
		this.OnSelected.Send(this);
		if ((bool)m_Arrows)
		{
			m_Arrows.SetActive(value: true);
		}
	}

	private void HandleButtonOnDeselected(object sender, EventArgs e)
	{
		if ((bool)m_Arrows)
		{
			m_Arrows.SetActive(value: false);
		}
	}

	private void HandleButtonBasicOnClick(object sender, EventArgs e)
	{
		this.OnClick.Send(this);
	}

	private void HandleButtonOnClick(object sender, EventArgs e)
	{
		RemoveListeners();
		if ((bool)m_Arrows)
		{
			m_Arrows.SetActive(value: true);
		}
		m_IsClicked = true;
		this.OnClick.Send(this);
		if (m_Callback != null)
		{
			m_Callback();
		}
		m_Button.OnClick += HandleButtonOnClick;
	}

	public void Deselect()
	{
		if (m_IsClicked)
		{
			AddListeners();
			m_IsClicked = false;
		}
		if ((bool)m_Arrows)
		{
			m_Arrows.SetActive(value: false);
		}
	}

	private void AddListeners()
	{
		RemoveListeners();
		m_Button.OnClick += HandleButtonOnClick;
		AddSelectionListeners();
	}

	private void RemoveListeners()
	{
		m_Button.OnClick -= HandleButtonOnClick;
		RemoveSelectionListeners();
	}

	private void AddSelectionListeners()
	{
		if ((bool)m_Arrows)
		{
			m_Button.OnEnter += HandleButtonOnSelected;
			m_Button.OnExit += HandleButtonOnDeselected;
		}
	}

	private void RemoveSelectionListeners()
	{
		if ((bool)m_Arrows)
		{
			m_Button.OnEnter -= HandleButtonOnSelected;
			m_Button.OnExit -= HandleButtonOnDeselected;
		}
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		this.OnSelected = null;
		this.OnClick = null;
		m_Callback = null;
		base.OnDisposed();
	}
}
