using System;
using UnityEngine;

public class CutsceneComplete : JMonoBehaviour
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Requirement Complete")]
	[SerializeField]
	private ActiveSetter m_ActiveSetter;

	public void Initialize()
	{
		if (!CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
		}
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		Debug.Log("CutsceneComplete :: HandleOnObjectiveComplete", this);
		CheckStatus();
	}

	private bool CheckStatus()
	{
		bool flag = true;
		if (m_Requirements != null)
		{
			flag = m_Requirements.IsComplete();
		}
		if (flag)
		{
			GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
			if (m_ActiveSetter != null)
			{
				m_ActiveSetter.SetActive(active: false);
			}
		}
		return flag;
	}

	private void RemoveListeners()
	{
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		base.OnDisposed();
	}
}
