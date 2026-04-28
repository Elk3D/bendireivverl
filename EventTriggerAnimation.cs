using System;
using DG.Tweening;
using UnityEngine;

public class EventTriggerAnimation : JMonoBehaviour
{
	[Serializable]
	public class EventTriggerAnimator
	{
		[SerializeField]
		public string Trigger;

		[SerializeField]
		public Animator Animator;
	}

	[SerializeField]
	private string m_AnimationTrigger;

	[SerializeField]
	private LayerMask m_IgnoreLayer;

	[Header("Props")]
	[SerializeField]
	private EventTriggerAnimator[] m_EventTriggerAnimators;

	[Header("Locations")]
	[SerializeField]
	private Transform m_StartLocation;

	[SerializeField]
	private Transform m_EndLocation;

	[SerializeField]
	private bool m_CanOffsetLocations = true;

	[Header("Triggers")]
	[SerializeField]
	private EventTrigger m_EventTrigger;

	[SerializeField]
	private Collider m_TargetCollider;

	[Header("Other Options")]
	[SerializeField]
	private bool m_HideCrosshair;

	[SerializeField]
	private bool m_IsReal;

	[SerializeField]
	private bool m_SnapOnComplete;

	private Vector3 m_InitialStartLocation;

	private Vector3 m_InitialEndLocation;

	private Sequence m_Sequence;

	private Transform m_Camera => GameManager.Instance.GameCamera.Camera.transform;

	public bool HasStartLocation => m_StartLocation != null;

	public bool HasEndLocation => m_EndLocation != null;

	public event EventHandler OnTriggered;

	public override void Start()
	{
		SetInitialLocations();
		AddListeners();
	}

	private void HandleTriggerEventOnEnter(object sender, EventArgs e)
	{
		if (!GameManager.Instance.Player.IsGrounded)
		{
			GameManager.Instance.Player.ResetAnimationTrigger("Jump");
		}
		RemoveListeners();
		if (m_TargetCollider != null)
		{
			Physics.Raycast(m_Camera.position, m_Camera.forward, out var hitInfo, 10f, ~(int)m_IgnoreLayer);
			if (hitInfo.transform != m_TargetCollider.transform)
			{
				ResetInitialLocations();
				m_EventTrigger.ResetAction();
				AddListeners();
				return;
			}
		}
		if (m_HideCrosshair)
		{
			GameManager.Instance.HideCrosshair();
		}
		if (m_CanOffsetLocations)
		{
			CheckStartLocation(m_Camera);
		}
		GameManager.Instance.Player.OnAnimationComplete -= HandleOnAnimationComplete;
		GameManager.Instance.Player.OnAnimationComplete += HandleOnAnimationComplete;
		GameManager.Instance.Player.EnterInteraction(m_AnimationTrigger);
		for (int i = 0; i < m_EventTriggerAnimators.Length; i++)
		{
			EventTriggerAnimator eventTriggerAnimator = m_EventTriggerAnimators[i];
			eventTriggerAnimator.Animator.SetTrigger(eventTriggerAnimator.Trigger);
		}
		if (HasStartLocation)
		{
			ResetSequence();
			m_Sequence.SetUpdate(UpdateType.Fixed);
			m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(m_StartLocation.eulerAngles, 0.1f).SetEase(Ease.InOutSine));
			m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(m_StartLocation.position, 0.1f).SetEase(Ease.InOutSine));
			m_Sequence.OnComplete(delegate
			{
				GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
			});
		}
		else
		{
			GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
		}
		this.OnTriggered.Send(this);
	}

	private void HandleOnAnimationComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnAnimationComplete -= HandleOnAnimationComplete;
		if (HasEndLocation)
		{
			Vector3 vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
			GameManager.Instance.Player.transform.position = m_EndLocation.position + vector;
			GameManager.Instance.Player.transform.eulerAngles = m_EndLocation.eulerAngles;
		}
		ResetInitialLocations();
		m_EventTrigger.ResetAction();
		AddListeners();
		if (m_HideCrosshair)
		{
			GameManager.Instance.ShowCrosshair(m_IsReal);
		}
		GameManager.Instance.Player.UnlockRotation();
		GameManager.Instance.Player.ExitAnimation();
	}

	private void SetInitialLocations()
	{
		if (HasStartLocation)
		{
			m_InitialStartLocation = m_StartLocation.localPosition;
		}
		if (HasEndLocation)
		{
			m_InitialEndLocation = m_EndLocation.localPosition;
		}
	}

	private void ResetInitialLocations()
	{
		if (HasStartLocation)
		{
			m_StartLocation.localPosition = m_InitialStartLocation;
		}
		if (HasEndLocation)
		{
			m_EndLocation.localPosition = m_InitialEndLocation;
		}
	}

	private void CheckStartLocation(Transform cameraTransform)
	{
		float x = 0f - cameraTransform.InverseTransformPoint(m_StartLocation.position).x;
		if (HasStartLocation)
		{
			Vector3 localPosition = m_StartLocation.localPosition;
			localPosition.x = x;
			m_StartLocation.localPosition = localPosition;
		}
		if (HasEndLocation)
		{
			Vector3 localPosition2 = m_EndLocation.localPosition;
			localPosition2.x = x;
			m_EndLocation.localPosition = localPosition2;
		}
	}

	private void AddListeners()
	{
		m_EventTrigger.OnEnter += HandleTriggerEventOnEnter;
	}

	private void RemoveListeners()
	{
		m_EventTrigger.OnEnter -= HandleTriggerEventOnEnter;
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
		RemoveListeners();
		KillSequence();
		base.OnDisposed();
	}
}
