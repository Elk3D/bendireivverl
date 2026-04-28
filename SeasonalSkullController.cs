using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using UnityEngine;
using UnityEngine.AI;

public class SeasonalSkullController : JMonoBehaviour
{
	[SerializeField]
	private SeasonalCauldron m_Cauldron;

	[SerializeField]
	private Transform m_LootLocation;

	[SerializeField]
	private Transform m_NodeContainer;

	[SerializeField]
	private Transform m_SpawnContainer;

	[SerializeField]
	private S13AudioHandler m_AudioHandler_Music;

	private bool m_IsActive;

	private int m_EnemyLimit;

	private int m_EnemyCount;

	private List<Enemy> m_Enemies = new List<Enemy>();

	public event EventHandler OnLoot;

	public override void Start()
	{
		m_Cauldron.Content.Disable();
		m_Cauldron.OnActivate += HandleCauldronOnActivated;
		GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		GameManager.Instance.Player.OnDeath += HandlePlayerOnDeath;
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		if (m_AudioHandler_Music.GetObject().isPlaying)
		{
			m_AudioHandler_Music.Stop();
		}
		m_EnemyCount = 0;
		if (m_Enemies != null)
		{
			for (int i = 0; i < m_Enemies.Count; i++)
			{
				m_Enemies[i].Dispose();
			}
			m_Enemies.Clear();
		}
	}

