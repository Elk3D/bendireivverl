using System;
using System.Collections.Generic;
using UnityEngine;

namespace S13Audio.BATDR;

[RequireComponent(typeof(S13AccessorToParameters))]
public class BATDRCombatMusic : MonoBehaviour
{
	[Serializable]
	internal struct EnemyContent
	{
		public bool isEmpty;

		public EnemyType type;

		public S13Object combatMusic;

		public S13Object discoveredStinger;

		public S13Object defeatEnemiesStinger;

		public S13Object hideFromEnemiesStinger;
	}

	[Header("Combat Settings")]
	[Tooltip("First element is designated as the default generic content")]
	[SerializeField]
	private List<EnemyContent> _enemyContent = new List<EnemyContent>();

	[Header("Actions")]
	[SerializeField]
	[Tooltip("Triggers when alert music starts")]
	private S13LocalAction alertAction;

	[SerializeField]
	[Tooltip("Triggers when all enemies are defeated")]
	private S13LocalAction defeatedEnemiesAction;

	[SerializeField]
	[Tooltip("Triggers when hidden from all enemies")]
	private S13LocalAction hiddenFromEnemiesAction;

	[Header("Danger Settings")]
	[SerializeField]
	private S13Object dangerMusic;

	[SerializeField]
	[Tooltip("Triggers when danger music starts")]
	private S13LocalAction startDangerAction;

	[SerializeField]
	[Tooltip("Triggers when danger music stops")]
	private S13LocalAction stopDangerAction;

	[Header("Parameters")]
	[SerializeField]
	private float dangerDistanceThreshold = 35f;

	private S13AccessorToParameters dangerVolume;

	[Header("Debug")]
	[Tooltip("For Testing purposes only")]
	[SerializeField]
	private float currentDistance;

	private bool IsPlaying
	{
		get
		{
			foreach (EnemyContent item in _enemyContent)
			{
				if (item.combatMusic.isPlaying)
				{
					return true;
				}
			}
			return false;
		}
	}

	private bool DangerWithinThreshold => currentDistance >= dangerDistanceThreshold;

	private void Awake()
	{
		dangerVolume = GetComponent<S13AccessorToParameters>();
	}

	private void OnDisable()
	{
		foreach (EnemyContent item in _enemyContent)
		{
			item.combatMusic.Stop();
		}
		dangerVolume.UpdateValue(float.PositiveInfinity);
	}

	public void OnAlert(EnemyType enemyType = EnemyType.Generic)
	{
		if (!base.enabled)
		{
			return;
		}
		StopDanger();
		alertAction.Execute();
		EnemyContent playingContentType = GetPlayingContentType();
		if (playingContentType.hideFromEnemiesStinger.isPlaying)
		{
			playingContentType.hideFromEnemiesStinger.Stop(ignoreFades: true);
		}
		if (playingContentType.defeatEnemiesStinger.isPlaying)
		{
			playingContentType.defeatEnemiesStinger.Stop(ignoreFades: true);
		}
		EnemyContent matchingContentType = GetMatchingContentType(enemyType);
		if (!matchingContentType.isEmpty)
		{
			if (matchingContentType.discoveredStinger.isPlaying)
			{
				matchingContentType.discoveredStinger.Stop(ignoreFades: true);
			}
			matchingContentType.discoveredStinger.Play();
			matchingContentType.combatMusic.Play();
		}
	}

	public void OnDefeatedEnemies()
	{
		if (base.enabled && IsPlaying)
		{
			StopCombat();
			GetPlayingContentType().defeatEnemiesStinger.Play();
			defeatedEnemiesAction.Execute();
		}
	}

	public void OnHiddenFromEnemies()
	{
		if (base.enabled && IsPlaying)
		{
			StopCombat();
			GetPlayingContentType().hideFromEnemiesStinger.Play();
			hiddenFromEnemiesAction.Execute();
		}
	}

	public void StopCombat()
	{
		if (!base.enabled)
		{
			return;
		}
		foreach (EnemyContent item in _enemyContent)
		{
			item.combatMusic.Stop();
		}
		UpdateEnemyDistance(currentDistance);
	}

	public void UpdateEnemyDistance(float distanceToClosestEnemy)
	{
		if (base.enabled && !IsPlaying)
		{
			currentDistance = distanceToClosestEnemy;
			float nextValue;
			if (DangerWithinThreshold)
			{
				nextValue = 1f;
				StopDanger();
			}
			else
			{
				nextValue = Mathf.Clamp01(currentDistance / dangerDistanceThreshold);
				PlayDanger();
			}
			dangerVolume.UpdateValue(nextValue);
		}
	}

	private EnemyContent GetMatchingContentType(EnemyType enemyType)
	{
		foreach (EnemyContent item in _enemyContent)
		{
			if (enemyType == item.type)
			{
				return item;
			}
		}
		return new EnemyContent
		{
			isEmpty = true
		};
	}

	private EnemyContent GetPlayingContentType()
	{
		foreach (EnemyContent item in _enemyContent)
		{
			if (item.combatMusic.isPlaying)
			{
				return item;
			}
		}
		return _enemyContent[0];
	}

	private void PlayDanger()
	{
		if (!dangerMusic.isPlaying)
		{
			dangerMusic.Play();
			startDangerAction.Execute();
		}
	}

	public void StopDanger()
	{
		if (dangerMusic.isPlaying)
		{
			dangerMusic.Stop();
			stopDangerAction.Execute();
		}
	}
}
