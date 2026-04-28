using System;
using DG.Tweening;
using UnityEngine;

public class Companion : Character
{
	protected CompanionDirector m_Director;

	private HeadTracker m_HeadTracker;

	private TextureSwapper m_TextureSwapper;

	private bool m_IsExiting;

	public float Timer;

	public CompanionDirector Director => m_Director;

	private CompanionNode m_CurrentCompanionNode => (CompanionNode)base.CurrentNode;

	private CompanionNode m_PreviousCompanionNode => (CompanionNode)base.PreviousNode;

	public override bool JumpInput => false;

	public override bool CrouchInput => false;

	public override bool RunInput => false;

	public override float MoveXInput => 0f;

	public override float MoveYInput => m_MoveSpeed;

	public override float RotateXInput => 0f;

	public override bool AttackInput => false;

	protected override bool UseAwake => false;

	public event EventHandler OnSpecialAnimationComplete;

	public event EventHandler OnGentPipeHit;

	public void SetDirector(CompanionDirector director)
	{
		m_Director = director;
	}

	protected override void InternalInitializeOnComplete()
	{
		GetCharacterContent();
		SetupCharacterContent();
		base.Agent.Agent.enabled = true;
		SetState(State.Character.Companion);
		GetHeadTracker();
		GetAudioController();
		SendOnInitializeOnComplete();
	}

