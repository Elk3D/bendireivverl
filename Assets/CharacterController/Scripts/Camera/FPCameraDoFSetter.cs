using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Optional component: auto-adjusts DoF focal distance by SphereCasting forward.
// Attach to the main camera GameObject alongside a PostProcessLayer and PostProcessVolume.
public class FPCameraDoFSetter : FPMonoBehaviour
{
    private const float MAX_FOCAL_DISTANCE = 10f;

    [Header("DoF Settings")]
    [SerializeField] private LayerMask m_LayerMask;
    [SerializeField] private float m_Speed = 5f;
    [SerializeField] private float m_Radius = 0.015f;
    [SerializeField] private float m_MaxDistance = 5f;
    [SerializeField] private float m_CloseRadius = 1f;
    [SerializeField] private float m_CloseMaxDistance = 2f;

    private DepthOfField m_DOFSettings;
    private RaycastHit m_Hit;
    private float m_FocalDistance = MAX_FOCAL_DISTANCE;
    private readonly float m_ResetSpeed = 1f;
    private bool m_IsActive;

    public void Initialize(bool isActive, DepthOfField dof)
    {
        m_DOFSettings = dof;
        m_IsActive = isActive;
    }

    private void Update()
    {
        if (!m_IsActive || m_DOFSettings == null || !m_DOFSettings.enabled || IsDisposed || FPPause.IsPaused) return;
        float target = MAX_FOCAL_DISTANCE;
        if (Physics.SphereCast(transform.position, m_Radius, transform.forward, out m_Hit, m_MaxDistance, m_LayerMask, QueryTriggerInteraction.Ignore))
        {
            target = m_Hit.distance;
            if (!m_DOFSettings.active) m_DOFSettings.active = true;
        }
        if (m_FocalDistance != target)
        {
            float speed = target > m_MaxDistance && m_FocalDistance < target ? m_ResetSpeed : m_Speed;
            m_FocalDistance = Mathf.Lerp(m_FocalDistance, target, speed * Time.deltaTime);
            if (m_FocalDistance > 9.99f) m_FocalDistance = MAX_FOCAL_DISTANCE;
            var fp = m_DOFSettings.focusDistance;
            fp.value = m_FocalDistance;
            m_DOFSettings.focusDistance = fp;
        }
        else if (!Physics.SphereCast(transform.position, m_CloseRadius, transform.forward, out m_Hit, m_CloseMaxDistance, m_LayerMask, QueryTriggerInteraction.Ignore))
        {
            if (m_DOFSettings.active) m_DOFSettings.active = false;
        }
        else if (!m_DOFSettings.active) m_DOFSettings.active = true;
    }

    protected override void OnDisposed() { m_DOFSettings = null; base.OnDisposed(); }
}
