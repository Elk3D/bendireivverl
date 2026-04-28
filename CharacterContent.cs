using System.Collections.Generic;
using S13Audio;
using UnityEngine;

public class CharacterContent : JMonoBehaviour
{
	[Header("Character Vision")]
	[SerializeField]
	private Transform m_EyeSight;

	[Header("Character Model")]
	[SerializeField]
	private GameObject m_Model;

	[Header("Animator")]
	[SerializeField]
	private Animator m_Animator;

	[Header("Animation Events")]
	[Rename("Events")]
	[SerializeField]
	private GenericAnimationEvents m_GenericAnimationEvents;

	[Header("Animation Audio")]
	[SerializeField]
	private CharacterAudioSelector m_CharacterAudio;

	[Header("Animations")]
	[SerializeField]
	private AnimationClipGroup[] m_AnimationClipGroup;

	[Header("Initialize Settings")]
	[SerializeField]
	private bool m_OnAwake = true;

	private AnimatorOverrideController m_AnimatorOverrideController;

	private AnimationClipOverrides m_AimationClipOverrides;

	private RagdollController m_RagdollController;

	private bool m_IsInitialized;

	public Transform EyeSight => m_EyeSight;

	public GameObject Model => m_Model;

	public Animator Animator => m_Animator;

	public GenericAnimationEvents GenericAnimationEvents => m_GenericAnimationEvents;

	public Renderer[] ModelRenderers { get; private set; }

	public RagdollController RagdollController => m_RagdollController;

	public override void Awake()
	{
		if (m_OnAwake)
		{
			Initialize();
		}
	}

	public void Initialize()
	{
		if (m_IsInitialized)
		{
			return;
		}
		if (m_CharacterAudio != null)
		{
			S13AnimationPassthrough component = base.gameObject.GetComponent<S13AnimationPassthrough>();
			if (component != null)
			{
				GameObject gameObject = m_CharacterAudio.Get();
				if (gameObject != null)
				{
					gameObject.transform.SetParent(base.transform);
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localEulerAngles = Vector3.zero;
					S13AnimationActions[] componentsInChildren = gameObject.GetComponentsInChildren<S13AnimationActions>();
					component.RegisterAnimationActions(componentsInChildren);
				}
			}
		}
		SetAnimationOverrideController();
		SoftInitialize();
	}

	public void SoftInitialize()
	{
		if (m_Model == null)
		{
			Transform transform = base.transform.FindChildName("TPose");
			if (transform == null)
			{
				transform = base.transform.FindChildName("Tpose");
			}
			if (transform != null)
			{
				m_Model = transform.gameObject;
			}
		}
		if (m_Model != null)
		{
			ModelRenderers = m_Model.transform.GetComponentsInChildren<Renderer>();
		}
		for (int i = 0; i < ModelRenderers.Length; i++)
		{
			Renderer obj = ModelRenderers[i];
			if (obj is SkinnedMeshRenderer skinnedMeshRenderer)
			{
				skinnedMeshRenderer.updateWhenOffscreen = true;
			}
			obj.material.SetFloat("_PostHitGlow", 0.3f);
		}
		if (m_Animator == null)
		{
			m_Animator = base.gameObject.GetComponent<Animator>();
		}
		if (m_GenericAnimationEvents == null)
		{
			m_GenericAnimationEvents = base.gameObject.GetComponent<GenericAnimationEvents>();
		}
		if (m_RagdollController == null)
		{
			m_RagdollController = GetComponentInChildren<RagdollController>();
		}
		m_IsInitialized = true;
	}

	public void SetAnimationTrigger(string trigger)
	{
		m_Animator?.SetTrigger(trigger);
	}

	public void Attack()
	{
		m_Animator?.Attack();
	}

	public void AttackLunge()
	{
		m_Animator?.AttackLunge();
	}

	public void AttackRanged()
	{
		m_Animator?.AttackRanged();
	}

	public void AttackBehind()
	{
		m_Animator?.SetTrigger("AttackBehind");
	}

	public void Jump()
	{
		m_Animator?.Jump();
	}

	public void Alert()
	{
		m_Animator?.SetTrigger("Alert");
	}

	private void SetAnimationOverrideController()
	{
		m_AnimatorOverrideController = new AnimatorOverrideController(Animator.runtimeAnimatorController);
		Animator.runtimeAnimatorController = m_AnimatorOverrideController;
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

	public void UpdateClipOverride(AnimationClip clip)
	{
		string text = clip.name;
		m_AimationClipOverrides[text] = clip;
		m_AnimatorOverrideController.ApplyOverrides(m_AimationClipOverrides);
	}

	public void SetAnimatorController(RuntimeAnimatorController animatorController)
	{
		m_Animator.runtimeAnimatorController = animatorController;
	}

	public void SetActiveRenderers(bool active)
	{
		for (int i = 0; i < ModelRenderers.Length; i++)
		{
			ModelRenderers[i].enabled = active;
		}
	}

	public virtual void UpdateMaterials()
	{
	}

	protected override void OnDisposed()
	{
		m_AnimatorOverrideController = null;
		m_AimationClipOverrides = null;
		ModelRenderers = null;
		m_RagdollController = null;
		base.OnDisposed();
	}
}