	private void GetHeadTracker()
	{
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
			transform2.localPosition = ((base.Content.EyeSight != null) ? base.Content.EyeSight.localPosition : Vector3.zero);
			transform2.localEulerAngles = Vector3.zero;
			m_HeadTracker.SetForwardDirection(transform2);
			m_HeadTracker.SetOffset(new Vector3(0f, 270f, 260f));
			m_HeadTracker.SetAngle(55f);
			m_HeadTracker.SetSpeed(6f);
			m_HeadTracker.SetDistance(12f);
			m_TextureSwapper = base.gameObject.GetComponentInChildren<TextureSwapper>();
		}
	}

	protected override void InternalUpdate()
	{
		if (base.CurrentState == State.Character.Cutscene && !m_IsExiting && Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position) > 20f)
		{
			CompanionNode closest = m_Director.GetClosest(GameManager.Instance.Player.transform.position);
			if (m_CurrentCompanionNode != closest)
			{
				ForceExitNode();
			}
		}
	}

	public void ForceExitNode(bool isInitialize = false)
	{
		base.Content.Animator.ResetTrigger("SpecialAnimationExit");
		base.Content.Animator.ResetTrigger("SpecialAnimationEnter");
		if (!isInitialize)
		{
			m_IsExiting = true;
			if (m_CurrentCompanionNode.NodeType == CompanionNodeType.Location)
			{
				base.Agent.Agent.enabled = true;
				SetState(State.Character.Companion);
				m_IsExiting = false;
			}
			else
			{
				base.OnAnimationComplete -= HandleExitOnAnimationComplete;
				base.OnAnimationComplete += HandleExitOnAnimationComplete;
				base.Content.Animator.SetTrigger("SpecialAnimationExit");
			}
		}
		else
		{
			base.Content.Animator.SetTrigger("Exit");
			base.Agent.Agent.enabled = true;
			SetState(State.Character.Companion);
			m_IsExiting = false;
		}
	}

	private void HandleExitOnAnimationComplete(object sender, EventArgs e)
	{
		base.OnAnimationComplete -= HandleExitOnAnimationComplete;
		this.OnSpecialAnimationComplete.Send(this);
		base.Agent.Agent.enabled = true;
		SetState(State.Character.Companion);
		m_IsExiting = false;
	}

	protected override void InternalSetNode(CharacterNode node)
	{
		CompanionNode companionNode = (CompanionNode)base.CurrentNode;
		if (companionNode.NodeType == CompanionNodeType.Sit)
		{
			if (companionNode.SitLocation != null)
			{
				base.OnAnimationEvent -= HandleOnAnimationEvent;
				base.OnAnimationEvent += HandleOnAnimationEvent;
				base.OnAnimationEvent2 -= HandleOnAnimationEvent2;
				base.OnAnimationEvent2 += HandleOnAnimationEvent2;
			}
			m_Director.UpdateAnimationClips("Sit");
		}
		else if (companionNode.NodeType == CompanionNodeType.Point)
		{
			m_Director.UpdateAnimationClips("Point");
		}
	}

	private void HandleOnAnimationEvent(object sender, EventArgs e)
	{
		base.OnAnimationEvent -= HandleOnAnimationEvent;
		CompanionNode companionNode = (CompanionNode)base.CurrentNode;
		if (companionNode.SitLocation != null)
		{
			base.transform.DOKill();
			base.transform.DOMove(companionNode.SitLocation.position, 0.25f).SetEase(Ease.Linear);
		}
	}

	private void HandleOnAnimationEvent2(object sender, EventArgs e)
	{
		base.OnAnimationEvent2 -= HandleOnAnimationEvent2;
		CompanionNode companionNode = (CompanionNode)base.CurrentNode;
		base.transform.DOKill();
		base.transform.DOMove(companionNode.transform.position, 0.15f).SetEase(Ease.Linear);
	}

	public void UpdateAnimationClipOverrides(AnimationClip[] animationClips)
	{
		base.Content.UpdateClipOverrides(animationClips);
	}

	public void SetAnimationTrigger(string trigger)
	{
		base.Content.Animator.SetTrigger(trigger);
	}

	public void ResetAnimationTrigger(string trigger)
	{
		base.Content.Animator.ResetTrigger(trigger);
	}

	protected override void InternalHandleNodeOnReached(CharacterNode node)
	{
		NodeReached(node);
	}

	public void NodeReached(CharacterNode node, bool isInstant = false)
	{
		CompanionNode companionNode = (CompanionNode)node;
		JDebug.Log("HandleNodeOnReached :: " + node.name + "  ::  " + companionNode.NodeType.ToString() + " :: isInstant: " + isInstant, JDebug.JDebugType.AI);
		SetState(State.Character.Cutscene);
		base.Agent.Agent.enabled = false;
		base.Content.Animator.ResetTrigger("SpecialAnimationExit");
		string text = string.Empty;
		if (companionNode.NodeType == CompanionNodeType.Sit)
		{
			text = (isInstant ? "SpecialAnimationEnterInstant" : "SpecialAnimationEnter");
		}
		else if (companionNode.NodeType == CompanionNodeType.Point)
		{
			text = (isInstant ? "SpecialAnimationEnterInstant" : "SpecialAnimationEnter");
		}
		else if (companionNode.NodeType == CompanionNodeType.Loot)
		{
			text = "Loot";
		}
		else if (companionNode.NodeType == CompanionNodeType.Interact)
		{
			text = (isInstant ? "InteractInstant" : "Interact");
		}
		else if (companionNode.NodeType == CompanionNodeType.Location)
		{
			ForceStop(smooth: false);
		}
		if (text != string.Empty)
		{
			base.Content.Animator.ResetTrigger(text);
			base.Content.Animator.SetTrigger(text);
		}
	}

	public override void OnHit(RaycastHit hit)
	{
		m_HeadTracker.SetActive(active: false);
		float x = ((UnityEngine.Random.value > 0.5f) ? 1440 : (-1440));
		Shock();
		m_HeadTracker.transform.DOKill();
		m_HeadTracker.transform.DOLocalRotate(new Vector3(x, 0f, 0f), 1.5f, RotateMode.LocalAxisAdd).SetEase(Ease.OutElastic).SetUpdate(UpdateType.Late)
			.OnComplete(delegate
			{
				m_HeadTracker.SetActive(active: true);
				Blink();
			});
		this.OnGentPipeHit.Send(this);
	}

	public void BlinkChance()
	{
		if (m_TextureSwapper != null && UnityEngine.Random.value < -0.5f)
		{
			m_TextureSwapper.Blink();
		}
	}

	public void Blink()
	{
		if (m_TextureSwapper != null)
		{
			m_TextureSwapper.Blink();
		}
	}

	public void Shock()
	{
		if (m_TextureSwapper != null)
		{
			m_TextureSwapper.Shock();
		}
	}

	public void Sad()
	{
		if (m_TextureSwapper != null)
		{
			m_TextureSwapper.Sad();
		}
	}

	protected override void OnDisposed()
	{
		this.OnSpecialAnimationComplete = null;
		this.OnGentPipeHit = null;
		m_Director = null;
		m_HeadTracker = null;
		m_TextureSwapper = null;
		base.OnAnimationComplete -= HandleExitOnAnimationComplete;
		base.OnAnimationEvent -= HandleOnAnimationEvent;
		base.OnAnimationEvent2 -= HandleOnAnimationEvent2;
		base.OnDisposed();
	}
}
