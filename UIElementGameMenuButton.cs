using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIElementGameMenuButton : UIElement
{
	[Header("Game Menu Type")]
	[SerializeField]
	private GameMenuType m_GameMenuType;

	[Header("Button")]
	[SerializeField]
	private UIButton m_Button;

	[Header("Visuals")]
	[SerializeField]
	private Image m_Icon;

	[Header("Color Settings")]
	[SerializeField]
	protected Color m_ActiveColor;

	[SerializeField]
	protected Color m_InactiveColor;

	[SerializeField]
	protected Color m_ActiveColorReal;

	[SerializeField]
	protected Color m_InactiveColorReal;

	private Sequencer m_Sequencer;

	public GameMenuType GameMenuType => m_GameMenuType;

	public UIButton Button => m_Button;

	public Image Icon => m_Icon;

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

	public event EventHandler OnEnter;

	public event EventHandler OnClick;

	public event EventHandler OnSelected;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		m_Sequencer = new Sequencer(isIndependent: true);
		m_Icon.color = InactiveColor;
		m_Button.OnEnter += HandleButtonOnEnter;
		m_Button.OnExit += HandleButtonOnExit;
		m_Button.OnClick += HandleButtonOnClick;
	}

	public void ForceOnEnter()
	{
		m_Sequencer?.CreateSequence(m_Icon.DOColor(ActiveColor, 0.1f), m_Icon.rectTransform.DOScale(1.05f, 0.1f));
		if (!m_Button.selected)
		{
			this.OnEnter.Send(this);
		}
		else
		{
			this.OnSelected.Send(this);
		}
	}

	public void ForceOnExit()
	{
		m_Sequencer?.CreateSequence(m_Icon.DOColor(InactiveColor, 0.1f), m_Icon.rectTransform.DOScale(1f, 0.1f));
	}

	protected virtual void HandleButtonOnEnter(object sender, EventArgs e)
	{
		ForceOnEnter();
	}

	protected virtual void HandleButtonOnExit(object sender, EventArgs e)
	{
		ForceOnExit();
	}

	private void HandleButtonOnClick(object sender, EventArgs e)
	{
		this.OnClick.Send(this);
	}

	private void RemoveListeners()
	{
		m_Button.OnEnter -= HandleButtonOnEnter;
		m_Button.OnExit -= HandleButtonOnExit;
		m_Button.OnClick -= HandleButtonOnClick;
	}

	protected override void OnDisposed()
	{
		this.OnEnter = null;
		this.OnClick = null;
		this.OnSelected = null;
		m_Sequencer?.Dispose();
		m_Sequencer = null;
		RemoveListeners();
		base.OnDisposed();
	}
}
