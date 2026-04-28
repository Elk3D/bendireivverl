using System;
using S13Audio.BATDR;
using UnityEngine;

public class CharacterStateEvade : CharacterState
{
	private CharacterActionEvade m_Action;

	private bool m_IsMoving;

	public CharacterStateEvade(Character character, State.Character state)
		: base(character, state)
	{
	}

	public override void InternalOnStateEnter()
	{
		if (base.Actor is Enemy enemy)
		{
			enemy.SetImmune(active: true);
		}
		base.Actor.Agent.Agent.updatePosition = true;
		m_Action = base.Actor.ActiveAction as CharacterActionEvade;
		m_IsMoving = false;
		base.Actor.ForceStop();
		base.Actor.SetMoveSpeed(0f);
		base.Actor.SetMoveXSpeed(0f);
		RemoveListeners();
		AddListeners();
		base.Actor.Content?.SetAnimationTrigger(m_Action.AnimationTrigger);
		base.Actor.AudioController?.React(BATDRNpcAudioController.NPCReaction.Evade);
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
		if (m_Action != null)
		{
			Vector3Direction direction = m_Action.Direction;
			float speed = m_Action.Speed;
			float num = speed / 2f;
			float num2 = UnityEngine.Random.Range(0f - num, num);
			switch (direction)
			{
			case Vector3Direction.Forward:
				base.Actor.SetMoveSpeed(speed);
				base.Actor.SetMoveXSpeed(num2);
				break;
			case Vector3Direction.Back:
				base.Actor.SetMoveSpeed(0f - speed);
				base.Actor.SetMoveXSpeed(num2);
				break;
			case Vector3Direction.Left:
				base.Actor.SetMoveXSpeed(0f - speed);
				base.Actor.SetMoveSpeed(num2);
				break;
			case Vector3Direction.Right:
				base.Actor.SetMoveXSpeed(speed);
				base.Actor.SetMoveSpeed(num2);
				break;
			}
			m_IsMoving = true;
		}
	}

	private void HandleActorOnAnimationComplete(object sender, EventArgs e)
	{
		base.Actor.OnAnimationComplete -= HandleActorOnAnimationComplete;
		m_IsMoving = false;
		base.Actor.ForceStop(smooth: false);
		base.Actor.Agent.Agent.updatePosition = true;
		if (base.Actor is Enemy enemy)
		{
			enemy.SetImmune(active: false);
		}
	}

	public override void InternalOnStateExit()
	{
		RemoveListeners();
		base.Actor.SetMoveSpeed(base.Actor.Movement.MoveSpeed);
		base.Actor.SetMoveXSpeed(0f);
		base.Actor.ForceStop(smooth: false);
		m_Action = null;
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

	protected override void OnDisposed()
	{
		m_Action = null;
		RemoveListeners();
		base.OnDisposed();
	}
}
