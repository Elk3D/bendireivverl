using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootableEnemy : Interactable
{
	private SectionID m_SectionID;

	private EnemyType m_EnemyType;

	private LostOneType m_LostOneType;

	private SkinnedMeshRenderer[] m_SkinnedMeshRenderers;

	private MeshRenderer[] m_MeshRenderers;

	private GameObject m_Model;

	private int m_LootableEnemyID;

	private float m_FoodChance = 0.2f;

	private float m_KitChance = 0.4f;

	private float m_BatteryChance = 0.6f;

	private float m_PartsChance = 0.8f;

	private float m_SelfDestructTimer;

	private float m_SelfDestructTimerLimit = 60f;

	private bool m_IsLooted;

	private bool m_IsColor;

	public EnemyType EnemyType => m_EnemyType;

	public LostOneType LostOneType => m_LostOneType;

	public void Initialize(GameObject model, SectionID sectionID, EnemyType enemyType, LostOneType lostOneType = LostOneType.NONE, bool isColor = false, Transform parent = null)
	{
		m_Model = model;
		m_SectionID = sectionID;
		m_EnemyType = enemyType;
		m_LostOneType = lostOneType;
		m_IsColor = isColor;
		m_Model.transform.SetParent(base.transform);
		if (parent == null)
		{
			Section section = GameManager.Instance.SectionManager.GetSection(m_SectionID);
			if (section != null)
			{
				SectionController[] sectionControllers = section.SectionControllers;
				for (int i = 0; i < sectionControllers.Length; i++)
				{
					if (sectionControllers[i] is SectionEnemyController sectionEnemyController)
					{
						sectionEnemyController.AddLootableEnemy(this);
						base.transform.SetParent(sectionEnemyController.transform);
						break;
					}
				}
			}
		}
		else
		{
			base.transform.SetParent(parent);
		}
		StartCoroutine(StartLootable());
	}

	public void AddData()
	{
		SectionDataObject sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(m_SectionID);
		m_LootableEnemyID = base.gameObject.GetInstanceID();
		while (sectionDataObject.LootableEnemyData.ContainsKey(m_LootableEnemyID))
		{
			m_LootableEnemyID += Random.Range(-100, 100);
		}
		LootableEnemyDataObject lootableEnemyDataObject = DataObject<int, LootableEnemyDataObject>.Create(m_LootableEnemyID);
		lootableEnemyDataObject.SetEnemyType(m_EnemyType);
		lootableEnemyDataObject.SetLostOneType(m_LostOneType);
		lootableEnemyDataObject.SetGameObjectData(m_Model);
		sectionDataObject.LootableEnemyData.Add(m_LootableEnemyID, lootableEnemyDataObject);
	}

	public void UpdateData()
	{
		if (!m_IsLooted)
		{
			((LootableEnemyDataObject)((SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(m_SectionID)).LootableEnemyData.GetValue(m_LootableEnemyID))?.SetGameObjectData(m_Model);
		}
	}

	public void SetData(int id)
	{
		m_LootableEnemyID = id;
		LootableEnemyDataObject lootableEnemyDataObject = (LootableEnemyDataObject)((SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(m_SectionID)).LootableEnemyData.GetValue(m_LootableEnemyID);
		if (lootableEnemyDataObject == null)
		{
			return;
		}
		Transform[] componentsInChildren = m_Model.GetComponentsInChildren<Transform>(includeInactive: true);
		List<Transform> list = new List<Transform>();
		int num = 0;
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (num > 0)
			{
				if (transform.GetComponent<SeasonalHat>() != null || transform.GetComponent<SeasonalHatContainer>() != null || transform.GetComponent<SeasonalHatLoader>() != null)
				{
					continue;
				}
				list.Add(transform);
			}
			num++;
		}
		for (int j = 0; j < list.Count; j++)
		{
			Transform obj = list[j];
			GameObjectDataExtensions.SetData(gameObjectData: lootableEnemyDataObject.GameObjectDatas[j], gameObject: obj.gameObject);
		}
	}

	private IEnumerator StartLootable()
	{
		m_SkinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
		m_MeshRenderers = GetComponentsInChildren<MeshRenderer>();
		SetActive(active: false);
		yield return new WaitForSeconds(1f);
		yield return new WaitForEndOfFrame();
		SetActive(active: true);
	}

	private void Update()
	{
		if (!base.IsDisposed && !GameManager.Instance.IsPaused && !m_IsLooted && m_LostOneType != LostOneType.Male_Amok && m_EnemyType != EnemyType.KingWidow && m_EnemyType != EnemyType.ShipAhoy)
		{
			if (m_SelfDestructTimer >= m_SelfDestructTimerLimit)
			{
				SelfDestruct();
			}
			else
			{
				m_SelfDestructTimer += Time.deltaTime;
			}
		}
	}

	public void SelfDestruct()
	{
		RaycastHit hit = default(RaycastHit);
		if (m_Model != null)
		{
			Collider componentInChildren = m_Model.GetComponentInChildren<Collider>();
			if (componentInChildren != null)
			{
				hit.point = componentInChildren.transform.position;
			}
			else
			{
				hit.point = m_Model.transform.position;
			}
		}
		Loot(hit, isSelfDestruct: true);
	}

	protected override void OnInternalEnter(Vector3 origin, RaycastHit hit, object sender = null)
	{
		if (m_SkinnedMeshRenderers != null)
		{
			for (int i = 0; i < m_SkinnedMeshRenderers.Length; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = m_SkinnedMeshRenderers[i];
				if (!(skinnedMeshRenderer != null) || skinnedMeshRenderer.materials == null || skinnedMeshRenderer.materials.Length == 0)
				{
					continue;
				}
				Material[] materials = skinnedMeshRenderer.materials;
				foreach (Material material in materials)
				{
					if (material.HasFloat("_Gradient"))
					{
						material.SetFloat("_Gradient", 1f);
					}
					if (material.HasFloat("_PostHitGlow"))
					{
						material.SetFloat("_PostHitGlow", 2f);
					}
				}
			}
		}
		if (m_MeshRenderers != null)
		{
			for (int k = 0; k < m_MeshRenderers.Length; k++)
			{
				MeshRenderer meshRenderer = m_MeshRenderers[k];
				if (!(meshRenderer != null) || meshRenderer.materials == null || meshRenderer.materials.Length == 0)
				{
					continue;
				}
				Material[] materials = meshRenderer.materials;
				foreach (Material material2 in materials)
				{
					if (material2.HasFloat("_Gradient"))
					{
						material2.SetFloat("_Gradient", 1f);
					}
					if (material2.HasFloat("_PostHitGlow"))
					{
						material2.SetFloat("_PostHitGlow", 2f);
					}
				}
			}
		}
		GameManager.Instance.ShowInteraction(TextUtility.GetKey("NOTIFICATION_LOOT"));
	}

	protected override void OnInternalExit(Vector3 origin, RaycastHit hit, object sender = null)
	{
		if (m_SkinnedMeshRenderers != null)
		{
			for (int i = 0; i < m_SkinnedMeshRenderers.Length; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = m_SkinnedMeshRenderers[i];
				if (!(skinnedMeshRenderer != null) || skinnedMeshRenderer.materials == null || skinnedMeshRenderer.materials.Length == 0)
				{
					continue;
				}
				Material[] materials = skinnedMeshRenderer.materials;
				foreach (Material material in materials)
				{
					if (material.HasFloat("_Gradient"))
					{
						material.SetFloat("_Gradient", 0.1f);
					}
					if (material.HasFloat("_PostHitGlow"))
					{
						material.SetFloat("_PostHitGlow", 0.3f);
					}
				}
			}
		}
		if (m_MeshRenderers != null)
		{
			for (int k = 0; k < m_MeshRenderers.Length; k++)
			{
				MeshRenderer meshRenderer = m_MeshRenderers[k];
				if (!(meshRenderer != null) || meshRenderer.materials == null || meshRenderer.materials.Length == 0)
				{
					continue;
				}
				Material[] materials = meshRenderer.materials;
				foreach (Material material2 in materials)
				{
					if (material2.HasFloat("_Gradient"))
					{
						material2.SetFloat("_Gradient", 0.1f);
					}
					if (material2.HasFloat("_PostHitGlow"))
					{
						material2.SetFloat("_PostHitGlow", 0.3f);
					}
				}
			}
		}
		GameManager.Instance.HideInteraction();
	}

	protected override void OnInternalInteract(Vector3 origin, RaycastHit hit, object sender = null)
	{
		Loot(hit, isSelfDestruct: false);
	}

	private void Loot(RaycastHit hit, bool isSelfDestruct)
	{
		if (!isSelfDestruct)
		{
			GameManager.Instance.HideInteraction();
			GameManager.Instance.Player.Interaction.ResetInteraction();
		}
		string prefab = "Effects/Effects_Looted_Enemy";
		string prefab2 = "Effects/Effects_Hit_Ink";
		if (m_IsColor)
		{
			prefab = "Effects/Effects_Looted_Enemy_Color";
			prefab2 = "Effects/Effects_Hit_Ink_Color";
		}
		GameManager.Instance.PoolingManager.GetFromPool(prefab, 10f).GetComponent<LootedEnemy>().Initialize(hit.point);
		GameManager.Instance.PoolingManager.GetFromPool(prefab2, 6f).transform.position = hit.point;
		if (!isSelfDestruct)
		{
			GetLoot();
		}
		SectionDataObject sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(m_SectionID);
		LootableEnemyDataObject lootableEnemyDataObject = (LootableEnemyDataObject)sectionDataObject.LootableEnemyData.GetValue(m_LootableEnemyID);
		if (lootableEnemyDataObject != null)
		{
			sectionDataObject.LootableEnemyData.Remove(lootableEnemyDataObject);
		}
		m_IsLooted = true;
		if (isSelfDestruct)
		{
			Dispose();
		}
	}

	private void GetLoot()
	{
		if (m_LostOneType == LostOneType.Male_Amok)
		{
			GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_LOOT_EAT") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_BACON_SOUP"), "Icon/Collectables/Small/UIIcon_Food", 1);
			GameManager.Instance.Player.Heal((int)UpgradeCheck.GetHealth());
			int amount = 20;
			GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_SLUGS"), "Icon/Collectables/Small/UIIcon_Slug", amount, 0.15f);
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddSlugs(amount);
			GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_GENT_TOOLKIT"), "Icon/Collectables/Small/UIIcon_Toolkit", 1, 0.3f);
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddToolkit();
			GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_GENT_PARTS"), "Icon/Collectables/Small/UIIcon_Parts", 2, 0.45f);
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddPart();
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddPart();
			GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_GENT_BATTERY"), "Icon/Collectables/Small/UIIcon_Battery", 1, 0.6f);
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddBattery();
		}
		else if (m_EnemyType == EnemyType.LostOneSeasonalWinter)
		{
			if (!SeasonalCheck.HasHat)
			{
				GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_SEASONAL_WINTER_HAT"));
				GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Hats/Prefab_Seasonal_Winter_Hat_01_Audrey"));
				Transform transform = GameManager.Instance.Player.transform.FindDeepChild("WinterHat");
				if (transform != null)
				{
					gameObject.transform.SetParent(transform);
					gameObject.transform.localScale = Vector3.one;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localEulerAngles = Vector3.zero;
				}
				SeasonalCheck.HasHat = true;
			}
			else
			{
				GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_SEASONAL_PRESENT"));
				PresentCheck.Collect();
			}
		}
		else if (m_EnemyType == EnemyType.LostOneSeasonalHalloween)
		{
			if (!SeasonalCheck.HasHat)
			{
				GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_SEASONAL_HALLOWEEN_HAT"));
				GameObject gameObject2 = (GameObject)Object.Instantiate(Resources.Load("Hats/Prefab_Seasonal_Halloween_Hat_01_Audrey"));
				Transform transform2 = GameManager.Instance.Player.transform.FindDeepChild("HalloweenHat");
				if (transform2 != null)
				{
					gameObject2.transform.SetParent(transform2);
					gameObject2.transform.localScale = Vector3.one;
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.localEulerAngles = Vector3.zero;
				}
				SeasonalCheck.HasHat = true;
			}
		}
		else if (Random.value <= LootCheck.GetChance())
		{
			float value = Random.value;
			if (value <= m_FoodChance)
			{
				string[] array = new string[3] { "NOTIFICATION_LOOT_BENDY_BAR", "NOTIFICATION_LOOT_SANDWHICH", "NOTIFICATION_LOOT_DONUT" };
				GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_LOOT_EAT") + " " + TextUtility.GetKey(array[Random.Range(0, array.Length)]), "Icon/Collectables/Small/UIIcon_Food", 1);
				GameManager.Instance.Player.Heal(1);
				CheckSlugs(value, 0.15f);
			}
			else if (value <= m_KitChance)
			{
				GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_GENT_TOOLKIT"), "Icon/Collectables/Small/UIIcon_Toolkit", 1);
				GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddToolkit();
				CheckSlugs(value, 0.15f);
			}
			else if (value <= m_BatteryChance)
			{
				GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_GENT_BATTERY"), "Icon/Collectables/Small/UIIcon_Battery", 1);
				GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddBattery();
				CheckSlugs(value, 0.15f);
			}
			else if (value <= m_PartsChance)
			{
				GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_GENT_PARTS"), "Icon/Collectables/Small/UIIcon_Parts", 1);
				GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddPart();
				CheckSlugs(value, 0.15f);
			}
			else
			{
				GetSlugs();
			}
		}
	}

	private void CheckSlugs(float percent, float delay)
	{
		if (percent < 0.1f)
		{
			GetSlugs(delay);
		}
	}

	private void GetSlugs(float delay = 0f)
	{
		int num = Random.Range(1, 6);
		string key = TextUtility.GetKey("NOTIFICATION_LOOT_SLUG");
		if (num > 1)
		{
			key = TextUtility.GetKey("NOTIFICATION_LOOT_SLUGS");
		}
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + key, "Icon/Collectables/Small/UIIcon_Slug", num, delay);
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddSlugs(num);
	}

	protected override void OnDisposed()
	{
		m_SkinnedMeshRenderers = null;
		m_MeshRenderers = null;
		m_Model = null;
		base.OnDisposed();
	}
}
