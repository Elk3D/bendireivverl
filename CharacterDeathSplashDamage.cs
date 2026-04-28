using System;
using UnityEngine;

public class CharacterDeathSplashDamage : JMonoBehaviour
{
	[SerializeField]
	private float m_Radius = 3f;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_Percent = 0.6f;

	private Enemy m_Enemy;

	public override void Start()
	{
		m_Enemy = GetComponent<Enemy>();
		if (m_Enemy != null)
		{
			m_Enemy.OnCharacterDeath -= HandleCharacterOnCharacterDeath;
			m_Enemy.OnCharacterDeath += HandleCharacterOnCharacterDeath;
		}
	}

	private void HandleCharacterOnCharacterDeath(object sender, EventArgs e)
	{
		m_Enemy.OnCharacterDeath -= HandleCharacterOnCharacterDeath;
		RaycastHit hit = new RaycastHit
		{
			point = base.transform.position + Vector3.up
		};
		Collider[] array = Physics.OverlapSphere(base.transform.position, m_Radius, LayerMaskUtility.Enemy() | LayerMaskUtility.IgnoreEnemy(), QueryTriggerInteraction.Ignore);
		for (int i = 0; i < array.Length; i++)
		{
			Enemy component = array[i].GetComponent<Enemy>();
			if (!(component == null) && !(component.transform == base.transform) && component.EnemyType == EnemyType.InkWidow && UnityEngine.Random.value < m_Percent)
			{
				component.OnHit(hit);
				break;
			}
		}
	}

	protected override void OnDisposed()
	{
		if (m_Enemy != null)
		{
			m_Enemy.OnCharacterDeath -= HandleCharacterOnCharacterDeath;
			m_Enemy = null;
		}
		base.OnDisposed();
	}
}
