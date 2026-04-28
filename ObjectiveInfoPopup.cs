using UnityEngine;

public class ObjectiveInfoPopup : Objective
{
	[Header("Info Popup Type")]
	[SerializeField]
	private InfoPopupType m_InfoPopupType;

	protected override void InternalInitialize()
	{
		string input = "";
		if (m_InfoPopupType == InfoPopupType.GentPipe)
		{
			input = "LMB";
		}
		else if (m_InfoPopupType == InfoPopupType.ShockPipe)
		{
			input = "TAB";
		}
		else if (m_InfoPopupType == InfoPopupType.StunPipe)
		{
			input = "TAB";
		}
		GameManager.Instance.ShowInfoPopup(UIInfoPopupDataVO.Create(m_InfoPopupType, input));
		SendOnComplete();
	}
}
