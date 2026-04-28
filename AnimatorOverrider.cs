using UnityEngine;

public class AnimatorOverrider : JMonoBehaviour
{
	[Header("Animation Animator")]
	[SerializeField]
	private Animator m_Animator;

	[Header("Animation Events")]
	[Rename("Events")]
	[SerializeField]
	private GenericAnimationEvents m_GenericAnimationEvents;

	[Header("Animation Clips")]
	[SerializeField]
	private AnimationClipGroup[] m_AnimationClipGroup;

	private AnimatorOverrideController m_AnimatorOverrideController;

	private AnimationClipOverrides m_AimationClipOverrides;

	public Animator Animator => m_Animator;

	public GenericAnimationEvents GenericAnimationEvents => m_GenericAnimationEvents;

	public override void Awake()
	{
		SetAnimator();
		SetGenericAnimatorEvents();
		UpdateAnimationOverrideController();
		UpdateClipOverrides(m_AnimationClipGroup);
	}

	private void SetAnimator()
	{
		if (m_Animator == null)
		{
			m_Animator = base.gameObject.GetComponent<Animator>();
		}
		if (m_Animator != null)
		{
			m_Animator.applyRootMotion = false;
			m_Animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		}
	}

	private void SetGenericAnimatorEvents()
	{
		if (m_GenericAnimationEvents == null)
		{
			m_GenericAnimationEvents = base.gameObject.GetComponent<GenericAnimationEvents>();
		}
	}

	public void UpdateAnimatorController(RuntimeAnimatorController animatorController)
	{
		m_Animator.runtimeAnimatorController = animatorController;
	}

	public void UpdateAnimationOverrideController()
	{
		m_AnimatorOverrideController = new AnimatorOverrideController(Animator.runtimeAnimatorController);
		Animator.runtimeAnimatorController = m_AnimatorOverrideController;
		m_AimationClipOverrides = new AnimationClipOverrides(m_AnimatorOverrideController.overridesCount);
		m_AnimatorOverrideController.GetOverrides(m_AimationClipOverrides);
	}

	public void UpdateClipOverrides(params AnimationClipGroup[] clipGroups)
	{
		if (!(m_AnimatorOverrideController == null) && m_AimationClipOverrides != null && clipGroups != null && clipGroups.Length != 0)
		{
			foreach (AnimationClipGroup animationClipGroup in clipGroups)
			{
				m_AimationClipOverrides[animationClipGroup.Name] = animationClipGroup.AnimationClip;
			}
			ApplyOverrides();
		}
	}

	public void UpdateClipOverride(string _name, AnimationClip clip)
	{
		if (!(m_AnimatorOverrideController == null) && m_AimationClipOverrides != null && !(clip == null) && !(_name == ""))
		{
			m_AimationClipOverrides[_name] = clip;
		}
	}

	public void ApplyOverrides()
	{
		m_AnimatorOverrideController.ApplyOverrides(m_AimationClipOverrides);
	}

	protected override void OnDisposed()
	{
		m_AnimatorOverrideController = null;
		m_AimationClipOverrides = null;
		base.OnDisposed();
	}
}
