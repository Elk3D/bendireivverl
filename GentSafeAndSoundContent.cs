using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GentSafeAndSoundContent : ActionEventContent<GentSafeAndSoundContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public GameObject Lighting;

		public GameObject Blocker;

		public float TweenDuration = 0.4f;

		public Ease TweenEase = Ease.OutSine;
	}

	[Header("Shelves")]
	[SerializeField]
	private BoxCollider m_TopShelf;

	[SerializeField]
	private BoxCollider m_BottomShelf;

	[Header("Loot Containers")]
	[SerializeField]
	private Transform[] m_LootContainers;

	private int m_LootCount;

	private List<ActionEventController> m_Loot = new List<ActionEventController>();

	private Transform m_ActiveContainer;

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position) > 15f)
		{
			if (base.IsInactive && GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Cards >= 1)
			{
				Enable();
			}
			else if (base.IsActivated)
			{
				ForceClose();
			}
		}
		if (!base.IsInactive && GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Cards < 1)
		{
			Disable();
		}
	}

	protected override void InternalEnable()
	{
		if (base.IsActivated)
		{
			base.Connectable.SendOnOnDeactivate();
		}
		base.IsActivated = false;
		if (m_Sequencer != null)
		{
			m_Sequencer.New();
		}
		for (int i = 0; i < m_Properties.Length; i++)
		{
			Properties properties = m_Properties[i];
			ResetContent(properties);
		}
	}

	protected override void InternalDisable()
	{
		for (int i = 0; i < m_Properties.Length; i++)
		{
			Properties properties = m_Properties[i];
			if (properties.Lighting != null)
			{
				properties.Lighting.SetActive(value: false);
			}
			if (properties.Blocker != null)
			{
				properties.Blocker.SetActive(value: false);
			}
		}
	}

	protected override void OnActivate()
	{
		RemoveLootListeners();
		ResetLoot();
		bool flag = true;
		Collider[] array = Physics.OverlapBox(m_TopShelf.bounds.center, m_TopShelf.bounds.extents);
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = array[i].transform;
			if (transform.GetComponentInParent<AudioLogContent>() != null || transform.GetComponentInParent<IllusionContent>() != null || transform.GetComponentInParent<GentSchematicContent>() != null)
			{
				flag = false;
			}
			if (!flag)
			{
				break;
			}
		}
		bool flag2 = true;
		Collider[] array2 = Physics.OverlapBox(m_BottomShelf.bounds.center, m_BottomShelf.bounds.extents);
		for (int j = 0; j < array2.Length; j++)
		{
			Transform transform2 = array2[j].transform;
			if (transform2.GetComponentInParent<AudioLogContent>() != null || transform2.GetComponentInParent<IllusionContent>() != null || transform2.GetComponentInParent<GentSchematicContent>() != null)
			{
				flag2 = false;
			}
			if (!flag2)
			{
				break;
			}
		}
		if (m_ActiveContainer == null)
		{
			m_ActiveContainer = m_LootContainers[UnityEngine.Random.Range(0, m_LootContainers.Length)];
		}
		else
		{
			Transform transform3 = m_ActiveContainer;
			while (transform3 == m_ActiveContainer)
			{
				transform3 = m_LootContainers[UnityEngine.Random.Range(0, m_LootContainers.Length)];
			}
			m_ActiveContainer = transform3;
		}
		ActionEventController[] componentsInChildren = m_ActiveContainer.GetComponentsInChildren<ActionEventController>(includeInactive: true);
		foreach (ActionEventController actionEventController in componentsInChildren)
		{
			actionEventController.gameObject.SetActive(value: false);
			if ((flag || !m_TopShelf.bounds.Contains(actionEventController.transform.position)) && (flag2 || !m_BottomShelf.bounds.Contains(actionEventController.transform.position)) && UnityEngine.Random.value < 0.45f)
			{
				GenerateLoot(actionEventController);
			}
		}
		m_ActiveContainer.gameObject.SetActive(value: true);
		base.IsTooExpensive = false;
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_LOOT_SPENT") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_GENT_CARD"), "Icon/Collectables/Small/UIIcon_Card", 1);
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveCard();
		Disable();
		if (m_Sequencer != null)
		{
			m_Sequencer.New();
		}
		for (int l = 0; l < m_ActiveProperties.Count; l++)
		{
			Properties properties = m_ActiveProperties[l];
			CheckAnimation(properties);
		}
	}

	private void GenerateLoot(ActionEventController lootPrefab)
	{
		ActionEventController actionEventController = GameManager.Instance.AssetManager.CreateAsset<ActionEventController>(lootPrefab);
		if (!m_Loot.Contains(actionEventController))
		{
			m_Loot.Add(actionEventController);
			actionEventController.transform.SetParent(m_ActiveContainer);
			actionEventController.transform.position = lootPrefab.transform.position;
			actionEventController.transform.eulerAngles = lootPrefab.transform.eulerAngles;
			actionEventController.Initialize();
			if (actionEventController.Content is FoodContent foodContent)
			{
				foodContent.InitializeContent();
			}
			else if (actionEventController.Content is SlugContent slugContent)
			{
				slugContent.InitializeContent();
			}
			else if (actionEventController.Content is GentPartsContent gentPartsContent)
			{
				gentPartsContent.InitializeContent();
			}
			else if (actionEventController.Content is GentBatteryContent gentBatteryContent)
			{
				gentBatteryContent.InitializeContent();
			}
			else if (actionEventController.Content is GentBatteryCasingContent gentBatteryCasingContent)
			{
				gentBatteryCasingContent.InitializeContent();
			}
			else
			{
				if (!(actionEventController.Content is GentToolkitContent gentToolkitContent))
				{
					actionEventController.Dispose();
					return;
				}
				gentToolkitContent.InitializeContent();
			}
			m_LootCount++;
			actionEventController.gameObject.SetActive(value: true);
			actionEventController.OnActivate -= HandleLootOnActivate;
			actionEventController.OnActivate += HandleLootOnActivate;
		}
		else
		{
			actionEventController.Dispose();
		}
	}

	protected override void OnActivateComplete()
	{
		for (int i = 0; i < m_ActiveProperties.Count; i++)
		{
			Properties properties = m_ActiveProperties[i];
			if (properties.Lighting != null)
			{
				properties.Lighting.SetActive(value: false);
			}
			if (properties.Blocker != null)
			{
				properties.Blocker.SetActive(value: false);
			}
			if (properties.ActionEvent != null)
			{
				properties.ActionEvent.gameObject.SetActive(value: false);
			}
		}
	}

	private void ResetContent(Properties properties)
	{
		DoAnimation(properties, properties.OriginPosition, properties.OriginRotation, properties.TweenDuration, properties.TweenEase);
		if (properties.Lighting != null)
		{
			properties.Lighting.SetActive(value: true);
		}
		if (properties.Blocker != null)
		{
			properties.Blocker.SetActive(value: true);
		}
		if (properties.ActionEvent != null)
		{
			properties.ActionEvent.gameObject.SetActive(value: true);
		}
	}

	private void CheckAnimation(Properties properties)
	{
		DoAnimation(properties, properties.ActiveLocation.localPosition, properties.ActiveLocation.localEulerAngles, properties.TweenDuration, properties.TweenEase);
	}

	private void ForceClose()
	{
		if (base.IsActivated)
		{
			base.Connectable.SendOnOnDeactivate();
		}
		base.IsActivated = false;
		if (m_Sequencer != null)
		{
			m_Sequencer.New();
			m_Sequencer.OnComplete(ForceCloseOnComplete);
		}
		for (int i = 0; i < m_Properties.Length; i++)
		{
			Properties properties = m_Properties[i];
			DoAnimation(properties, properties.OriginPosition, properties.OriginRotation, properties.TweenDuration, properties.TweenEase);
			if (properties.Blocker != null)
			{
				properties.Blocker.SetActive(value: true);
			}
			if (properties.ActionEvent != null)
			{
				properties.ActionEvent.gameObject.SetActive(value: true);
			}
		}
		for (int num = m_Loot.Count - 1; num >= 0; num--)
		{
			m_Loot[num].Dispose();
		}
		ClearLoot();
	}

	private void ForceCloseOnComplete()
	{
		if (base.IsInactive && GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Cards >= 1)
		{
			Enable();
		}
	}

	private void HandleLootOnActivate(object sender, EventArgs e)
	{
		(sender as ActionEventController).OnActivate -= HandleLootOnActivate;
		m_LootCount--;
		if (m_LootCount <= 0)
		{
			m_LootCount = 0;
			ForceClose();
		}
	}

	private void ResetLoot()
	{
		ClearLoot();
		m_Loot = new List<ActionEventController>();
	}

	private void ClearLoot()
	{
		if (m_Loot != null)
		{
			m_Loot.Clear();
			m_Loot = null;
		}
		for (int i = 0; i < m_LootContainers.Length; i++)
		{
			m_LootContainers[i].gameObject.SetActive(value: false);
		}
	}

	private void RemoveLootListeners()
	{
		if (m_Loot != null)
		{
			for (int i = 0; i < m_Loot.Count; i++)
			{
				m_Loot[i].OnActivate -= HandleLootOnActivate;
			}
		}
	}

	protected override void OnDisposed()
	{
		RemoveLootListeners();
		ClearLoot();
		base.OnDisposed();
	}
}
