using System;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveExit : Objective
{
	[SerializeField]
	private Interactable m_Interactalbe;

	private UIPrompt m_MenuExitPrompt;

	protected override void InternalInitialize()
	{
		m_Interactalbe.OnInteract += HandleInteractableOnInteract;
		SendOnComplete();
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		m_Interactalbe.OnInteract -= HandleInteractableOnInteract;
		GameManager.Instance.SoftPause();
		GameManager.Instance.ShowCursor();
		if (m_MenuExitPrompt != null)
		{
			m_MenuExitPrompt.Dispose();
			m_MenuExitPrompt = null;
		}
		m_MenuExitPrompt = GameManager.Instance.UIManager.Show<UIPrompt>("UI/GameMenuPrompt/UIPrompt", "GAME MENU PROMPT", new UIPromptDataVO(TextUtility.GetKey("MENU_SETTINGS_MAIN_MENU"), TextUtility.GetKey("MENU_SETTINGS_MAIN_MENU_HEADDER"), new UIElementButtonDataVO("UI/GameMenu/MenuElementButtonControllerLabel", TextUtility.GetKey("MENU_YES"), delegate
		{
			SceneManager.LoadScene("Reset");
		}, "", TextAlignmentOptions.Midline, InputControlType.Action1), new UIElementButtonDataVO("UI/GameMenu/MenuElementButtonControllerLabel", TextUtility.GetKey("MENU_NO"), PlayOutMenuExit, "", TextAlignmentOptions.Midline, InputControlType.Action2)));
	}

	private void PlayOutMenuExit()
	{
		GameManager.Instance.SoftUnpause();
		GameManager.Instance.HideCursor();
		m_Interactalbe.OnInteract += HandleInteractableOnInteract;
		m_Interactalbe.ResetAction();
		if (m_MenuExitPrompt != null)
		{
			m_MenuExitPrompt.PlayOut();
		}
	}
}
