using System;
using UnityEngine;

public class ActiveSetterConnector : JMonoBehaviour
{
	[SerializeField]
	private ActionEvent m_ActionEvent;

	[SerializeField]
	private ActiveSetter m_ActiveSetter;

	public override void Start()
	{
		m_ActionEvent.OnInteract += HandleActionEventOnInteract;
	}

	private void HandleActionEventOnInteract(object sender, EventArgs e)
	{
		m_ActiveSetter.SetActive(!m_ActiveSetter.IsActive);
	}
}
