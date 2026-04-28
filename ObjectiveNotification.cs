using InControl;
using UnityEngine;

public class ObjectiveNotification : Objective
{
	[Header("Primary Label")]
	[SerializeField]
	private string m_Label = "";

	[SerializeField]
	private bool m_IsKey;

	[SerializeField]
	private bool m_ToUpper;

	[Header("Input Options")]
	[SerializeField]
	private bool m_UseInput;

	[SerializeField]
	private string m_PressKey;

	[SerializeField]
	private string m_ActionKey;

	[SerializeField]
	private string m_PCKeyboardButton = "";

	[SerializeField]
	private InputControlType m_InputControlType;

	protected override void InternalInitialize()
	{
		if (m_UseInput)
		{
			if (GameManager.Instance.HasController)
			{
				GameManager.Instance.ShowNotificationTextInput(TextUtility.GetKey(m_PressKey).ToUpper(), TextUtility.GetKey(m_ActionKey).ToUpper(), m_InputControlType);
			}
			else
			{
				GameManager.Instance.ShowNotificationTextInput(TextUtility.GetKey(m_PressKey).ToUpper(), TextUtility.GetKey(m_ActionKey).ToUpper(), m_PCKeyboardButton);
			}
		}
		else if (m_IsKey)
		{
			if (m_ToUpper)
			{
				GameManager.Instance.ShowNotificationText(TextUtility.GetKey(m_Label).GetColorNotification().GetWhite()
					.ToUpper());
			}
			else
			{
				GameManager.Instance.ShowNotificationText(TextUtility.GetKey(m_Label).GetColorNotification().GetWhite());
			}
		}
		else if (m_ToUpper)
		{
			GameManager.Instance.ShowNotificationText(m_Label.ToUpper());
		}
		else
		{
			GameManager.Instance.ShowNotificationText(m_Label);
		}
		SendOnComplete();
	}
}
