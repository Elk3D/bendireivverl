using System;
using DG.Tweening;
using UnityEngine;

public class ObjectiveDummyCompanion : Objective
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Dummy Stuff")]
	[SerializeField]
	private Dummy m_DummyCompanion;

	[SerializeField]
	private Transform m_ToLocation;

	private bool m_IsChecking;

	protected override void InternalInitialize()
	{
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		if (!CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
		}
	}

	protected override void InternalUpdate()
	{
		if (m_IsChecking && Vector3.Distance(m_DummyCompanion.transform.position, m_ToLocation.position) < 1.5f)
		{
			m_IsChecking = false;
			Sequence sequence = DOTween.Sequence();
			sequence.Insert(0f, m_DummyCompanion.transform.DOMove(m_ToLocation.position, 0.25f).SetEase(Ease.Linear));
			sequence.Insert(0f, m_DummyCompanion.transform.DORotate(m_ToLocation.eulerAngles, 0.25f).SetEase(Ease.Linear));
			sequence.OnComplete(SequenceOnComplete);
		}
	}

	private void SequenceOnComplete()
	{
		m_DummyCompanion.gameObject.SetActive(value: false);
		SendOnComplete();
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
			GameManager.Instance.DummyManager.RemoveDummiesOf(m_DummyCompanion.transform);
			m_DummyCompanion.SetStopDistance(0f);
			m_DummyCompanion.SetTarget(m_ToLocation);
			m_IsChecking = true;
		}
		return flag;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		CheckStatus();
	}
}
