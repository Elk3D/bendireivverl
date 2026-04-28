using System;
using DG.Tweening;
using UnityEngine;

public class InkDemonTest : JMonoBehaviour
{
	[Header("Cutscene")]
	[SerializeField]
	private CutsceneContent m_Cutscene;

	[Header("Event Triggers")]
	[SerializeField]
	private EventTrigger m_SafeEventTrigger;

	[SerializeField]
	private EventTrigger m_EventTrigger;

	[Header("Lost One")]
	[SerializeField]
	private Animator m_LostOneAnimator;

	[Header("Ink Demon")]
	[SerializeField]
	private Animator m_Animator;

	[Header("Player")]
	[SerializeField]
	private AnimationClip m_AudreyDeathClip;

	public event EventHandler OnDeath;

	public override void Start()
	{
		m_EventTrigger.OnEnter += HandleEventTriggerOnEnter;
		if (m_SafeEventTrigger != null)
		{
			m_SafeEventTrigger.OnEnter += HandleSafeEventTriggerOnEnter;
		}
	}

	private void HandleSafeEventTriggerOnEnter(object sender, EventArgs e)
	{
		m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		m_SafeEventTrigger.OnEnter -= HandleSafeEventTriggerOnEnter;
		m_Cutscene.Director.Stop();
		m_Cutscene.Dispose();
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		m_Cutscene.Director.Stop();
		if (m_LostOneAnimator != null)
		{
			m_LostOneAnimator.SetTrigger("Death");
		}
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.Player.ForceAbilitiesCancel();
		}
		GameManager.Instance.HideTeleport();
		Vector3 position = GameManager.Instance.Player.transform.position;
		position.y = m_Animator.transform.position.y;
		Vector3 normalized = (position - m_Animator.transform.position).normalized;
		m_Animator.transform.eulerAngles = Quaternion.LookRotation(normalized).eulerAngles;
		GameManager.Instance.Player.transform.DOMove(m_Animator.transform.position + normalized * 12f, 0.25f);
		GameManager.Instance.Player.transform.DORotate(Quaternion.LookRotation(-normalized).eulerAngles, 0.25f);
		Death();
	}

	private void Death()
	{
		this.OnDeath.Send(this);
		GameManager.Instance.LockPause();
		m_Animator.SetTrigger("Death");
		GameManager.Instance.HideCrosshair();
		GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
		m_AudreyDeathClip.name = "Interact";
		GameManager.Instance.Player.UpdatePlayerContentClipOverrides(m_AudreyDeathClip);
		GameManager.Instance.Player.EnterInteraction("Interact");
	}

	public void Respawn()
	{
		GameManager.Instance.ShowGameOver(GameOverType.InkDemon);
	}

	protected override void OnDisposed()
	{
		this.OnDeath = null;
		if (m_EventTrigger != null)
		{
			m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		}
		if (m_SafeEventTrigger != null)
		{
			m_SafeEventTrigger.OnEnter -= HandleSafeEventTriggerOnEnter;
		}
		base.OnDisposed();
	}
}
