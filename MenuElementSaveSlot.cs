using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuElementSaveSlot : MenuElementButtonLabel
{
	[Header("Save Slot Settings")]
	[SerializeField]
	protected TextMeshProUGUI m_NewSaveGameLabel;

	[SerializeField]
	private Image[] m_Borders;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		m_NewSaveGameLabel.text = TextUtility.GetKey("MENU_SETTINGS_SAVE_GAME_NEW");
		m_NewSaveGameLabel.color = base.InactiveColor;
		for (int i = 0; i < m_Borders.Length; i++)
		{
			m_Borders[i].color = base.InactiveColor;
		}
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
		m_Sequencer?.RestartSequence();
		m_Sequencer?.Insert(0f, m_Label.DOColor(base.ActiveColor, 0.1f));
		m_Sequencer?.Insert(0f, m_NewSaveGameLabel.DOColor(base.ActiveColor, 0.1f));
		for (int i = 0; i < m_Borders.Length; i++)
		{
			Image target = m_Borders[i];
			m_Sequencer?.Insert(0f, target.DOColor(base.ActiveColor, 0.1f));
		}
		GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[0]);
	}

	public override void ForceOnExit()
	{
		m_Sequencer?.RestartSequence();
		m_Sequencer?.Insert(0f, m_Label.DOColor(base.InactiveColor, 0.1f));
		m_Sequencer?.Insert(0f, m_NewSaveGameLabel.DOColor(base.InactiveColor, 0.1f));
		for (int i = 0; i < m_Borders.Length; i++)
		{
			Image target = m_Borders[i];
			m_Sequencer?.Insert(0f, target.DOColor(base.InactiveColor, 0.1f));
		}
	}
}
