using System;
using S13Audio.BATDR;
using UnityEngine;

public class CharacterStateAttack(Character character, State.Character state) : CharacterState(character, state)
{
	private CharacterActionAttack m_Action;

	private float m_AttackType;

	private float m_LastAttackType;

	private float[] m_AttackTypes = new float[3] { 0f, 1f, 2f };

	private bool m_IsMoving;

	public override void InternalOnStateEnter()
	{
		base.Actor.ForceStop();
		base.Actor.SetMoveSpeed(0f);
		base.Actor.SetMoveXSpeed(0f);
		m_IsMoving = false;
		m_Action = base.Actor.ActiveAction as CharacterActionAttack;
		GetAttackType();
		RemoveListeners();
		AddListeners();
		string animationTrigger = (m_Action ? m_Action.AnimationTrigger : "Attack");
		base.Actor.Content?.SetAnimationTrigger(animationTrigger);
		base.Actor.AudioController?.React(BATDRNpcAudioController.NPCReaction.Attack);
	}

	protected override void InternalUpdate()
	{
		if (m_IsMoving)
		{
			base.Actor.Movement?.UpdateInput();
		}
	}

	protected override void InternalFixedUpdate()
	{
		if (m_IsMoving)
		{
			base.Actor.Movement?.Update();
		}
	}

	private void HandleActorOnAnimationEnter(object sender, EventArgs e)
	{
		base.Actor.OnAnimationEnter -= HandleActorOnAnimationEnter;
		Vector3 vector = (m_Action ? m_Action.StartRotation : Vector3.zero);
		base.Actor.ForceRotation(Quaternion.Euler(base.Actor.transform.eulerAngles + vector));
		float num = (m_Action ? m_Action.MoveSpeed : 0f);
		if (num != 0f)
		{
			base.Actor.SetRun(isRunning: true);
			base.Actor.SetMoveSpeed(num);
			m_IsMoving = true;
		}
	}

	private void HandleActorOnAnimationComplete(object sender, EventArgs e)
	{
		base.Actor.OnAnimationComplete -= HandleActorOnAnimationComplete;
		m_IsMoving = false;
		base.Actor.ForceStop(smooth: false);
	}

	public override void InternalOnStateExit()
	{
		base.Actor.IsAttackDelay = true;
		RemoveListeners();
		base.Actor.SetMoveSpeed(base.Actor.Movement.MoveSpeed);
		base.Actor.SetMoveXSpeed(0f);
		base.Actor.ForceStop(smooth: false);
		m_Action = null;
	}

	private void GetAttackType()
	{
		while (m_AttackType == m_LastAttackType)
		{
			m_AttackType = m_AttackTypes[UnityEngine.Random.Range(0, m_AttackTypes.Length)];
		}
		m_LastAttackType = m_AttackType;
		base.Actor.Content.Animator.SetAttackType(m_AttackType);
	}

	private void AddListeners()
	{
		base.Actor.OnAnimationEnter += HandleActorOnAnimationEnter;
		base.Actor.OnAnimationComplete += HandleActorOnAnimationComplete;
	}

	private void RemoveListeners()
	{
		base.Actor.OnAnimationEnter -= HandleActorOnAnimationEnter;
		base.Actor.OnAnimationComplete -= HandleActorOnAnimationComplete;
	}
}
