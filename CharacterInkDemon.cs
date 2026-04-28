using System;
using DG.Tweening;
using UnityEngine;

public class CharacterInkDemon : Character
{
	[SerializeField]
	private CharacterContent m_CharacterInkDemon;

	private bool isDead;

	private HeadTracker m_HeadTracker;

	private Sequencer m_PlayerSequencer;

	public override bool JumpInput => false;

	public override bool CrouchInput => false;

	public override bool RunInput => false;

	public override float MoveXInput => 0f;

	public override float MoveYInput => 1f;

	public override float RotateXInput => 0f;

	public override bool AttackInput => false;

	public event EventHandler OnEnterPortal;

	public void ExitPortal()
	{
		SetState(State.Character.Cutscene);
		m_CharacterInkDemon.Animator.SetInteger("SpecialType", 0);
		m_CharacterInkDemon.Animator.SetTrigger("Special");
	}

	public void OnExitComplete()
	{
		base.Agent.Agent.enabled = false;
		base.Agent.Agent.enabled = true;
		GameObject.Find("AiManager")?.GetComponent<AiManager>();
		SetTarget(GameManager.Instance.Player.transform);
		SetState(State.Character.Follow);
	}

	public void EnterPortal()
	{
		SetState(State.Character.Cutscene);
		m_CharacterInkDemon.Animator.SetInteger("SpecialType", 1);
		m_CharacterInkDemon.Animator.SetTrigger("Special");
	}

	public void OnEnterComplete()
	{
		Dispose();
	}

	protected override void InternalUpdate()
	{
		base.InternalUpdate();
		if (isDead || base.CurrentState != State.Character.Follow)
		{
			return;
		}
		Player player = GameManager.Instance.Player;
		if (base.Target == player.transform)
		{
			if (Vector3.Distance(base.transform.position, player.transform.position) < 5f)
			{
				isDead = true;
				m_HeadTracker.SetActive(active: false);
				SetState(State.Character.Cutscene);
				m_CharacterInkDemon.Animator.SetTrigger("Death");
				player.Death();
				m_PlayerSequencer.CreateSequence(player.transform.DOMove(base.transform.position + base.transform.forward * 2.5f, 0.25f).SetEase(Ease.InOutSine), player.transform.DORotate(base.transform.eulerAngles + new Vector3(0f, 180f, 0f), 0.25f).SetEase(Ease.InOutSine));
			}
		}
		else if (base.Content.Animator.GetFloat("MovementState") == 0f)
		{
			this.OnEnterPortal.Send(this);
			SetState(State.Character.Cutscene);
		}
	}

	protected override void InternalInitialize()
	{
		SetContent(m_CharacterInkDemon);
	}

	protected override void InternalInitializeOnComplete()
	{
		m_PlayerSequencer = new Sequencer();
		SetState(State.Character.Cutscene);
		Transform transform = base.transform.FindDeepChild("BN_Head");
		if (transform == null)
		{
			transform = base.transform.FindDeepChild("BN_Head001");
		}
		if (transform == null)
		{
			transform = base.transform.FindDeepChild("BN_Head_01");
		}
		if (transform == null)
		{
			transform = base.transform.FindDeepChild("Head_Ctrl");
		}
		if (transform == null)
		{
			transform = base.transform.FindDeepChild("CTRL_Mouth");
		}
		if (transform != null)
		{
			m_HeadTracker = transform.gameObject.AddComponent<HeadTracker>();
			Transform transform2 = new GameObject().transform;
			transform2.SetParent(base.transform);
			transform2.localPosition = Vector3.forward;
			transform2.localEulerAngles = Vector3.zero;
			m_HeadTracker.SetForwardDirection(transform2);
			m_HeadTracker.SetOffset(new Vector3(0f, 270f, 260f));
			m_HeadTracker.SetAngle(75f);
		}
		base.Agent.Agent.enabled = false;
		base.Agent.Agent.enabled = true;
	}

	protected override void OnDisposed()
	{
		m_PlayerSequencer?.Kill();
		m_PlayerSequencer = null;
		base.OnDisposed();
	}
}
