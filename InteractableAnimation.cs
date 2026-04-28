using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class InteractableAnimation : JMonoBehaviour
{
	[Serializable]
	public class InteractableAnimator
	{
		[SerializeField]
		public string Trigger;

		[SerializeField]
		public Animator Animator;

		[SerializeField]
		public bool DisableOnComplete;
	}

	[Header("Player")]
	[SerializeField]
	private string m_AnimationTrigger;

	[SerializeField]
	private Interactable m_Interactable;

	[Header("Props")]
	[SerializeField]
	private InteractableAnimator[] m_InteractableEnterAnimators;

	[SerializeField]
	private InteractableAnimator[] m_InteractableExitAnimators;

	[Header("Positions")]
	[SerializeField]
	private Transform m_StartLocation;

	[SerializeField]
	private Transform m_EndLocation;

	[SerializeField]
	private Transform m_CancelLocation;

	[SerializeField]
	private Transform m_SlideLocation;

	[SerializeField]
	private bool m_UseDynamicLocation;

	[SerializeField]
	private float m_DynamicLocationDistance = 2.42f;

	[Header("Options")]
	[SerializeField]
	private bool m_EnableOnStart = true;

	[SerializeField]
	private bool m_IsSingleInteraction;

	[SerializeField]
	private bool m_IsSingleInteractionExit;

	[SerializeField]
	private string m_AnimationExitTrigger;

	[SerializeField]
	private bool m_DisableInteractionButtonExit;

	[SerializeField]
	private bool IgnoreSlideToLocation;

	[SerializeField]
	private bool SlideAndPlayImmediately;

	[SerializeField]
	private bool m_HideCrosshair;

	[SerializeField]
	private bool m_IsReal;

	[SerializeField]
	private bool m_EnableAnimationRotation = true;

	[SerializeField]
	private Vector2 m_RotationLock = new Vector2(15f, 20f);

	private bool m_IsInteracted;

	private Vector3 m_PlayerPosition;

	private Sequence m_Sequence;

	public bool CanCancel => m_CancelLocation != null;

	public bool HasStartLocation => m_StartLocation != null;

	public bool HasEndLocation => m_EndLocation != null;

	public bool IsLocked { get; private set; }

	public bool IsSingleInteraction => m_IsSingleInteraction;

	public bool HideCrosshair => m_HideCrosshair;

	public Interactable Interactable => m_Interactable;

	public Transform StartLocation => m_StartLocation;

	public event EventHandler OnInteracted;

	public event EventHandler OnEnter;

	public event EventHandler OnExit;

	public event EventHandler OnInteractionOnEnter;

	public event EventHandler OnInteractionOnExit;

	public override void Start()
	{
		SetActive(m_EnableOnStart);
		m_Interactable.OnEnter += HandleInteractableOnEnter;
		m_Interactable.OnExit += HandleInteractableOnExit;
	}

	private void HandleInteractableOnExit(object sender, EventArgs e)
	{
		this.OnInteractionOnExit.Send(this);
	}

	private void HandleInteractableOnEnter(object sender, EventArgs e)
	{
		this.OnInteractionOnEnter.Send(this);
	}

	private void Update()
	{
		if (!IsLocked && m_IsInteracted && (CanCancel || HasEndLocation) && !m_DisableInteractionButtonExit && !base.IsDisposed && !GameManager.Instance.IsPaused && m_IsInteracted && PlayerInput.InteractOnReleased())
		{
			if (CanCancel && PlayerInput.MoveY() > 0.1f)
			{
				Cancel();
			}
			else
			{
				Exit();
			}
		}
	}

	public void ResetAll()
	{
		m_IsInteracted = false;
		RemoveListeners();
		SetActive(m_EnableOnStart);
	}

	public void SetActive(bool active)
	{
		if (active)
		{
			AddListeners();
		}
		else
		{
			RemoveListeners();
		}
		m_Interactable.SetActive(active);
	}

	public void SetLocked(bool isLocked)
	{
		IsLocked = isLocked;
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		RemoveListeners();
		Enter();
	}

	public void Enter()
	{
		if (m_UseDynamicLocation)
		{
			Vector3 vector = GameManager.Instance.Player.transform.position - base.transform.position;
			Vector3 position = base.transform.position + vector.normalized * m_DynamicLocationDistance;
			position.y = GameManager.Instance.Player.transform.position.y;
			m_StartLocation.position = position;
			Vector3 position2 = base.transform.position;
			position2.y = position.y;
			m_StartLocation.LookAt(position2);
			if (GetComponent<Rigidbody>() != null)
			{
				GetComponent<Rigidbody>().isKinematic = true;
			}
		}
		if (m_HideCrosshair)
		{
			GameManager.Instance.HideCrosshair();
		}
		if (HasStartLocation && IgnoreSlideToLocation)
		{
			if (m_SlideLocation == null)
			{
				GameManager.Instance.Player.HeadContainer.SetParent(null);
			}
			else
			{
				GameManager.Instance.Player.SetState(State.Player.Cutscene);
				Vector3 vector2 = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
				ResetSequence();
				m_Sequence.SetUpdate(UpdateType.Fixed);
				m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(m_SlideLocation.eulerAngles, 0.25f).SetEase(Ease.InOutSine));
				m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(m_SlideLocation.position + vector2, 0.25f).SetEase(Ease.InOutSine));
				if (SlideAndPlayImmediately)
				{
					GameManager.Instance.Player.OnAnimationComplete -= HandleEnterOnAnimationComplete;
					GameManager.Instance.Player.OnAnimationComplete += HandleEnterOnAnimationComplete;
					GameManager.Instance.Player.EnterInteraction(m_AnimationTrigger);
					for (int i = 0; i < m_InteractableEnterAnimators.Length; i++)
					{
						InteractableAnimator interactableAnimator = m_InteractableEnterAnimators[i];
						interactableAnimator.Animator.gameObject.SetActive(value: true);
						interactableAnimator.Animator.SetTrigger(interactableAnimator.Trigger);
					}
				}
				else
				{
					m_Sequence.Insert(0f, GameManager.Instance.Player.HeadContainer.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.InOutSine));
					m_Sequence.OnComplete(delegate
					{
						GameManager.Instance.Player.OnAnimationComplete -= HandleEnterOnAnimationComplete;
						GameManager.Instance.Player.OnAnimationComplete += HandleEnterOnAnimationComplete;
						GameManager.Instance.Player.EnterInteractionInstant(m_AnimationTrigger);
						for (int j = 0; j < m_InteractableEnterAnimators.Length; j++)
						{
							InteractableAnimator interactableAnimator3 = m_InteractableEnterAnimators[j];
							interactableAnimator3.Animator.gameObject.SetActive(value: true);
							interactableAnimator3.Animator.SetTrigger(interactableAnimator3.Trigger);
						}
						GameManager.Instance.Player.transform.position = m_StartLocation.position;
						GameManager.Instance.Player.transform.eulerAngles = m_StartLocation.eulerAngles;
					});
				}
			}
		}
		if (m_SlideLocation == null)
		{
			GameManager.Instance.Player.OnAnimationComplete -= HandleEnterOnAnimationComplete;
			GameManager.Instance.Player.OnAnimationComplete += HandleEnterOnAnimationComplete;
			GameManager.Instance.Player.EnterInteraction(m_AnimationTrigger);
			for (int num = 0; num < m_InteractableEnterAnimators.Length; num++)
			{
				InteractableAnimator interactableAnimator2 = m_InteractableEnterAnimators[num];
				interactableAnimator2.Animator.gameObject.SetActive(value: true);
				interactableAnimator2.Animator.SetTrigger(interactableAnimator2.Trigger);
			}
		}
		if (HasStartLocation)
		{
			if (!IgnoreSlideToLocation)
			{
				Vector3 vector3 = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
				ResetSequence();
				m_Sequence.SetUpdate(UpdateType.Fixed);
				m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(m_StartLocation.eulerAngles, 0.25f).SetEase(Ease.InOutSine));
				m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(m_StartLocation.position + vector3, 0.25f).SetEase(Ease.InOutSine)).OnComplete(delegate
				{
					GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
				});
			}
			else
			{
				GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
				if (m_SlideLocation == null)
				{
					GameManager.Instance.Player.transform.position = m_StartLocation.position;
					GameManager.Instance.Player.transform.eulerAngles = m_StartLocation.eulerAngles;
				}
			}
		}
		this.OnInteracted.Send(this);
	}

	public void ForceEnter(Quaternion rotationX, Quaternion rotationY, int ladderSide, Action callback = null, bool isLadder = true)
	{
		StartCoroutine(InternalForceEnter(rotationX, rotationY, ladderSide, callback, isLadder));
	}

	private IEnumerator InternalForceEnter(Quaternion rotationX, Quaternion rotationY, int ladderSide, Action callback, bool isLadder)
	{
		while (GameManager.Instance.Player == null || !GameManager.Instance.Player.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		RemoveListeners();
		if (m_HideCrosshair)
		{
			GameManager.Instance.HideCrosshair();
		}
		GameManager.Instance.Player.Interaction.ResetInteraction();
		GameManager.Instance.Player.HideFirstPersonArms();
		m_IsInteracted = true;
		if (isLadder)
		{
			GameManager.Instance.Player.AnimatorBody.SetInteger("LadderSide", ladderSide);
		}
		GameManager.Instance.Player.EnterInteractionInstant(m_AnimationTrigger + "Instant");
		Debug.Log("EnterInteractionInstant: " + m_AnimationTrigger + "Instant");
		for (int i = 0; i < m_InteractableEnterAnimators.Length; i++)
		{
			InteractableAnimator interactableAnimator = m_InteractableEnterAnimators[i];
			if (interactableAnimator.DisableOnComplete)
			{
				interactableAnimator.Animator.gameObject.SetActive(value: false);
			}
		}
		if (m_EnableAnimationRotation)
		{
			GameManager.Instance.Player.EnableAnimationRotation(m_RotationLock.x, m_RotationLock.y);
		}
		GameManager.Instance.Player.ForceRotation(rotationX, rotationY);
		if (!CanCancel && !HasEndLocation)
		{
			if (!m_IsSingleInteraction)
			{
				AddListeners();
			}
		}
		else
		{
			m_IsInteracted = true;
		}
		if (m_IsSingleInteraction)
		{
			m_Interactable.SetActive(active: false);
		}
		callback?.Invoke();
	}

	private void HandleEnterOnAnimationComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnAnimationComplete -= HandleEnterOnAnimationComplete;
		for (int i = 0; i < m_InteractableEnterAnimators.Length; i++)
		{
			InteractableAnimator interactableAnimator = m_InteractableEnterAnimators[i];
			if (interactableAnimator.DisableOnComplete)
			{
				interactableAnimator.Animator.gameObject.SetActive(value: false);
			}
		}
		if (m_EnableAnimationRotation)
		{
			GameManager.Instance.Player.EnableAnimationRotation(m_RotationLock.x, m_RotationLock.y);
		}
		if (!CanCancel && !HasEndLocation)
		{
			if (!m_IsSingleInteraction)
			{
				AddListeners();
			}
		}
		else
		{
			m_IsInteracted = true;
		}
		this.OnEnter.Send(this);
		if (m_IsSingleInteraction)
		{
			m_Interactable.SetActive(active: false);
		}
		if (m_IsSingleInteractionExit)
		{
			m_IsInteracted = false;
			GameManager.Instance.Player.ExitAnimation();
			GameManager.Instance.Player.SetBodyAnimationTrigger(m_AnimationExitTrigger);
			if (!m_IsSingleInteraction)
			{
				AddListeners();
			}
			if (m_HideCrosshair)
			{
				GameManager.Instance.ShowCrosshair(m_IsReal);
			}
			if (HasEndLocation && m_AnimationExitTrigger == "")
			{
				Vector3 vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
				GameManager.Instance.Player.transform.position = m_EndLocation.position + vector;
				GameManager.Instance.Player.transform.eulerAngles = m_EndLocation.eulerAngles;
			}
			this.OnExit.Send(this);
		}
	}

	private void Cancel()
	{
		Exit();
		Vector3 vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
		m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(m_CancelLocation.eulerAngles, 0.1f).SetEase(Ease.Linear));
		m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(m_CancelLocation.position + vector, 0.1f).SetEase(Ease.Linear));
	}

	public void Exit()
	{
		m_IsInteracted = false;
		for (int i = 0; i < m_InteractableExitAnimators.Length; i++)
		{
			m_InteractableExitAnimators[i].Animator.SetTrigger(m_InteractableExitAnimators[i].Trigger);
		}
		GameManager.Instance.Player.OnAnimationComplete -= HandleExitOnAnimationComplete;
		GameManager.Instance.Player.OnAnimationComplete += HandleExitOnAnimationComplete;
		GameManager.Instance.Player.ExitInteraction();
		if (m_EnableAnimationRotation)
		{
			ResetSequence();
			m_Sequence.Insert(0f, GameManager.Instance.Player.AnimationContainer.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.InOutSine));
		}
	}

	private void HandleExitOnAnimationComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnAnimationComplete -= HandleExitOnAnimationComplete;
		if (HasEndLocation)
		{
			Vector3 vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
			GameManager.Instance.Player.transform.position = m_EndLocation.position + vector;
			GameManager.Instance.Player.transform.eulerAngles = m_EndLocation.eulerAngles;
		}
		if (!m_IsSingleInteraction)
		{
			m_Interactable.ResetAction();
			SetActive(active: true);
		}
		this.OnExit.Send(this);
		if (m_HideCrosshair)
		{
			GameManager.Instance.ShowCrosshair(m_IsReal);
		}
		GameManager.Instance.Player.ExitAnimation();
	}

	public void SendOnExit()
	{
		this.OnExit.Send(this);
	}

	public void Stop()
	{
		m_IsInteracted = false;
		KillSequence();
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnAnimationComplete -= HandleEnterOnAnimationComplete;
			GameManager.Instance.Player.OnAnimationComplete -= HandleExitOnAnimationComplete;
		}
	}

	private void AddListeners()
	{
		if (m_Interactable != null)
		{
			m_Interactable.ResetAction();
			m_Interactable.OnInteract -= HandleInteractableOnInteract;
			m_Interactable.OnInteract += HandleInteractableOnInteract;
		}
	}

	private void RemoveListeners()
	{
		if (m_Interactable != null)
		{
			m_Interactable.OnInteract -= HandleInteractableOnInteract;
		}
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		m_Sequence?.Kill();
		m_Sequence = null;
	}

	protected override void OnDisposed()
	{
		m_Interactable.OnEnter -= HandleInteractableOnEnter;
		m_Interactable.OnExit -= HandleInteractableOnExit;
		RemoveListeners();
		KillSequence();
		base.OnDisposed();
	}
}
