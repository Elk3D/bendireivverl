using System;
using UnityEngine;

public class ObjectiveComboLock : Objective
{
	[SerializeField]
	private ComboLock m_ComboLock;

	protected override void InternalInitialize()
	{
		if (!m_ComboLock.IsComplete)
		{
			m_ComboLock.OnComplete += HandleComboLockOnComplete;
		}
	}

	private void HandleComboLockOnComplete(object sender, EventArgs e)
	{
		RemoveListeners();
		SendOnComplete();
	}

	protected override void RemoveListeners()
	{
		m_ComboLock.OnComplete -= HandleComboLockOnComplete;
	}
}
