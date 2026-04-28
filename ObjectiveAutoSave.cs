using System.Collections;
using UnityEngine;

public class ObjectiveAutoSave : Objective
{
	[SerializeField]
	private bool m_DisableNotification;

	[SerializeField]
	private bool m_WaitToComplete;

	private bool CantSave
	{
		get
		{
			if (!GameManager.Instance.IsPaused)
			{
				return !GameManager.Instance.Player.gameObject.activeInHierarchy;
			}
			return true;
		}
	}

	protected override void InternalInitialize()
	{
		StartCoroutine(AutoSave());
		if (!m_WaitToComplete)
		{
			SendOnComplete();
		}
	}

	private IEnumerator AutoSave()
	{
		while (CantSave)
		{
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(0.2f);
		while (CantSave)
		{
			yield return new WaitForEndOfFrame();
		}
		if (!base.IsDisposed)
		{
			if (m_WaitToComplete)
			{
				GameManager.Instance.GameData.AutoSave(m_DisableNotification, base.SendOnComplete);
			}
			else
			{
				GameManager.Instance.GameData.AutoSave(m_DisableNotification);
			}
		}
	}
}
