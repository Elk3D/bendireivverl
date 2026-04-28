using System;
using UnityEngine;
using UnityEngine.AI;

public class CharacterStatePatrol(Character character, State.Character state) : CharacterState(character, state)
{
	private bool m_IsStopAndGo;

	private float m_InactiveTimer;

	private float m_InactiveTimerLimit = 5f;

	private Vector3 m_CurrentPosition;

	private Vector3 m_PreviousPosition;

	public override void InternalOnStateEnter()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.RemoveEnemy(base.Actor);
		}
		base.Actor.SetCharacterAction(null);
		base.Actor.Content.Animator.SetCombatState(0f);
		base.Actor.ClearAnimationTriggers();
		base.Actor.ForceStop();
		base.Actor.Agent.Agent.updatePosition = true;
		base.Actor.Agent.Agent.updateRotation = true;
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
		base.Actor.IsAlerted = false;
		base.Actor.OnDamageTaken -= HandleCharacterOnDamageTaken;
		base.Actor.OnDamageTaken += HandleCharacterOnDamageTaken;
	}

	private void HandleCharacterOnDamageTaken(object sender, EventArgs e)
	{
		base.Actor.OnDamageTaken -= HandleCharacterOnDamageTaken;
		SetTargetPlayer(checkAbility: false);
	}

	public override void InternalOnStateExit()
	{
		if (base.Actor != null)
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
			base.Actor.OnDamageTaken -= HandleCharacterOnDamageTaken;
			base.Actor.OnAnimationComplete -= HandleAnimationComplete;
		}
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
		if (!CheckPlayerTarget() && !(base.Actor.ActiveAction != null) && !EvadeInactive() && !EvadeEnemy() && !CheckForPlayer() && !(base.Actor.Target == null) && !CheckPatrol() && !(base.Actor.Target == null))
		{
			m_PreviousPosition = base.Actor.transform.position;
			base.Actor.Agent.MoveTo(base.Actor.Target.position);
			m_CurrentPosition = base.Actor.transform.position;
			GetAnimationStatesTEMP();
		}
	}

	private bool CheckPlayerTarget()
	{
		bool result = false;
		if (GameManager.Instance.Player != null && base.Actor.Target == GameManager.Instance.Player.transform && base.Actor.CurrentNode != null)
		{
			base.Actor.SetTarget(base.Actor.CurrentNode.transform);
			result = true;
		}
		return result;
	}

	private bool EvadeInactive()
	{
		if (Vector3.Distance(m_PreviousPosition, m_CurrentPosition) < 0.005f)
		{
			if (m_InactiveTimer >= m_InactiveTimerLimit)
			{
				Collider[] array = Physics.OverlapSphere(base.Actor.transform.position + Vector3.up * (base.Actor.Agent.Agent.height / 2f), base.Actor.Agent.Agent.radius * 2f, -1, QueryTriggerInteraction.Ignore);
				Collider[] array2 = array;
				foreach (Collider collider in array2)
				{
					if (array != null && !(collider.transform == base.Actor.transform) && base.Actor.ActionGroupEvade != null && base.Actor.ActionGroupEvade.GetAction(base.Actor, collider.transform) != null)
					{
						m_InactiveTimer = 0f;
						return true;
					}
				}
			}
			else
			{
				m_InactiveTimer += Time.deltaTime;
			}
		}
		else
		{
			m_InactiveTimer = 0f;
		}
		return false;
	}

	private bool EvadeEnemy()
	{
		Collider[] array = Physics.OverlapSphere(base.Actor.transform.position + Vector3.up * (base.Actor.Agent.Agent.height / 2f), base.Actor.Agent.Agent.radius * 1.6f, 1 << LayerMask.NameToLayer("AI"), QueryTriggerInteraction.Ignore);
		if (array.Length > 1)
		{
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				if (array != null && !(collider.transform == base.Actor.transform) && base.Actor.ActionGroupEvade != null && base.Actor.ActionGroupEvade.GetAction(base.Actor, collider.transform) != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool CheckForPlayer()
	{
		if (GameManager.Instance.Player == null || GameManager.Instance.Player.CombatStatus == CombatStatus.Hide)
		{
			return false;
		}
		if (base.Actor.CharacterVision != null)
		{
			float num = GameManager.Instance.Player.transform.position.y - base.Actor.transform.position.y;
			if (num > base.Actor.CharacterVision.VerticalDistance || num < 0f - base.Actor.CharacterVision.VerticalDistance)
			{
				return false;
			}
			float num2 = Vector3.Distance(base.Actor.transform.position, GameManager.Instance.Player.transform.position);
			if (base.Actor.Target != GameManager.Instance.Player.transform && num2 <= base.Actor.CharacterVision.CloseRange && GameManager.Instance.Player.CombatStatus != CombatStatus.Stealth && GameManager.Instance.Player.CombatStatus != CombatStatus.Hide && SetTargetPlayer())
			{
				return true;
			}
			if (num2 < base.Actor.CharacterVision.TooCloseRange && SetTargetPlayer())
			{
				return true;
			}
			float angle = ((GameManager.Instance.Player.CombatStatus == CombatStatus.Stealth) ? base.Actor.CharacterVision.StealthAngle : base.Actor.CharacterVision.Angle);
			if (VisionUtility.CheckFOV(base.Actor.transform.forward, base.Actor.VisionPosition, GameManager.Instance.Player.transform, angle, base.Actor.CharacterVision.AwareRange, ~(1 << LayerMask.NameToLayer("AI"))))
			{
				NavMesh.SamplePosition(GameManager.Instance.Player.transform.position, out var hit, 25f, -1);
				base.Actor.Agent.GetPath(hit.position, out var navMeshPath);
				if (navMeshPath.status != NavMeshPathStatus.PathComplete)
				{
					return false;
				}
				if (SetTargetPlayer())
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool CheckPatrol()
	{
		if (base.Actor.CurrentNode != null && Vector3.Distance(base.Actor.transform.position, base.Actor.CurrentNode.transform.position) <= base.Actor.Agent.Agent.stoppingDistance)
		{
			CharacterInteractionNode characterInteractionNode = base.Actor.CurrentNode as CharacterInteractionNode;
			if (characterInteractionNode != null)
			{
				if (characterInteractionNode.NodeType == CharacterNodeType.Location || base.Actor is Enemy { EnemyType: EnemyType.Searcher })
				{
					base.Actor.SetTarget(null);
					base.Actor.ForceStop();
					base.Actor.Content.Animator.SetCombatState(0f);
					base.Actor.SendOnNodeReached();
				}
				else
				{
					if (characterInteractionNode.NodeType == CharacterNodeType.Interaction)
					{
						Interaction("Interact", "Interact");
						return true;
					}
					if (characterInteractionNode.NodeType == CharacterNodeType.InteractionLoop)
					{
						Interaction("SpecialAnimation01", "SpecialAnimationEnter", "SpecialAnimation02");
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool SetTargetPlayer(bool checkAbility = true)
	{
		bool flag = true;
		if (checkAbility)
		{
			flag = GameManager.Instance.Player.CurrentState != State.Player.Ability;
		}
		if (flag)
		{
			base.Actor.SetTarget(GameManager.Instance.Player.transform);
			GameManager.Instance.BreakStealth();
			if (!base.Actor.IsAlerted)
			{
				base.Actor.SetState(State.Character.Alert);
			}
			else
			{
				base.Actor.SetState(State.Character.Follow);
			}
			return true;
		}
		return false;
	}

	private void Interaction(string clipName, string triggerName, string loopClipName = "")
	{
		CharacterInteractionNode node = base.Actor.CurrentNode as CharacterInteractionNode;
		if (!(node != null))
		{
			return;
		}
		base.Actor.SetTarget(null);
		base.Actor.ForceStop();
		base.Actor.Content.Animator.SetCombatState(0f);
		base.Actor.SlideTo(base.Actor.CurrentNode.transform, 0.75f, delegate
		{
			node.Interactable.Trigger();
			node.InteractionClip.name = clipName;
			if (loopClipName != "" && node.InteractionLoopClip != null)
			{
				node.InteractionLoopClip.name = loopClipName;
			}
			if (node.InteractionLoopClip != null)
			{
				base.Actor.Content.UpdateClipOverrides(node.InteractionClip, node.InteractionLoopClip);
			}
			else
			{
				base.Actor.Content.UpdateClipOverrides(node.InteractionClip);
			}
			base.Actor.OnAnimationComplete -= HandleAnimationComplete;
			base.Actor.OnAnimationComplete += HandleAnimationComplete;
			base.Actor.Content.SetAnimationTrigger(triggerName);
		});
	}

	private void HandleAnimationComplete(object sender, EventArgs e)
	{
		base.Actor.OnAnimationComplete -= HandleAnimationComplete;
		CharacterInteractionNode characterInteractionNode = base.Actor.CurrentNode as CharacterInteractionNode;
		if (characterInteractionNode != null && base.Actor.CurrentNode != null && characterInteractionNode.EndLocation != null)
		{
			base.Actor.transform.position = characterInteractionNode.EndLocation.position;
			base.Actor.transform.eulerAngles = characterInteractionNode.EndLocation.eulerAngles;
		}
		base.Actor.SendOnNodeReached();
	}

	protected void GetAnimationStatesTEMP()
	{
		if (base.Actor == null || base.Actor.Content == null || base.Actor.Content.Animator == null)
		{
			return;
		}
		float num = Vector3.Distance(base.Actor.Target.position, base.Actor.transform.position);
		float value = 0f;
		float value2 = 0f;
		bool smooth = true;
		bool run = false;
		if (num > base.Actor.Agent.Agent.stoppingDistance)
		{
			if (base.Actor.IsStopAndGo)
			{
				if (m_IsStopAndGo)
				{
					base.Actor.Agent.Agent.speed = 5f;
					base.Actor.Agent.Agent.velocity = base.Actor.transform.forward * 5f;
				}
				else
				{
					base.Actor.Agent.Agent.velocity = Vector3.zero;
				}
			}
			else if (base.Actor is Enemy enemy)
			{
				base.Actor.Agent.Agent.speed = enemy.PatrolSpeed;
			}
			else
			{
				base.Actor.Agent.Agent.speed = 2f;
			}
			value = 0f;
			value2 = 1f;
			run = false;
		}
		else if (num <= base.Actor.Agent.Agent.stoppingDistance)
		{
			base.Actor.Agent.Agent.angularSpeed = 360f;
			base.Actor.Agent.Agent.velocity = Vector3.zero;
			value = 0f;
			value2 = 0f;
			run = false;
		}
		base.Actor.Content.Animator.SetMovementState(value2, smooth);
		base.Actor.Content.Animator.SetMovementSpeed(value, smooth);
		base.Actor.SetRun(run);
	}

	protected override void OnDisposed()
	{
		if (base.Actor != null)
		{
			base.Actor.OnAnimationEvent -= Actor_OnAnimationEvent;
			base.Actor.OnAnimationEvent2 -= Actor_OnAnimationEvent2;
			base.Actor.OnAnimationEnter -= Actor_OnAnimationEnter;
			base.Actor.OnAnimationComplete -= Actor_OnAnimationComplete;
			base.Actor.OnDamageTaken -= HandleCharacterOnDamageTaken;
			base.Actor.OnAnimationComplete -= HandleAnimationComplete;
		}
		base.OnDisposed();
	}
}
