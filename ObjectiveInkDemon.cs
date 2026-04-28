using UnityEngine;

public class ObjectiveInkDemon : Objective
{
	[SerializeField]
	private bool m_IsReset;

	[SerializeField]
	private bool m_Disable = true;

	protected override void InternalInitialize()
	{
		if (m_IsReset)
		{
			GameManager.Instance.InkDemonManager.ResetTimer();
			if (m_Disable)
			{
				GameManager.Instance.InkDemonManager.SetActive(active: false);
			}
			else
			{
				GameManager.Instance.InkDemonManager.SetActive(active: true);
			}
		}
		else
		{
			GameManager.Instance.Player.InkDemonTest();
		}
		SendOnComplete();
	}
}
