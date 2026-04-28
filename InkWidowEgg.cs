using System;
using DG.Tweening;
using UnityEngine;

public class InkWidowEgg : JMonoBehaviour
{
	[SerializeField]
	private GameObject m_Egg;

	[SerializeField]
	private GameObject m_Ink;

	[SerializeField]
	private ParticleSystem m_BrokenEgg;

	[SerializeField]
	private Enemy[] m_InkWidowPrefabs;

	private Enemy m_InkWidow;

	private Sequence m_Sequence;

	public event EventHandler OnInitialize;

	public event EventHandler OnDeath;

	public override void Start()
	{
		m_Ink.SetActive(value: false);
	}

	public void Initialize()
	{
		m_Ink.SetActive(value: true);
		GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		GameManager.Instance.Player.OnDeath += HandlePlayerOnDeath;
		m_Sequence = DOTween.Sequence();
		m_Sequence.InsertCallback(1f, delegate
		{
			m_Egg.SetActive(value: false);
			m_BrokenEgg.Play();
			m_InkWidow = GameManager.Instance.AssetManager.CreateAsset<Enemy>(m_InkWidowPrefabs[UnityEngine.Random.Range(0, m_InkWidowPrefabs.Length)]);
			m_InkWidow.transform.SetParent(base.transform);
			m_InkWidow.transform.position = base.transform.position;
			m_InkWidow.transform.eulerAngles = new Vector3(0f, UnityEngine.Random.Range(0f, 360f), 0f);
			m_InkWidow.OnInitializeOnComplete -= HandleInkWidowOnInitializeOnComplete;
			m_InkWidow.OnInitializeOnComplete += HandleInkWidowOnInitializeOnComplete;
			m_InkWidow.OnCharacterDeath -= HandleInkWidowOnCharacterDeath;
			m_InkWidow.OnCharacterDeath += HandleInkWidowOnCharacterDeath;
			this.OnInitialize.Send(this);
		});
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		if (m_InkWidow != null)
		{
			m_InkWidow.OnCharacterDeath -= HandleInkWidowOnCharacterDeath;
			m_InkWidow.OnInitializeOnComplete -= HandleInkWidowOnInitializeOnComplete;
		}
		SendDeath();
	}

	private void HandleInkWidowOnCharacterDeath(object sender, EventArgs e)
	{
		m_InkWidow.OnCharacterDeath -= HandleInkWidowOnCharacterDeath;
		GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		if (m_InkWidow != null)
		{
			m_InkWidow.transform.SetParent(null);
		}
		SendDeath();
	}

	private void HandleInkWidowOnInitializeOnComplete(object sender, EventArgs e)
	{
		m_InkWidow.OnInitializeOnComplete -= HandleInkWidowOnInitializeOnComplete;
		m_InkWidow.SetTarget(GameManager.Instance.Player.transform);
		m_InkWidow.SetState(State.Character.Follow);
	}

	private void SendDeath()
	{
		this.OnDeath.Send(this);
		Dispose();
	}

	protected override void OnDisposed()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		}
		this.OnInitialize = null;
		this.OnDeath = null;
		if (m_InkWidow != null)
		{
			m_InkWidow.OnCharacterDeath -= HandleInkWidowOnCharacterDeath;
			m_InkWidow.OnInitializeOnComplete -= HandleInkWidowOnInitializeOnComplete;
		}
		if (m_Sequence != null)
		{
			m_Sequence.Kill();
			m_Sequence = null;
		}
		base.OnDisposed();
	}
}
