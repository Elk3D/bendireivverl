using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Character/New Character Action Attack")]
public class CharacterActionAttack : CharacterAction
{
	[SerializeField]
	private Vector3Direction m_Direction = Vector3Direction.Forward;

	[Header("Validate Settings")]
	[SerializeField]
	private float m_StartOffset;

	[SerializeField]
	private float m_Range = 7f;

	[SerializeField]
	private float m_Angle = 10f;

	[Header("Enter Settings")]
	[SerializeField]
	private Vector3 m_StartRotation = Vector3.zero;

	[SerializeField]
	private float m_MoveSpeed;

	[Header("Attack Settings")]
	[Header("Easy")]
	[SerializeField]
	private float m_AttackRadiusEasy = 7f;

	[SerializeField]
	private int m_AttackDamageEasy = 2;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_AttackPercentEasy = 1f;

	[Header("Normal")]
	[SerializeField]
	private float m_AttackRadius = 7f;

	[SerializeField]
	private int m_AttackDamage = 2;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_AttackPercent = 1f;

	[Header("Hard")]
	[SerializeField]
	private float m_AttackRadiusHard = 7f;

	[SerializeField]
	private int m_AttackDamageHard = 2;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_AttackPercentHard = 1f;

	public Vector3 StartRotation => m_StartRotation;

	public float MoveSpeed => m_MoveSpeed;

	private int m_PlayerLayer => LayerMaskUtility.Player();

	private int m_AILayer => ~(LayerMaskUtility.Enemy() | LayerMaskUtility.IgnoreEnemy());

	protected override State.Character m_CharacterState => State.Character.Attack;

	protected override bool OnValidated(Character character, Transform target)
	{
		bool result = false;
		if (VisionUtility.CheckFOV(GetDirection(character), GetStartPosition(character), target, m_Angle, m_Range, m_AILayer))
		{
			result = true;
		}
		return result;
	}

	protected override void OnAction(Character character)
	{
		float num = character.Agent.Agent.height / 2f;
		if (num > 3.5f)
		{
			num = 3.5f;
		}
		Vector3 vector = Vector3.up * num + Vector3.up * (num / 2f);
		Vector3 vector2 = character.transform.position + vector;
		DifficultyLevel difficulty = GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty;
		float num2 = m_AttackRadius;
		switch (difficulty)
		{
		case DifficultyLevel.Easy:
			num2 = m_AttackRadiusEasy;
			break;
		case DifficultyLevel.Hard:
			num2 = m_AttackRadiusHard;
			break;
		}
		int damage = m_AttackDamage;
		switch (difficulty)
		{
		case DifficultyLevel.Easy:
			damage = m_AttackDamageEasy;
			break;
		case DifficultyLevel.Hard:
			damage = m_AttackDamageHard;
			break;
		}
		float num3 = m_AttackPercent;
		switch (difficulty)
		{
		case DifficultyLevel.Easy:
			num3 = m_AttackPercentEasy;
			break;
		case DifficultyLevel.Hard:
			num3 = m_AttackPercentHard;
			break;
		}
		Vector3 position = vector2 + character.transform.forward * (num2 / 2f);
		Collider[] array = new Collider[1];
		if (Physics.OverlapSphereNonAlloc(position, num2 / 2f, array, m_PlayerLayer, QueryTriggerInteraction.Ignore) > 0)
		{
			Vector3 position2 = array[0].transform.position;
			Vector3 vector3 = character.transform.position + Vector3.up * (character.Agent.Agent.height / 2f);
			if (GameManager.Instance.BeastBendy == null && GameManager.Instance.GameCamera != null)
			{
				position2.y = GameManager.Instance.GameCamera.transform.position.y;
			}
			else
			{
				position2.y = vector3.y;
			}
			if (Physics.SphereCast(vector2, 0.2f, (position2 - vector2).normalized, out var hitInfo, num2, m_AILayer, QueryTriggerInteraction.Ignore) && UnityEngine.Random.value < num3)
			{
				hitInfo.collider.GetComponent<Player>()?.Hit(hitInfo, character, damage);
			}
		}
	}

	private Vector3 GetStartPosition(Character character)
	{
		float num = character.Agent.Agent.height / 2f;
		Vector3 vector = Vector3.up * num + Vector3.up * (num / 2f);
		return character.transform.position + vector + GetDirection(character) * m_StartOffset;
	}

	private Vector3 GetDirection(Character character)
	{
		Transform transform = character.transform;
		if (m_Direction == Vector3Direction.Forward)
		{
			return transform.forward;
		}
		if (m_Direction == Vector3Direction.Back)
		{
			return -transform.forward;
		}
		if (m_Direction == Vector3Direction.Left)
		{
			return transform.right;
		}
		if (m_Direction == Vector3Direction.Right)
		{
			return -transform.right;
		}
		if (m_Direction == Vector3Direction.Up)
		{
			return transform.up;
		}
		if (m_Direction == Vector3Direction.Down)
		{
			return -transform.up;
		}
		return Vector3.zero;
	}
}