	private void Update()
	{
		if (base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (!m_IsActive)
		{
			if (SeasonalSkullCheck.SkullCount >= 6)
			{
				m_IsActive = true;
				m_Cauldron.Content.Enable();
				m_Cauldron.EnableParticles();
			}
		}
		else if (SeasonalSkullCheck.SkullCount < 6)
		{
			m_IsActive = false;
			m_Cauldron.Content.Disable();
			m_Cauldron.DisableParticles();
		}
	}

	private void HandleCauldronOnActivated(object sender, EventArgs e)
	{
		m_IsActive = false;
		m_Cauldron.Content.Disable();
		m_Cauldron.DisableParticles();
		m_Cauldron.Deposit();
		SeasonalSkullCheck.Reset();
		Section componentInParent = GetComponentInParent<Section>();
		CharacterNode[] componentsInChildren = m_SpawnContainer.GetComponentsInChildren<CharacterNode>();
		m_EnemyLimit = componentsInChildren.Length;
		foreach (CharacterNode characterNode in componentsInChildren)
		{
			if (!(characterNode == m_SpawnContainer))
			{
				Enemy enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>("Enemies/Enemy_LostOne_Seasonal_Halloween_PumpkinHead");
				enemy.SetSection(componentInParent.SectionID);
				enemy.SetNode(characterNode);
				if (!m_Enemies.Contains(enemy))
				{
					m_Enemies.Add(enemy);
				}
				enemy.transform.SetParent(base.transform);
				enemy.transform.position = characterNode.transform.position;
				enemy.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
				enemy.OnInitializeOnComplete += HandleEnemyOnInitializeOnComplete;
				enemy.OnCharacterDeath -= HandleEnemyOnCharacterDeath;
				enemy.OnCharacterDeath += HandleEnemyOnCharacterDeath;
			}
		}
		m_AudioHandler_Music.Play();
		SeasonalHalloweenAchievement.Mask();
	}

	private void HandleEnemyOnInitializeOnComplete(object sender, EventArgs e)
	{
		Enemy obj = sender as Enemy;
		obj.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
		obj.SetTarget(GameManager.Instance.Player.transform);
		obj.SetState(State.Character.Follow);
		obj.Agent.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
		obj.Agent.ResetAgent();
	}

	private void HandleEnemyOnCharacterDeath(object sender, EventArgs e)
	{
		(sender as Enemy).OnCharacterDeath -= HandleEnemyOnCharacterDeath;
		m_EnemyCount++;
		if (m_EnemyCount >= m_EnemyLimit)
		{
			m_EnemyCount = 0;
			if (m_Enemies != null)
			{
				m_Enemies.Clear();
			}
			Section componentInParent = GetComponentInParent<Section>();
			SeasonalEnemy seasonalEnemy = GameManager.Instance.AssetManager.CreateAsset<SeasonalEnemy>("Enemies/Enemy_LostOne_Seasonal_Halloween");
			seasonalEnemy.SetSection(componentInParent.SectionID);
			seasonalEnemy.SetNodes(m_NodeContainer.GetComponentsInChildren<CharacterInteractionNode>());
			seasonalEnemy.transform.SetParent(base.transform);
			seasonalEnemy.transform.eulerAngles = GameManager.Instance.Player.transform.eulerAngles;
			seasonalEnemy.transform.position = GameManager.Instance.Player.transform.position - GameManager.Instance.Player.transform.forward * 7f;
			seasonalEnemy.OnInitializeOnComplete -= HandleSeasonalEnemyOnInitializeOnComplete;
			seasonalEnemy.OnInitializeOnComplete += HandleSeasonalEnemyOnInitializeOnComplete;
			seasonalEnemy.OnCharacterDeath -= HandleSeasonalEnemyOnCharacterDeath;
			seasonalEnemy.OnCharacterDeath += HandleSeasonalEnemyOnCharacterDeath;
			m_AudioHandler_Music.Stop();
		}
	}

	private void HandleSeasonalEnemyOnInitializeOnComplete(object sender, EventArgs e)
	{
		SeasonalEnemy obj = sender as SeasonalEnemy;
		obj.OnInitializeOnComplete -= HandleSeasonalEnemyOnInitializeOnComplete;
		obj.SetState(State.Character.Seasonal);
	}

	private void HandleSeasonalEnemyOnCharacterDeath(object sender, EventArgs e)
	{
		SeasonalEnemy obj = sender as SeasonalEnemy;
		obj.OnCharacterDeath -= HandleSeasonalEnemyOnCharacterDeath;
		obj.OnAnimationComplete -= HandleSeasonalEnemyDeathOnAnimationComplete;
		obj.OnAnimationComplete += HandleSeasonalEnemyDeathOnAnimationComplete;
	}

	private void HandleSeasonalEnemyDeathOnAnimationComplete(object sender, EventArgs e)
	{
		SeasonalEnemy seasonalEnemy = sender as SeasonalEnemy;
		seasonalEnemy.OnAnimationComplete -= HandleSeasonalEnemyDeathOnAnimationComplete;
		Collider[] colliders = seasonalEnemy.Content.GetComponentsInChildren<Collider>();
		Sequence sequence = DOTween.Sequence();
		Vector3 position = seasonalEnemy.transform.position;
		Vector3 position2 = position + seasonalEnemy.transform.forward * 2f + -seasonalEnemy.transform.right * 2f;
		m_LootLocation.position = position2;
		float num = 0f;
		for (int i = 0; i < 20; i++)
		{
			Slug slug = GameManager.Instance.AssetManager.CreateAsset<Slug>("Slugs/Slug_Many");
			(slug.Content as SlugContent).InitializeContent();
			slug.Content.Disable();
			slug.transform.position = position2;
			slug.transform.localScale = Vector3.zero;
			Vector3 endValue = position + UnityEngine.Random.insideUnitSphere * 2f;
			endValue.y = position2.y;
			sequence.Insert(num + 0f, slug.transform.DOScale(1f, 0.25f).SetEase(Ease.Linear));
			sequence.Insert(num + 0f, slug.transform.DOMoveY(position2.y + 7f, 0.25f).SetEase(Ease.Linear));
			sequence.Insert(num + 0.25f, slug.transform.DOMove(endValue, 0.25f).SetEase(Ease.Linear));
			sequence.InsertCallback(num, delegate
			{
				this.OnLoot.Send(this);
			});
			sequence.InsertCallback(num + 0.5f, slug.Content.Enable);
			num += 0.2f;
		}
		sequence.OnComplete(delegate
		{
			for (int j = 0; j < colliders.Length; j++)
			{
				colliders[j].enabled = true;
			}
		});
		seasonalEnemy.Dispose();
	}

	protected override void OnDisposed()
	{
		if (m_Enemies != null)
		{
			m_Enemies.Clear();
			m_Enemies = null;
		}
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		}
		base.OnDisposed();
	}
}
