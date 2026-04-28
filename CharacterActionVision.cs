using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Character/New Character Action Vision")]
public class CharacterActionVision : ScriptableObject
{
	[Header("Vision Settings")]
	[SerializeField]
	[Range(0f, 180f)]
	private float m_Angle;

	[SerializeField]
	[Range(0f, 180f)]
	private float m_StealthAngle;

	[SerializeField]
	private float m_VerticalDistance = 10f;

	[SerializeField]
	private float m_TooCloseRange = 3.5f;

	[SerializeField]
	public float m_CloseRange;

	[SerializeField]
	public float m_AwareRange;

	[SerializeField]
	public float m_SightRange;

	public float Angle => m_Angle;

	public float StealthAngle => m_StealthAngle;

	public float VerticalDistance => m_VerticalDistance;

	public float TooCloseRange => m_TooCloseRange;

	public float CloseRange => m_CloseRange;

	public float AwareRange => m_AwareRange;

	public float SightRange => m_SightRange;

	public bool Validated(Character character)
	{
		bool result = false;
		float angle = ((GameManager.Instance.Player.CombatStatus == CombatStatus.Stealth) ? m_StealthAngle : m_Angle);
		if (VisionUtility.CheckFOV(character.transform.forward, character.VisionPosition, character.Target, angle, m_AwareRange, ~(1 << LayerMask.NameToLayer("AI"))))
		{
			result = true;
		}
		return result;
	}
}
