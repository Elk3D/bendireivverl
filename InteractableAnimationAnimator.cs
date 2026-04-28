using System;
using UnityEngine;

[Serializable]
public class InteractableAnimationAnimator
{
	[SerializeField]
	public Animator Animator;

	[SerializeField]
	public AnimationClip AnimationClip;

	private AnimatorOverrideController m_AnimatorOverrideController;

	private AnimationClipOverrides m_AimationClipOverrides;

	public void Initialize()
	{
		if (!(Animator == null) && !(AnimationClip == null))
		{
			m_AnimatorOverrideController = new AnimatorOverrideController(Animator.runtimeAnimatorController);
			Animator.runtimeAnimatorController = m_AnimatorOverrideController;
			m_AimationClipOverrides = new AnimationClipOverrides(m_AnimatorOverrideController.overridesCount);
			m_AnimatorOverrideController.GetOverrides(m_AimationClipOverrides);
			if (!(m_AnimatorOverrideController == null) && m_AimationClipOverrides != null)
			{
				m_AnimatorOverrideController.ApplyOverrides(m_AimationClipOverrides);
			}
		}
	}

	public void Interact()
	{
		Animator.SetTrigger("Interact");
	}

	public void InteractEnter()
	{
		Animator.SetTrigger("InteractEnter");
	}

	public void InteractEixt()
	{
		Animator.SetTrigger("InteractExit");
	}
}
