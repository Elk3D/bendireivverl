using System;
using DG.Tweening;
using UnityEngine;

public class SockPuppet : JMonoBehaviour
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Cutscene Activator")]
	[SerializeField]
	private CutsceneActivator m_CutsceneActivator;

	[Header("Balls")]
	[SerializeField]
	private Interactable m_Interactable;

	[SerializeField]
	private Collectable[] m_Collectables;

	[SerializeField]
	private Transform m_PlopLocation;

	[Header("Prefabs")]
	[SerializeField]
	private Transform m_BallPrefab;

	private int m_BallCount;

	private int m_PlopCount;

	public event EventHandler OnPlop;

	public override void Start()
	{
		m_Interactable.SetActive(active: false);
		for (int i = 0; i < m_Collectables.Length; i++)
		{
			m_Collectables[i].Content.Disable();
		}
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		if (!CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
		}
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		m_Interactable.ResetAction();
		if (m_BallCount <= 0 || m_PlopCount >= m_Collectables.Length || m_PlopCount >= m_BallCount)
		{
			return;
		}
		Debug.Log("plop less than ball");
		m_PlopCount++;
		if (m_PlopCount >= m_Collectables.Length)
		{
			m_Interactable.OnInteract -= HandleInteractableOnInteract;
			m_Interactable.Dispose();
		}
		Transform ball = GameManager.Instance.AssetManager.CreateAsset<Transform>(m_BallPrefab);
		ball.position = GameManager.Instance.GameCamera.transform.position + Vector3.down;
		ball.eulerAngles = new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
		Vector3 endValue = ball.eulerAngles + new Vector3(UnityEngine.Random.Range(180, 720), UnityEngine.Random.Range(180, 720), UnityEngine.Random.Range(180, 720));
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, ball.DOMoveX(m_PlopLocation.position.x, 0.35f).SetEase(Ease.InSine));
		sequence.Insert(0f, ball.DOMoveZ(m_PlopLocation.position.z, 0.35f).SetEase(Ease.InSine));
		sequence.Insert(0f, ball.DOMoveY(m_PlopLocation.position.y, 0.55f).SetEase(Ease.InSine));
		sequence.Insert(0f, ball.DORotate(endValue, 0.55f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
		sequence.OnComplete(delegate
		{
			this.OnPlop.Send(this);
			UnityEngine.Object.Destroy(ball.gameObject);
			if (m_PlopCount >= m_Collectables.Length)
			{
				m_CutsceneActivator.ForcePlay();
			}
		});
	}

	private void HandleCollectableOnActivate(object sender, EventArgs e)
	{
		(sender as Collectable).OnActivate -= HandleCollectableOnActivate;
		m_BallCount++;
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
			m_Interactable.OnInteract -= HandleInteractableOnInteract;
			m_Interactable.OnInteract += HandleInteractableOnInteract;
			m_Interactable.SetActive(active: true);
			m_Interactable.ResetAction();
			for (int i = 0; i < m_Collectables.Length; i++)
			{
				Collectable obj = m_Collectables[i];
				obj.OnActivate -= HandleCollectableOnActivate;
				obj.OnActivate += HandleCollectableOnActivate;
				obj.Content.Enable();
			}
		}
		return flag;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		CheckStatus();
	}

	protected override void OnDisposed()
	{
		this.OnPlop = null;
		base.OnDisposed();
	}
}
