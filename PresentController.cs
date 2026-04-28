using System;
using DG.Tweening;
using UnityEngine;

public class PresentController : JMonoBehaviour
{
	[SerializeField]
	private Transform m_LootLocation;

	[SerializeField]
	private Transform m_NodeContainer;

	private Present[] m_Presents;

	public event EventHandler OnLoot;

	public override void Start()
	{
		m_Presents = GetComponentsInChildren<Present>(includeInactive: true);
		if (m_Presents == null)
		{
			return;
		}
		Present[] presents = m_Presents;
		foreach (Present present in presents)
		{
			if (present != null)
			{
				present.OnActivate -= HandlePresentOnActivate;
				present.OnActivate += HandlePresentOnActivate;
			}
		}
	}

	private void HandlePresentOnActivate(object sender, EventArgs e)
	{
		Present present = sender as Present;
		if (present != null)
		{
			present.OnActivate -= HandlePresentOnActivate;
		}
		if (PresentCheck.PresentCount >= 20)
		{
			Section componentInParent = GetComponentInParent<Section>();
			SeasonalEnemy seasonalEnemy = GameManager.Instance.AssetManager.CreateAsset<SeasonalEnemy>("Enemies/Enemy_LostOne_Seasonal_Winter");
			seasonalEnemy.SetSection(componentInParent.SectionID);
			seasonalEnemy.SetNodes(m_NodeContainer.GetComponentsInChildren<CharacterInteractionNode>());
			seasonalEnemy.transform.SetParent(base.transform);
			seasonalEnemy.transform.eulerAngles = GameManager.Instance.Player.transform.eulerAngles;
			seasonalEnemy.transform.position = GameManager.Instance.Player.transform.position - GameManager.Instance.Player.transform.forward * 7f;
			seasonalEnemy.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
			seasonalEnemy.OnInitializeOnComplete += HandleEnemyOnInitializeOnComplete;
			seasonalEnemy.OnCharacterDeath -= HandleEnemyOnCharacterDeath;
			seasonalEnemy.OnCharacterDeath += HandleEnemyOnCharacterDeath;
			PresentCheck.Reset();
		}
	}

	private void HandleEnemyOnInitializeOnComplete(object sender, EventArgs e)
	{
		SeasonalEnemy obj = sender as SeasonalEnemy;
		obj.OnInitializeOnComplete -= HandleEnemyOnInitializeOnComplete;
		obj.SetState(State.Character.Seasonal);
	}

	private void HandleEnemyOnCharacterDeath(object sender, EventArgs e)
	{
		SeasonalEnemy obj = sender as SeasonalEnemy;
		obj.OnCharacterDeath -= HandleEnemyOnCharacterDeath;
		obj.OnAnimationComplete -= HandleEnemyDeathOnAnimationComplete;
		obj.OnAnimationComplete += HandleEnemyDeathOnAnimationComplete;
	}

	private void HandleEnemyDeathOnAnimationComplete(object sender, EventArgs e)
	{
		SeasonalEnemy seasonalEnemy = sender as SeasonalEnemy;
		seasonalEnemy.OnAnimationComplete -= HandleEnemyDeathOnAnimationComplete;
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
		if (m_Presents != null)
		{
			Present[] presents = m_Presents;
			foreach (Present present in presents)
			{
				if (present != null)
				{
					present.OnActivate -= HandlePresentOnActivate;
				}
			}
		}
		base.OnDisposed();
	}
}
