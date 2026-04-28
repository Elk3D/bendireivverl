using System;
using DG.Tweening;
using UnityEngine;

public class EnemyPlayerDeathController : JMonoBehaviour
{
	[SerializeField]
	private Character m_Target;

	[SerializeField]
	private Transform m_StartLocation;

	[SerializeField]
	private AnimationClip m_TargetDeathAnimation;

	[SerializeField]
	private AnimationClip m_PlayerDeathAnimation;

	public void Activate()
	{
		GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		GameManager.Instance.Player.OnRespawn -= HandlePlayerOnRespawn;
		GameManager.Instance.Player.OnRespawn += HandlePlayerOnRespawn;
		if (m_TargetDeathAnimation != null)
		{
			GameManager.Instance.Player.OnDeath += HandlePlayerOnDeath;
			GameManager.Instance.Player.SetDeathSequence(active: true);
		}
	}

	public void Deactivate()
	{
		GameManager.Instance.Player.OnRespawn -= HandlePlayerOnRespawn;
		GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		if (m_TargetDeathAnimation != null)
		{
			GameManager.Instance.Player.SetDeathSequence(active: false);
			m_Target.OnAnimationComplete -= HandleTargetOnAnimationComplete;
		}
	}

	private void HandlePlayerOnRespawn(object sender, EventArgs e)
	{
		if (m_Target != null && m_StartLocation != null)
		{
			m_Target.SetState(State.Character.Cutscene);
			m_Target.SetTarget(null);
			m_Target.transform.position = m_StartLocation.position;
			m_Target.transform.eulerAngles = m_StartLocation.eulerAngles;
			m_Target.CancelPath();
			m_Target.Content.Animator.SetMovementState(0f, smooth: false);
			m_Target.Content.Animator.SetMovementSpeed(0f, smooth: false);
			m_Target.ClearAnimationTriggers();
			m_Target.Content.Animator.enabled = false;
			m_Target.Content.Animator.enabled = true;
			DOTween.Sequence().InsertCallback(4f, delegate
			{
				m_Target.SetTarget(GameManager.Instance.Player.transform);
				m_Target.SetState(State.Character.Follow);
				m_Target.Agent.ResetAgent();
			});
		}
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		m_TargetDeathAnimation.name = "Cutscene";
		m_Target.Content.UpdateClipOverrides(m_TargetDeathAnimation);
		m_Target.Content.SetAnimationTrigger("Cutscene");
		m_PlayerDeathAnimation.name = "Cutscene";
		GameManager.Instance.Player.UpdatePlayerContentClipOverrides(m_PlayerDeathAnimation);
		GameManager.Instance.Player.Death();
		CameraEffects.Damage();
		GameManager.Instance.Player.transform.DORotate(m_Target.transform.eulerAngles + new Vector3(0f, 180f, 0f), 0.3f).SetEase(Ease.InOutSine);
		GameManager.Instance.Player.transform.DOMove(m_Target.transform.position + m_Target.transform.forward * 5f, 0.3f).SetEase(Ease.InOutSine);
		m_Target.OnAnimationComplete -= HandleTargetOnAnimationComplete;
		m_Target.OnAnimationComplete += HandleTargetOnAnimationComplete;
	}

	private void HandleTargetOnAnimationComplete(object sender, EventArgs e)
	{
		m_Target.OnAnimationComplete -= HandleTargetOnAnimationComplete;
		GameManager.Instance.Player.ForceRespawn(0.3f);
	}
}
