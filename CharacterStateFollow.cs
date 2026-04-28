using System;
using UnityEngine;
using UnityEngine.AI;

public class CharacterStateFollow(Character character, State.Character state) : CharacterState(character, state)
{
	private bool m_IsStopAndGo;

	private bool m_IsPartial;

	private float m_BrokenPathTimer;

	private float m_BrokenPathTimerLimit = 3f;

	private float m_AttackDelay;

	public override void InternalOnStateEnter()
	{
		base.Actor.SetCharacterAction(null);
		if (base.Actor.Content != null)
		{
			base.Actor.Content.Animator.SetCombatState(1f);
			base.Actor.ClearAnimationTriggers();
			base.Actor.ForceStop();
		}
		if (base.Actor.IsStopAndGo)
		{
			m_IsStopAndGo = false;
			base.Actor.OnAnimationEvent -= Actor_OnAnimationEvent;
			base.Actor.OnAnimationEvent2 -= Actor_OnAnimationEvent2;
			base.Actor.OnAnimationEnter -= Actor_OnAnimationEnter;
			base.Actor.OnAnimationComplete -= Actor_OnAnimationComplete;
			base.Actor.OnAnimationEvent += Actor_OnAnimationEvent;
			base.Actor.OnAnimationEvent2 += Actor_OnAnimationEvent2;
			base.Actor.OnAnimationEnter += Actor_OnAnimationEnter;
			base.Actor.OnAnimationComplete += Actor_OnAnimationComplete;
		}
		if (base.Actor.Target == GameManager.Instance.Player.transform)
		{
			if (GameManager.Instance.Player.CombatStatus != CombatStatus.Hide)
			{
				GameManager.Instance.Player.AddEnemy(base.Actor);
			}
			else
			{
				GameManager.Instance.Player.RemoveEnemy(base.Actor);
			}
			GameManager.Instance.Player.OnAttackStart -= HandlePlayerOnAttackStart;
			GameManager.Instance.Player.OnAttackStart += HandlePlayerOnAttackStart;
		}
	}

	public override void InternalOnStateExit()
	{
		if (base.Actor.IsStopAndGo)
		{
			base.Actor.OnAnimationEvent -= Actor_OnAnimationEvent;
			base.Actor.OnAnimationEvent2 -= Actor_OnAnimationEvent2;
			base.Actor.OnAnimationEnter -= Actor_OnAnimationEnter;
			base.Actor.OnAnimationComplete -= Actor_OnAnimationComplete;
			base.Actor.Agent.Agent.angularSpeed = 360f;
			m_IsStopAndGo = false;
		}
		base.Actor.OnAnimationComplete -= HandleActorDespawnOnAnimationComplete;
		GameManager.Instance.Player.OnAttackStart -= HandlePlayerOnAttackStart;
	}

	private void Actor_OnAnimationEvent(object sender, EventArgs e)
	{
		base.Actor.Agent.Agent.angularSpeed = 360f;
	}

	private void Actor_OnAnimationEvent2(object sender, EventArgs e)
	{
		base.Actor.Agent.Agent.angularSpeed = 0f;
	}

	private void Actor_OnAnimationEnter(object sender, EventArgs e)
	{
		m_IsStopAndGo = true;
		base.Actor.Agent.Agent.angularSpeed = 0f;
	}

	private void Actor_OnAnimationComplete(object sender, EventArgs e)
	{
		m_IsStopAndGo = false;
		base.Actor.Agent.Agent.angularSpeed = 0f;
	}

	protected override void InternalUpdate()
	{
		if (GameManager.Instance.Player != null && base.Actor.Target == GameManager.Instance.Player.transform && GameManager.Instance.Player.CombatStatus == CombatStatus.Hide)
		{
			LoseTarget();
		}
		else
		{
			if (base.Actor.ActiveAction != null)
			{
				return;
			}
			if (base.Actor.IsAttackDelay)
			{
				m_AttackDelay += Time.deltaTime;
				if (m_AttackDelay >= 1.5f)
				{
					base.Actor.IsAttackDelay = false;
					m_AttackDelay = 0f;
				}
			}
			else if (base.Actor.CanAttack && base.Actor.ActionGroupAttack != null && base.Actor.ActionGroupAttack.GetAction(base.Actor) != null)
			{
				return;
			}
			if (!CheckTarget())
			{
				base.Actor.Content.Animator.SetCombatState(0f);
				base.Actor.SetState(State.Character.Idle);
				return;
			}
			base.Actor.Content.Animator.SetCombatState(1f);
			if (Vector3.Distance(base.Actor.Target.position, base.Actor.transform.position) < base.Actor.CharacterVision.CloseRange && !base.Actor.Agent.Agent.isOnOffMeshLink)
			{
				CloseLookAt();
			}
			else
			{
				if (Vector3.Distance(base.Actor.transform.position, base.Actor.Agent.Agent.destination) <= base.Actor.Agent.Agent.stoppingDistance && (base.Actor.Agent.Agent.pathStatus == NavMeshPathStatus.PathPartial || base.Actor.Agent.Agent.pathStatus == NavMeshPathStatus.PathInvalid))
				{
					CloseLookAt(isPartial: true);
				}
				else
				{
					Move();
				}
				m_AttackDelay += Time.deltaTime * 2f;
			}
			if (base.Actor.Agent.Agent.pathStatus == NavMeshPathStatus.PathPartial || base.Actor.Agent.Agent.pathStatus == NavMeshPathStatus.PathInvalid)
			{
				if (m_BrokenPathTimer >= m_BrokenPathTimerLimit)
				{
					LoseTarget();
					return;
				}
				m_BrokenPathTimer += Time.deltaTime;
			}
			else
			{
				m_BrokenPathTimer = 0f;
			}
			GetAnimationStatesTEMP();
		}
	}

