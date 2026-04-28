using System.Collections.Generic;
using UnityEngine;

public class PlayerContent : JMonoBehaviour
{
	[Header("Animation")]
	[SerializeField]
	private Animator m_Animator;

	[Space]
	[SerializeField]
	private AnimationClipGroup[] m_AnimationClipGroup;

	private AnimatorOverrideController m_AnimatorOverrideController;

	private AnimationClipOverrides m_AimationClipOverrides;

	public Animator Animator => m_Animator;

	public override void Awake()
	{
		if (m_Animator == null)
		{
			m_Animator = base.gameObject.GetComponent<Animator>();
		}
		SetAnimationOverrideController();
	}

	private void SetAnimationOverrideController()
	{
		m_AnimatorOverrideController = new AnimatorOverrideController(m_Animator.runtimeAnimatorController);
		m_Animator.runtimeAnimatorController = m_AnimatorOverrideController;
		m_AimationClipOverrides = new AnimationClipOverrides(m_AnimatorOverrideController.overridesCount);
		m_AnimatorOverrideController.GetOverrides(m_AimationClipOverrides);
		for (int i = 0; i < m_AnimationClipGroup.Length; i++)
		{
			m_AnimationClipGroup[i].Initialize();
		}
		List<AnimationClip> list = new List<AnimationClip>();
		for (int j = 0; j < m_AnimationClipGroup.Length; j++)
		{
			AnimationClipGroup animationClipGroup = m_AnimationClipGroup[j];
			list.Add(animationClipGroup.AnimationClip);
		}
		UpdateClipOverrides(list.ToArray());
	}

	public void UpdateClipOverrides(params AnimationClip[] clips)
	{
		if (!(m_AnimatorOverrideController == null) && m_AimationClipOverrides != null && clips != null && clips.Length != 0)
		{
			foreach (AnimationClip animationClip in clips)
			{
				string text = animationClip.name;
				m_AimationClipOverrides[text] = animationClip;
			}
			m_AnimatorOverrideController.ApplyOverrides(m_AimationClipOverrides);
		}
	}

	protected override void OnDisposed()
	{
		m_AnimatorOverrideController = null;
		m_AimationClipOverrides = null;
		base.OnDisposed();
	}
}
