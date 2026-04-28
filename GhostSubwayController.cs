using System;
using DG.Tweening;
using UnityEngine;

public class GhostSubwayController : JMonoBehaviour
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Content")]
	[SerializeField]
	private GameObject m_Content;

	[Header("Subway Car")]
	[SerializeField]
	private Transform m_SubwayCar;

	[Header("Locations")]
	[SerializeField]
	private Transform m_StartLocation;

	[SerializeField]
	private Transform m_EndLocation;

	[Space]
	[SerializeField]
	private Transform m_AudioStartLocation;

	[SerializeField]
	private Transform m_AudioEndLocation;

	[Header("Audio")]
	[SerializeField]
	private AudioSource m_AudioSource;

	[SerializeField]
	private AudioSource m_AudioSourceLoop;

	private Sequence m_Sequence;

	private bool m_IsInitialized;

	private bool m_IsActivated;

	public override void Start()
	{
		m_Content.SetActive(value: false);
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		if (!CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
		}
	}

	private void Update()
	{
		if (m_IsInitialized && !m_IsActivated && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			DateTime now = DateTime.Now;
			if ((now.Hour == 4 || now.Hour == 16) && now.Minute == 14)
			{
				Execute();
			}
		}
	}

	private void Execute()
	{
		m_IsActivated = true;
		m_AudioSourceLoop.Stop();
		m_AudioSource.Stop();
		m_SubwayCar.position = m_StartLocation.position;
		m_Sequence.Kill();
		m_Sequence = DOTween.Sequence();
		m_Content.SetActive(value: true);
		m_AudioSourceLoop.Play();
		m_AudioSource.transform.position = m_AudioStartLocation.position;
		m_Sequence.InsertCallback(4f, m_AudioSource.Play);
		m_Sequence.Insert(5f, m_AudioSource.transform.DOMove(m_AudioEndLocation.position, 4f).SetEase(Ease.OutSine));
		m_Sequence.Insert(5f, m_SubwayCar.DOMove(m_EndLocation.position, 4f).SetEase(Ease.OutSine));
		m_Sequence.InsertCallback(11f, delegate
		{
			m_AudioSourceLoop.Stop();
			m_Content.SetActive(value: false);
		});
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("GhostSubwayController :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
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
			m_IsInitialized = true;
		}
		return flag;
	}

	protected override void OnDisposed()
	{
		if (m_Sequence != null)
		{
			m_Sequence.Kill();
			m_Sequence = null;
		}
		base.OnDisposed();
	}
}