	private void Move()
	{
		m_IsPartial = false;
		base.Actor.Agent.Agent.updatePosition = true;
		base.Actor.Agent.MoveTo(base.Actor.Target.position);
	}

	private void CloseLookAt(bool isPartial = false)
	{
		m_IsPartial = isPartial;
		base.Actor.Agent.Agent.updatePosition = isPartial;
		Quaternion b = Quaternion.LookRotation((base.Actor.Target.position - base.Actor.transform.position).normalized);
		Vector3 eulerAngles = b.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		b.eulerAngles = eulerAngles;
		base.Actor.transform.rotation = Quaternion.Slerp(base.Actor.transform.rotation, b, Time.deltaTime * 5f);
	}

	private void LoseTarget()
	{
		m_BrokenPathTimer = 0f;
		base.Actor.SendOnTargetLost();
		Enemy component = base.Actor.GetComponent<Enemy>();
		if ((!(component != null) || component.EnemyType != EnemyType.KingWidow) && component.EnemyType != EnemyType.InkWidow)
		{
			GameManager.Instance.Player.RemoveEnemy(base.Actor);
			if (base.Actor.CurrentNode != null)
			{
				base.Actor.SetNode(base.Actor.CurrentNode);
				base.Actor.SetState(State.Character.Patrol);
			}
			else if (component != null && component.EnemyType == EnemyType.Searcher && component.DespawnAnimationClip != null)
			{
				base.Actor.SetState(State.Character.Cutscene);
				component.DespawnAnimationClip.name = "Interact";
				base.Actor.Content.UpdateClipOverrides(component.DespawnAnimationClip);
				base.Actor.Content.SetAnimationTrigger("InteractInstant");
				base.Actor.OnAnimationComplete -= Actor_OnAnimationComplete;
				base.Actor.OnAnimationComplete -= HandleActorDespawnOnAnimationComplete;
				base.Actor.OnAnimationComplete += HandleActorDespawnOnAnimationComplete;
			}
		}
	}

	private void HandleActorDespawnOnAnimationComplete(object sender, EventArgs e)
	{
		base.Actor.OnAnimationComplete -= HandleActorDespawnOnAnimationComplete;
		base.Actor.Dispose();
	}

	private void HandlePlayerOnAttackStart(object sender, EventArgs e)
	{
		if (base.Actor != null && base.Actor.ActionGroupEvade != null)
		{
			base.Actor.ActionGroupEvade.GetAction(base.Actor);
		}
	}

	private void ForceStop(out float movementSpeed, out bool movementSpeedRun)
	{
		base.Actor.Agent.Agent.angularSpeed = 360f;
		base.Actor.Agent.Agent.velocity = Vector3.zero;
		movementSpeed = 0f;
		movementSpeedRun = false;
	}

	protected void GetAnimationStatesTEMP()
	{
		if (base.Actor == null || base.Actor.Content == null || base.Actor.Content.Animator == null)
		{
			return;
		}
		float num = Vector3.Distance(base.Actor.Target.position, base.Actor.transform.position);
		float num2 = Vector3.Distance(base.Actor.Agent.Agent.pathEndPosition, base.Actor.transform.position);
		float num3 = 0f;
		float movementSpeed = 0f;
		bool smooth = true;
		bool movementSpeedRun = false;
		if ((num2 >= 0f && num2 < base.Actor.Agent.Agent.stoppingDistance) || m_IsPartial)
		{
			ForceStop(out movementSpeed, out movementSpeedRun);
		}
		else if (num > base.Actor.CharacterVision.CloseRange + num3 && !base.Actor.IsStopAndGo)
		{
			base.Actor.Agent.Agent.speed = base.Actor.Movement.RunSpeed;
			movementSpeed = 2f;
			movementSpeedRun = true;
		}
		else if (num > base.Actor.CharacterVision.CloseRange)
		{
			if (base.Actor.IsStopAndGo)
			{
				if (m_IsStopAndGo)
				{
					base.Actor.Agent.Agent.speed = base.Actor.Movement.MoveSpeed;
					base.Actor.Agent.Agent.velocity = base.Actor.transform.forward * 5f;
				}
				else
				{
					base.Actor.Agent.Agent.velocity = Vector3.zero;
				}
			}
			else
			{
				base.Actor.Agent.Agent.speed = base.Actor.Movement.MoveSpeed;
			}
			movementSpeed = 1f;
			movementSpeedRun = false;
		}
		else if (num < base.Actor.CharacterVision.CloseRange)
		{
			ForceStop(out movementSpeed, out movementSpeedRun);
		}
		base.Actor.Content.Animator.SetMovementState(movementSpeed, smooth);
		base.Actor.Content.Animator.SetMovementSpeed(movementSpeed, smooth);
		base.Actor.SetRun(movementSpeedRun);
	}

	protected override void OnDisposed()
	{
		if (base.Actor != null)
		{
			if (base.Actor.IsStopAndGo)
			{
				base.Actor.OnAnimationEvent -= Actor_OnAnimationEvent;
				base.Actor.OnAnimationEvent2 -= Actor_OnAnimationEvent2;
				base.Actor.OnAnimationEnter -= Actor_OnAnimationEnter;
				base.Actor.OnAnimationComplete -= Actor_OnAnimationComplete;
			}
			base.Actor.OnAnimationComplete -= HandleActorDespawnOnAnimationComplete;
		}
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnAttackStart -= HandlePlayerOnAttackStart;
			GameManager.Instance.Player.RemoveEnemy(base.Actor);
		}
		base.OnDisposed();
	}
}
