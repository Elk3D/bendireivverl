using System;
using DG.Tweening;
using UnityEngine;

public class AnimationEnter : AnimationBase
{
	private const string INTERACT_ENTER = "InteractEnter";

	private const string INTERACT_ENTER_INSTANT = "InteractEnterInstant";

	private const string INTERACT_LOOP = "InteractLoop";

	private const string INTERACT_EXIT = "InteractExit";

	private const string INTERACT_EXIT_FORCE_COMPLETE = "InteractExitForceComplete";

	[SerializeField]
	private bool m_SkipEnter;

	[SerializeField]
	private bool m_EnableAnimationRotation = true;

	[SerializeField]
	private Vector2 m_RotationLock = new Vector2(15f, 20f);

	[Header("Input Exit")]
	[SerializeField]
	private bool m_InputExit = true;

	[SerializeField]
	private InteractionType m_InteractionType;

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

	private Sequence m_HeadRotationSequence;

	private bool m_IsInteracted;

	private bool m_IsLocked;

	private void Update()
	{
		if (!m_IsLocked && m_InputExit && m_IsInteracted && !base.IsDisposed && !GameManager.Instance.IsPaused && m_IsInteracted && PlayerInput.InteractOnReleased())
		{
			Exit();
		}
	}

	public override void Enter()
	{
		if (m_HideCrosshair)
		{
			GameManager.Instance.HideCrosshair();
		}
		if (m_InputExit)
		{
			string text = TextUtility.GetKey(m_InteractionType.ToString());
			if (m_IsReal)
			{
				text = text.ToUpper();
			}
			GameManager.Instance.ShowInteraction(text, m_IsReal);
		}
		GameManager.Instance.Player.HideFirstPersonArms();
		if (!m_SkipEnter)
		{
			m_InteractEnterClip.name = "InteractEnter";
		}
		m_InteractLoopClip.name = "InteractLoop";
		m_InteractExitClip.name = "InteractExit";
		RemoveListeners();
		if (!m_SkipEnter)
		{
			GameManager.Instance.Player.UpdatePlayerContentClipOverrides(m_InteractEnterClip, m_InteractLoopClip, m_InteractExitClip);
			GameManager.Instance.Player.OnAnimationComplete += HandleEnterOnAnimationComplete;
			GameManager.Instance.Player.EnterInteraction("InteractEnter");
			for (int i = 0; i < m_Animators.Length; i++)
			{
				m_Animators[i].SetTrigger("InteractEnter");
			}
		}
		else
		{
			GameManager.Instance.Player.UpdatePlayerContentClipOverrides(m_InteractLoopClip, m_InteractExitClip);
			GameManager.Instance.Player.EnterInteractionInstant("InteractEnterInstant");
			for (int j = 0; j < m_Animators.Length; j++)
			{
				m_Animators[j].SetTrigger("InteractEnterInstant");
			}
		}
		Vector3 vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
		ResetSequence();
		m_Sequence.SetUpdate(UpdateType.Fixed);
		if (!m_SkipEnter)
		{
			m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(m_StartLocation.eulerAngles, 0.25f).SetEase(Ease.InOutSine));
			m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(m_StartLocation.position + vector, 0.25f).SetEase(Ease.InOutSine));
			return;
		}
		GameManager.Instance.Player.transform.position = m_StartLocation.position + vector;
		GameManager.Instance.Player.transform.eulerAngles = m_StartLocation.eulerAngles;
		m_Sequence.InsertCallback(0.1f, delegate
		{
			m_IsInteracted = true;
			if (m_EnableAnimationRotation)
			{
				GameManager.Instance.Player.EnableAnimationRotation(m_RotationLock.x, m_RotationLock.y);
			}
			SendOnComplete();
		});
	}

	private void HandleEnterOnAnimationComplete(object sender, EventArgs e)
	{
		if (!base.IsDisposed)
		{
			RemoveListeners();
			m_IsInteracted = true;
			if (m_EnableAnimationRotation)
			{
				GameManager.Instance.Player.EnableAnimationRotation(m_RotationLock.x, m_RotationLock.y);
			}
			SendOnComplete();
		}
	}

	public override void Exit()
	{
		m_IsInteracted = false;
		if (m_InputExit)
		{
			GameManager.Instance.Player.Interaction.ResetInteraction();
			GameManager.Instance.ClearInteraction();
		}
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
			m_Sequence.Insert(0f, GameManager.Instance.Player.AnimationContainer.DOLocalRotate(Vector3.zero, 0.1f).SetEase(Ease.InOutSine));
		}
	}

	public void ForceExit()
	{
		for (int i = 0; i < m_Animators.Length; i++)
		{
			m_Animators[i].SetTrigger("InteractExitForceComplete");
		}
	}

	private void HandleExitOnAnimationComplete(object sender, EventArgs e)
	{
		if (!base.IsDisposed)
		{
			RemoveListeners();
			if (base.HasEndLocation)
			{
				Vector3 vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
				GameManager.Instance.Player.transform.position = m_EndLocation.position + vector;
				GameManager.Instance.Player.transform.eulerAngles = m_EndLocation.eulerAngles;
			}
			if (m_HideCrosshair)
			{
				GameManager.Instance.ShowCrosshair(m_IsReal);
			}
			GameManager.Instance.Player.ExitAnimation();
			SendOnExit();
		}
	}

	public void ResetCameraRotation()
	{
		KillHeadRotationSequence();
		m_HeadRotationSequence = DOTween.Sequence();
		m_HeadRotationSequence.Insert(0f, GameManager.Instance.Player.HeadContainer.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.InOutSine));
		m_HeadRotationSequence.Insert(0f, GameManager.Instance.Player.AnimationContainer.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.InOutSine));
	}

	public void SetLock(bool isLocked)
	{
		m_IsLocked = isLocked;
	}

	public void ResetInteraction()
	{
		RemoveListeners();
		m_IsInteracted = false;
	}

	private void KillHeadRotationSequence()
	{
		if (m_HeadRotationSequence != null)
		{
			m_HeadRotationSequence.Kill();
			m_HeadRotationSequence = null;
		}
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
		KillHeadRotationSequence();
		base.OnDisposed();
	}
}
