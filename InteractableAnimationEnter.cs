using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class InteractableAnimationEnter : InteractableAnimationBase
{
	private const string INTERACT_ENTER = "InteractEnter";

	private const string INTERACT_ENTER_INSTANT = "InteractEnterInstant";

	private const string INTERACT_LOOP = "InteractLoop";

	private const string INTERACT_EXIT = "InteractExit";

	[SerializeField]
	private bool m_EndLocationOnIdle;

	[SerializeField]
	private bool m_InputExit = true;

	[SerializeField]
	private bool m_EndCrouched;

	[SerializeField]
	private bool m_EnableAnimationRotation = true;

	[SerializeField]
	private bool m_IsCutscene;

	[SerializeField]
	private Vector2 m_RotationLock = new Vector2(15f, 20f);

	[Header("Animation Clip")]
	[SerializeField]
	private AnimationClip m_InteractEnterClip;

	[SerializeField]
	private AnimationClip m_InteractLoopClip;

	[SerializeField]
	private AnimationClip m_InteractExitClip;

	[Header("Animation Props")]
	[SerializeField]
	private Animator[] m_Animators;

	private bool m_IsInteracted;

	private bool m_IsLocked;

	public bool IsInteracted => m_IsInteracted;

	private void Update()
	{
		if (!m_IsLocked && m_InputExit && m_IsInteracted && !base.IsDisposed && !GameManager.Instance.IsPaused && m_IsInteracted && PlayerInput.InteractOnReleased())
		{
			Exit();
		}
	}

	public override void Enter()
	{
		CheckCrosshair(show: false);
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.DisableTeleport();
		}
		Vector3 vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
		ResetSequence();
		m_Sequence.SetUpdate(UpdateType.Fixed);
		m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(m_StartLocation.eulerAngles, 0.25f).SetEase(Ease.InOutSine));
		m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(m_StartLocation.position + vector, 0.25f).SetEase(Ease.InOutSine));
		RemoveListeners();
		m_InteractEnterClip.name = "InteractEnter";
		m_InteractLoopClip.name = "InteractLoop";
		m_InteractExitClip.name = "InteractExit";
		GameManager.Instance.Player.UpdatePlayerContentClipOverrides(m_InteractEnterClip, m_InteractLoopClip, m_InteractExitClip);
		GameManager.Instance.Player.OnAnimationComplete += HandleEnterOnAnimationComplete;
		GameManager.Instance.Player.EnterInteraction("InteractEnter");
		for (int i = 0; i < m_Animators.Length; i++)
		{
			m_Animators[i].SetTrigger("InteractEnter");
		}
	}

	private void HandleEnterOnAnimationComplete(object sender, EventArgs e)
	{
		if (!base.IsDisposed)
		{
			RemoveListeners();
			m_IsInteracted = true;
			if (m_EndLocationOnIdle && base.HasEndLocation)
			{
				SendToEndLocation();
			}
			if (m_EnableAnimationRotation)
			{
				GameManager.Instance.Player.EnableAnimationRotation(m_RotationLock.x, m_RotationLock.y, m_IsCutscene);
			}
			SendOnInteractionComplete();
		}
	}

	public override void ForceEnter(Quaternion rotationX, Quaternion rotationY)
	{
		StartCoroutine(InternalForceEnter(rotationX, rotationY));
	}

	private IEnumerator InternalForceEnter(Quaternion rotationX, Quaternion rotationY)
	{
		while (GameManager.Instance.Player == null || !GameManager.Instance.Player.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		SendOnInteract(this);
		CheckCrosshair(show: false);
		GameManager.Instance.Player.Interaction.ResetInteraction();
		GameManager.Instance.Player.HideFirstPersonArms();
		m_InteractLoopClip.name = "InteractLoop";
		m_InteractExitClip.name = "InteractExit";
		RemoveListeners();
		m_IsInteracted = true;
		GameManager.Instance.Player.UpdatePlayerContentClipOverrides(m_InteractLoopClip, m_InteractExitClip);
		GameManager.Instance.Player.EnterInteractionInstant("InteractEnterInstant");
		for (int i = 0; i < m_Animators.Length; i++)
		{
			m_Animators[i].SetTrigger("InteractEnterInstant");
		}
		Vector3 vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
		GameManager.Instance.Player.transform.position = m_StartLocation.position + vector;
		GameManager.Instance.Player.transform.eulerAngles = m_StartLocation.eulerAngles;
		if (m_EnableAnimationRotation)
		{
			GameManager.Instance.Player.EnableAnimationRotation(m_RotationLock.x, m_RotationLock.y, m_IsCutscene);
		}
		GameManager.Instance.Player.ForceRotation(rotationX, rotationY);
		SendOnInteractionComplete();
	}

	public override void Exit()
	{
		if (m_EndCrouched)
		{
			GameManager.Instance.Player.ForceCrouch(isInternalCrouch: false);
		}
		else
		{
			GameManager.Instance.Player.ForceStand(isOutOfCombat: false);
		}
		m_IsInteracted = false;
		for (int i = 0; i < m_Animators.Length; i++)
		{
			m_Animators[i].SetTrigger("InteractExit");
		}
		RemoveListeners();
		GameManager.Instance.Player.OnAnimationComplete += HandleExitOnAnimationComplete;
		GameManager.Instance.Player.ExitInteraction();
		if (m_EnableAnimationRotation)
		{
			ResetSequence();
			m_Sequence.Insert(0f, GameManager.Instance.Player.AnimationContainer.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.InOutSine));
		}
		if (m_presetRumble != ControllerRumble.RUMBLE_PRESETS.NONE)
		{
			GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[(int)m_presetRumble]);
		}
		else
		{
			GameManager.Instance.TriggerRumble(m_CustomRumble);
		}
	}

	private void HandleExitOnAnimationComplete(object sender, EventArgs e)
	{
		if (!base.IsDisposed)
		{
			RemoveListeners();
			SendToEndLocation();
			if (!m_IsSingleInteraction)
			{
				ResetAction();
				SetActive(active: true);
			}
			GameManager.Instance.Player.ExitAnimation();
			CheckCrosshair(show: true);
			if (GameManager.Instance.Player.HasTeleport())
			{
				GameManager.Instance.EnableTeleport();
			}
			SendOnInteractionExit();
		}
	}

	public void SetLock(bool isLocked)
	{
		m_IsLocked = isLocked;
	}

	public void ResetInteraction()
	{
		RemoveListeners();
		m_IsInteracted = true;
	}

	private void RemoveListeners()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnAnimationComplete -= HandleEnterOnAnimationComplete;
			GameManager.Instance.Player.OnAnimationComplete -= HandleExitOnAnimationComplete;
		}
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		base.OnDisposed();
	}
}
