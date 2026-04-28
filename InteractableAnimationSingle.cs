using System;
using DG.Tweening;
using UnityEngine;

public class InteractableAnimationSingle : InteractableAnimationBase
{
	private const string INTERACT = "Interact";

	private const string INTERACT_INSTANT = "InteractInstant";

	[Header("Animation Clip")]
	[SerializeField]
	private AnimationClip m_InteractClip;

	[Header("Animation Props")]
	[SerializeField]
	private bool m_InstantInteract;

	[SerializeField]
	private Animator[] m_Animators;

	public Animator[] Animators => m_Animators;

	public void UpdateInteractClip(AnimationClip animationClip)
	{
		m_InteractClip = animationClip;
	}

	public override void Enter()
	{
		CheckCrosshair(show: false);
		Vector3 vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
		ResetSequence();
		m_Sequence.SetUpdate(UpdateType.Fixed);
		m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(m_StartLocation.eulerAngles, 0.25f).SetEase(Ease.InOutSine));
		m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(m_StartLocation.position + vector, 0.25f).SetEase(Ease.InOutSine));
		RemoveListeners();
		m_InteractClip.name = "Interact";
		GameManager.Instance.Player.UpdatePlayerContentClipOverrides(m_InteractClip);
		GameManager.Instance.Player.OnAnimationComplete += HandleEnterOnAnimationComplete;
		GameManager.Instance.Player.EnterInteraction("Interact");
		for (int i = 0; i < m_Animators.Length; i++)
		{
			m_Animators[i].SetTrigger(m_InstantInteract ? "InteractInstant" : "Interact");
		}
	}

	private void HandleEnterOnAnimationComplete(object sender, EventArgs e)
	{
		RemoveListeners();
		SendToEndLocation();
		if (!m_IsSingleInteraction)
		{
			ResetAction();
			SetActive(active: true);
		}
		CheckCrosshair(show: true);
		GameManager.Instance.Player.ExitAnimation();
		SendOnInteractionComplete();
	}

	private void RemoveListeners()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnAnimationComplete -= HandleEnterOnAnimationComplete;
		}
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		base.OnDisposed();
	}
}
