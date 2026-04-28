using System;
using DG.Tweening;

public class CharacterStateStun : CharacterState
{
	private DOTweenAnimation m_DOTweenAnimation;

	public CharacterStateStun(Character character, State.Character state)
		: base(character, state)
	{
	}

	public override void InternalOnStateEnter()
	{
		base.Actor.SetPreviousState(State.Character.Patrol);
		base.Actor.ForceStop(smooth: false);
		m_DOTweenAnimation = base.Actor.GetComponentInChildren<DOTweenAnimation>(includeInactive: true);
		SetTween(active: false);
		Enemy component = base.Actor.GetComponent<Enemy>();
		if (component != null)
		{
			component.StunAnimationClip.name = "Interact";
			base.Actor.Content.UpdateClipOverride(component.StunAnimationClip);
			base.Actor.Content.SetAnimationTrigger("Interact");
			base.Actor.OnAnimationComplete -= HandleActorOnAnimationComplete;
			base.Actor.OnAnimationComplete += HandleActorOnAnimationComplete;
		}
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.RemoveEnemy(base.Actor);
		}
	}

	private void HandleActorOnAnimationComplete(object sender, EventArgs e)
	{
		base.Actor.OnAnimationComplete -= HandleActorOnAnimationComplete;
		base.Actor.SetState(State.Character.Patrol);
		SetTween(active: true);
	}

	private void SetTween(bool active)
	{
		if (m_DOTweenAnimation != null)
		{
			if (active)
			{
				m_DOTweenAnimation.DOPlay();
			}
			else
			{
				m_DOTweenAnimation.DOPause();
			}
		}
	}

	protected override void OnDisposed()
	{
		m_DOTweenAnimation = null;
		base.Actor.OnAnimationComplete -= HandleActorOnAnimationComplete;
		base.OnDisposed();
	}
}
