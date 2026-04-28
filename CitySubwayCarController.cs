using System;
using DG.Tweening;
using UnityEngine;

public class CitySubwayCarController : JMonoBehaviour
{
	[SerializeField]
	private Transform m_SubwayCar;

	[SerializeField]
	private Transform m_StartLocation;

	[SerializeField]
	private Transform m_EndLocation;

	[SerializeField]
	private Transform m_AudioLocation;

	private Sequence m_Sequence;

	private float m_Timer;

	private float m_TimerRate = 30f;

	public event EventHandler OnTrigger;

	private void Update()
	{
		if (!base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > m_TimerRate)
			{
				m_Timer = 0f;
				Trigger();
			}
		}
	}

	private void Trigger()
	{
		this.OnTrigger.Send(this);
		m_SubwayCar.localPosition = m_StartLocation.localPosition;
		m_SubwayCar.gameObject.SetActive(value: true);
		ResetSequence();
		m_Sequence.Insert(0.5f, m_SubwayCar.DOLocalMove(m_EndLocation.localPosition, 1.5f).SetEase(Ease.Linear));
		m_Sequence.InsertCallback(0.6f, delegate
		{
			Vector3 position = m_AudioLocation.position;
			position.y = GameManager.Instance.Player.transform.position.y;
			if (Vector3.Distance(GameManager.Instance.Player.transform.position, position) < 25f)
			{
				CameraEffects.ShakeRotation(2f, 1f);
			}
		});
		m_Sequence.OnComplete(Deactivate);
	}

	private void Deactivate()
	{
		m_SubwayCar.gameObject.SetActive(value: false);
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			m_Sequence.Kill();
			m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		KillSequence();
		base.OnDisposed();
	}
}
