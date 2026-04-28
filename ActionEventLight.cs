using System;
using UnityEngine;

public class ActionEventLight : JMonoBehaviour
{
	[SerializeField]
	private LightBulbController m_LightBulbController;

	private ActionEventController m_ActionEventController;

	public override void Start()
	{
		if (!(m_LightBulbController == null))
		{
			m_ActionEventController = GetComponent<ActionEventController>();
			if (m_ActionEventController != null)
			{
				m_ActionEventController.OnActivated += HAndleActionEventControllerOnActivated;
				m_ActionEventController.OnDeactivated += HandleActionEventControllerOnDeactivated;
			}
		}
	}

	private void HandleActionEventControllerOnDeactivated(object sender, EventArgs e)
	{
		m_LightBulbController?.TurnOff();
	}

	private void HAndleActionEventControllerOnActivated(object sender, EventArgs e)
	{
		m_LightBulbController?.TurnOn();
	}
}
