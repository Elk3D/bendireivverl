using System;
using DG.Tweening;
using S13Audio.BATDR;
using UnityEngine;

[DefaultExecutionOrder(-50)]
public abstract class Character : CharacterStateMachine
{
	[SerializeField]
	private CharacterMovement m_Movement;

	[Header("TEST")]
	[SerializeField]
	private bool m_IsStopAndGo;

	[SerializeField]
	private Transform m_WeaponParent;

	[SerializeField]
	private CharacterActionVision m_CharacterVision;

	[SerializeField]
	private CharacterActionGroup m_ActionGroupAttack;

	[SerializeField]
	private CharacterActionGroup m_ActionGroupEvade;

	protected CharacterAction m_ActiveAction;

	private bool m_DisableVision;

	private bool m_CanAttack = true;

	private Sequence m_SlideSequence;

	protected float m_MoveSpeed = 0.5f;

	protected float m_MoveXSpeed;

	protected bool m_RunInput;

	public bool IsAlerted;

	[HideInInspector]
	public bool IsAttackDelay;

	public abstract bool JumpInput { get; }

	public abstract bool CrouchInput { get; }

	public abstract bool RunInput { get; }

	public abstract float MoveXInput { get; }

	public abstract float MoveYInput { get; }

	public abstract float RotateXInput { get; }

	public abstract bool AttackInput { get; }

	public CharacterMovement Movement => m_Movement;

	public bool IsStopAndGo => m_IsStopAndGo;

	public Transform WeaponParent => m_WeaponParent;

	public CharacterActionVision CharacterVision => m_CharacterVision;

	public CharacterActionGroup ActionGroupAttack => m_ActionGroupAttack;

	public CharacterActionGroup ActionGroupEvade => m_ActionGroupEvade;

	public CharacterAction ActiveAction => m_ActiveAction;

	public CharacterContent Content { get; private set; }

	public CharacterController Controller { get; private set; }

	public CharacterRotation Rotation { get; private set; }

	public CharacterAgent Agent { get; private set; }

	public CharacterNode CurrentNode { get; private set; }

	public CharacterNode PreviousNode { get; private set; }

	public Transform Target { get; private set; }

	public bool DisableVision => m_DisableVision;

	public bool CanAttack => m_CanAttack;

	public bool InCombat
	{
		get
		{
			if (GameManager.Instance.Player == null)
			{
				return false;
			}
			return Target == GameManager.Instance.Player.transform;
		}
	}

	public BATDRNpcAudioController AudioController { get; private set; }

	public Vector3 VisionPosition
	{
		get
		{
			if (Content.EyeSight != null)
			{
				return Content.EyeSight.position;
			}
			return base.transform.position + Vector3.up * (Agent.Agent.height / 2f);
		}
	}

	public event EventHandler OnInitializeOnComplete;

	public event EventHandler OnAnimationEvent;

	public event EventHandler OnAnimationEvent2;

	public event EventHandler OnAnimationEnter;

	public event EventHandler OnAnimationComplete;

	public event EventHandler OnNodeReached;

	public event EventHandler OnDamageTaken;

	public event EventHandler OnTargetLost;

	public event EventHandler OnCharacterDeath;

	public void SetCharacterAction(CharacterAction action)
	{
		m_ActiveAction = action;
	}

	public void SetMoveSpeed(float speed)
	{
		m_MoveSpeed = speed;
	}

	public void SetMoveXSpeed(float speed)
	{
		m_MoveXSpeed = speed;
	}

	public void SetRun(bool isRunning)
	{
		m_RunInput = isRunning;
	}

	public void SetTarget(Transform target)
	{
		Target = target;
	}

	public void SetDisableVision(bool active)
	{
		m_DisableVision = active;
	}

	public void SetCanAttack(bool active)
	{
		m_CanAttack = active;
	}

	protected override void InternalInitializeOnComplete()
	{
		GetCharacterContent();
		SetupCharacterContent();
		SetState(State.Character.Idle);
		GetAudioController();
		CharacterInitialized();
		SendOnInitializeOnComplete();
	}

	protected virtual void CharacterInitialized()
	{
	}

	protected void GetCharacterContent()
	{
		Content = null;
		SetContent(base.gameObject.GetComponentInChildren<CharacterContent>());
	}

	protected void SetContent(CharacterContent content)
	{
		if (!(content == null))
		{
			Content = content;
			Content.GenericAnimationEvents.SetReciever(this);
			Content.Initialize();
		}
	}

	protected void SetupCharacterContent()
	{
		if (Controller == null)
		{
			Controller = base.gameObject.GetComponent<CharacterController>();
		}
		m_Movement.Initialize(this);
		if (Rotation == null)
		{
			Rotation = new CharacterRotation(this);
		}
		if (Agent == null)
		{
			Agent = new CharacterAgent(this);
		}
		if (Agent != null)
		{
			Agent.ResetAgent();
		}
	}

