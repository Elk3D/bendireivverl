using System;
using DG.Tweening;
using UnityEngine;

public class SimpleDOTweenTo : MonoBehaviour
{
	[SerializeField]
	private EventTrigger m_EventTrigger;

	[SerializeField]
	private GameObject m_TweenGameObject;

	[SerializeField]
	private Transform m_ToTween;

	[SerializeField]
	private float m_Speed = 3f;

	private void Start()
	{
		m_EventTrigger.OnEnter += M_EventTrigger_OnEnter;
		m_TweenGameObject.SetActive(value: false);
	}

	private void M_EventTrigger_OnEnter(object sender, EventArgs e)
	{
		m_TweenGameObject.SetActive(value: true);
		m_TweenGameObject.transform.DOMove(m_ToTween.position, m_Speed).SetEase(Ease.Linear);
		m_TweenGameObject.transform.DORotate(m_ToTween.eulerAngles, m_Speed).SetEase(Ease.Linear).OnComplete(onComplete);
	}

	private void onComplete()
	{
		m_TweenGameObject.SetActive(value: false);
	}
}
