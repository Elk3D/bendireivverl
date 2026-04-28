using S13Audio.BATDR;
using UnityEngine;

public class CharacterStateHit(Character character, State.Character state) : CharacterState(character, state)
{
	private float m_HitType;

	private float m_LastHitType;

	private float[] m_HitTypes = new float[2] { 0f, 1f };

	public override void InternalOnStateEnter()
	{
		CheckHitType();
		base.Actor.SetPreviousState(State.Character.Idle);
		if (GameManager.Instance.Player != null && base.Actor.Target != GameManager.Instance.Player.transform)
		{
			base.Actor.SetTarget(GameManager.Instance.Player.transform);
		}
		base.Actor.ForceStop(smooth: false);
		base.Actor.Content.Animator.Hit(m_HitType);
		base.Actor.AudioController?.React(BATDRNpcAudioController.NPCReaction.Hit);
	}

	private void CheckHitType()
	{
		while (m_HitType == m_LastHitType)
		{
			m_HitType = m_HitTypes[Random.Range(0, m_HitTypes.Length)];
		}
		m_LastHitType = m_HitType;
	}
}
