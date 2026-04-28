using UnityEngine;

public class ObjectiveNotificationBox : Objective
{
	[SerializeField]
	private string m_Label = "Collected a Key";

	[SerializeField]
	private bool m_IsKey = true;

	protected override void InternalInitialize()
	{
		if (m_IsKey)
		{
			GameManager.Instance.ShowNotificationBox(TextUtility.GetKey(m_Label), "Icon/Collectables/Small/UIIcon_Key", 1);
		}
		else
		{
			GameManager.Instance.ShowNotificationBox(TextUtility.GetKey(m_Label));
		}
		SendOnComplete();
	}
}
