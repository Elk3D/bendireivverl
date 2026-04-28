using System;
using System.Collections;
using UnityEngine;

public class CollectableSpawner : JMonoBehaviour
{
	private const float TIMER_LIMIT = 300f;

	private const float SPAWN_PERCENT = 0.7f;

	[Header("Prefabs")]
	[SerializeField]
	private ActionEventController[] m_Prefabs;

	private float m_Timer;

	private ActionEventController m_Collectable;

	public bool IsSpawned { get; private set; }

	public override void Start()
	{
		StartCoroutine(InternalStart());
	}

	private IEnumerator InternalStart()
	{
		while (GameManager.Instance.Player == null || !GameManager.Instance.Player.gameObject.activeInHierarchy || GameManager.Instance.IsPaused)
		{
			yield return new WaitForEndOfFrame();
		}
		if (Vector3.Distance(GameManager.Instance.Player.transform.position, base.transform.position) > 40f)
		{
			Spawn();
		}
		yield return null;
	}

	private void Update()
	{
		if (IsSpawned || GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (m_Timer >= 300f)
		{
			if (Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position) > 40f)
			{
				Spawn();
			}
		}
		else
		{
			m_Timer += Time.deltaTime;
		}
	}

	private void Spawn(bool isForced = false)
	{
		if (IsSpawned)
		{
			return;
		}
		if (isForced || UnityEngine.Random.value <= 0.7f)
		{
			IsSpawned = true;
			m_Collectable = GameManager.Instance.AssetManager.CreateAsset<ActionEventController>(m_Prefabs[UnityEngine.Random.Range(0, m_Prefabs.Length)]);
			m_Collectable.SetParent(base.transform);
			m_Collectable.transform.localPosition = Vector3.zero;
			m_Collectable.transform.localEulerAngles = Vector3.zero;
			m_Collectable.OnActivate -= HandleCollectableOnActivate;
			m_Collectable.OnActivate += HandleCollectableOnActivate;
			if (m_Collectable.Content is SlugContent slugContent)
			{
				slugContent.InitializeContent();
			}
			else if (m_Collectable.Content is GentCardContent gentCardContent)
			{
				gentCardContent.InitializeContent();
			}
			else if (m_Collectable.Content is FoodContent foodContent)
			{
				foodContent.InitializeContent();
			}
			else if (m_Collectable.Content is GentPartsContent gentPartsContent)
			{
				gentPartsContent.InitializeContent();
			}
			else if (m_Collectable.Content is GentToolkitContent gentToolkitContent)
			{
				gentToolkitContent.InitializeContent();
			}
			else if (m_Collectable.Content is GentBatteryContent gentBatteryContent)
			{
				gentBatteryContent.InitializeContent();
			}
		}
		m_Timer = 0f;
	}

	public void ForceSpawn()
	{
		if (m_Collectable != null)
		{
			m_Collectable.OnActivate -= HandleCollectableOnActivate;
			m_Collectable.Dispose();
			m_Collectable = null;
			IsSpawned = false;
			m_Timer = 0f;
		}
		Spawn(isForced: true);
	}

	private void HandleCollectableOnActivate(object sender, EventArgs e)
	{
		m_Collectable.OnActivate -= HandleCollectableOnActivate;
		m_Collectable = null;
		IsSpawned = false;
		m_Timer = 0f;
	}

	protected override void OnDisposed()
	{
		StopCoroutine(InternalStart());
		if (m_Collectable != null)
		{
			m_Collectable.OnActivate -= HandleCollectableOnActivate;
			m_Collectable = null;
		}
		base.OnDisposed();
	}
}
