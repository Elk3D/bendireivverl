using System.Collections.Generic;
using UnityEngine;

public class FPContent : FPMonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator m_Animator;
    [SerializeField] private FPAnimationClipGroup[] m_ClipGroups;

    private AnimatorOverrideController m_OverrideController;
    private FPAnimationClipOverrides m_ClipOverrides;

    public Animator Animator => m_Animator;

    public override void Awake()
    {
        if (m_Animator == null) m_Animator = GetComponent<Animator>();
        SetupOverrideController();
    }

    private void SetupOverrideController()
    {
        m_OverrideController = new AnimatorOverrideController(m_Animator.runtimeAnimatorController);
        m_Animator.runtimeAnimatorController = m_OverrideController;
        m_ClipOverrides = new FPAnimationClipOverrides(m_OverrideController.overridesCount);
        m_OverrideController.GetOverrides(m_ClipOverrides);
        var clips = new List<AnimationClip>();
        foreach (var g in m_ClipGroups) { g.Initialize(); clips.Add(g.AnimationClip); }
        UpdateClipOverrides(clips.ToArray());
    }

    public void UpdateClipOverrides(params AnimationClip[] clips)
    {
        if (m_OverrideController == null || m_ClipOverrides == null || clips == null || clips.Length == 0) return;
        foreach (var clip in clips) m_ClipOverrides[clip.name] = clip;
        m_OverrideController.ApplyOverrides(m_ClipOverrides);
    }

    protected override void OnDisposed()
    {
        m_OverrideController = null;
        m_ClipOverrides = null;
        base.OnDisposed();
    }
}