	protected void GetAudioController()
	{
		if (AudioController == null)
		{
			AudioController = base.gameObject.GetComponentInChildren<BATDRNpcAudioController>();
		}
	}

	public virtual void OnInteract()
	{
	}

	public virtual void OnAttack()
	{
	}

	public virtual void OnAttackRanged()
	{
	}

	public virtual void OnHit(RaycastHit hit)
	{
	}

	public virtual void OnDeath()
	{
	}

	public void SendOnAnimationEnter()
	{
		this.OnAnimationEnter.Send(this);
	}

	public void AnimationEvent()
	{
		this.OnAnimationEvent.Send(this);
	}

	public void AnimationEvent2()
	{
		this.OnAnimationEvent2.Send(this);
	}

	public void AnimationEnter()
	{
		this.OnAnimationEnter.Send(this);
	}

	public void AnimationComplete()
	{
		this.OnAnimationComplete.Send(this);
	}

	public void CharacterDeath()
	{
		GameManager.Instance.Player.RemoveEnemy(this);
		this.OnCharacterDeath.Send(this);
	}

	public void SetNode(CharacterNode node)
	{
		if (CurrentNode != null)
		{
			CurrentNode.OnReached -= HandleNodeOnReached;
		}
		PreviousNode = CurrentNode;
		if (node != null)
		{
			CurrentNode = node;
			InternalSetNode(CurrentNode);
			CurrentNode.OnReached -= HandleNodeOnReached;
			CurrentNode.OnReached += HandleNodeOnReached;
			SetTarget(CurrentNode.transform);
		}
		else
		{
			SetTarget(null);
		}
	}

	protected virtual void InternalSetNode(CharacterNode node)
	{
	}

	private void HandleNodeOnReached(object sender, EventArgs e)
	{
		CharacterNode characterNode = sender as CharacterNode;
		characterNode.OnReached -= HandleNodeOnReached;
		Rotation?.ForceRotation(characterNode.transform.rotation);
		InternalHandleNodeOnReached(characterNode);
		SlideTo(characterNode.transform);
		SendOnNodeReached();
	}

	protected virtual void InternalHandleNodeOnReached(CharacterNode node)
	{
	}

	public void ForceNodeOnReached(float duration = 0.35f)
	{
		if (!(CurrentNode == null))
		{
			Rotation?.ForceRotation(CurrentNode.transform.rotation);
			InternalHandleNodeOnReached(CurrentNode);
			SlideTo(CurrentNode.transform, duration);
			SendOnNodeReached();
		}
	}

	public void SlideTo(Transform target, float duration = 0.35f, Action onComplete = null)
	{
		m_SlideSequence?.Kill();
		m_SlideSequence = DOTween.Sequence();
		m_SlideSequence.SetUpdate(UpdateType.Fixed);
		m_SlideSequence.Insert(0f, base.transform.DOMove(target.position, duration).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
		m_SlideSequence.Insert(0f, base.transform.DORotate(target.eulerAngles, duration).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
		m_SlideSequence.OnComplete(delegate
		{
			onComplete?.Invoke();
		});
	}

	public void CancelSlide()
	{
		m_SlideSequence?.Kill();
		m_SlideSequence = null;
	}

	public void ClearAnimationTriggers()
	{
		if (Content != null && Content.Animator != null)
		{
			Content.Animator.ResetTrigger("Attack");
			Content.Animator.ResetTrigger("AttackBehind");
			Content.Animator.ResetTrigger("AttackLeap");
			Content.Animator.ResetTrigger("AttackRanged");
		}
	}

	public void CancelPath()
	{
		if (Agent != null)
		{
			Agent.CancelPath();
		}
	}

	public void ForceStop(bool smooth = true)
	{
		CancelPath();
		SetRun(isRunning: false);
		if (Content != null && Content.Animator != null)
		{
			Content.Animator.SetMovementState(0f, smooth);
			Content.Animator.SetMovementSpeed(0f, smooth);
		}
	}

	public void ForceRotation(Quaternion quaternion)
	{
		Rotation.ForceRotation(quaternion);
		Rotation.Update();
	}

	protected void SendOnInitializeOnComplete()
	{
		this.OnInitializeOnComplete.Send(this);
	}

	public void SendOnNodeReached()
	{
		this.OnNodeReached.Send(this);
	}

	public void SendOnTargetLost()
	{
		this.OnTargetLost.Send(this);
	}

	public void SendOnDamageTaken()
	{
		this.OnDamageTaken.Send(this);
	}

	protected override void OnDisposed()
	{
		m_SlideSequence?.Kill();
		m_SlideSequence = null;
		this.OnInitializeOnComplete = null;
		this.OnAnimationEvent = null;
		this.OnAnimationEvent2 = null;
		this.OnAnimationEnter = null;
		this.OnAnimationComplete = null;
		this.OnNodeReached = null;
		this.OnDamageTaken = null;
		this.OnTargetLost = null;
		this.OnCharacterDeath = null;
		Content = null;
		Controller = null;
		base.OnDisposed();
	}
}
