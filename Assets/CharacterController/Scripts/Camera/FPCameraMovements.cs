using System;
using UnityEngine;

[Serializable]
public class FPCameraMovements : FPMonoBehaviour
{
    [SerializeField] private bool m_IsActive = true;
    [SerializeField] private float m_SwaySpeed = 0.6f;
    [SerializeField] private float m_BaseSwayAmount = 1.5f;
    [SerializeField] private float m_TrackingSwayAmount = 1.5f;
    [SerializeField] private float m_TrackingBias;
    [SerializeField] private float m_FollowSpeed = 1f;

    private Transform m_Target;
    private Quaternion m_OriginalRotation;
    private Vector3 m_FollowVelocity;
    private Vector3 m_FollowAngles;

    public bool IsActive => m_IsActive;

    // Call after construction to apply a settings SO value, otherwise the serialized default is used.
    public void ApplySettings(FPControllerSettings settings)
    {
        if (settings != null) m_IsActive = settings.ViewSwaying;
    }

    public override void Awake()
    {
        base.Awake();
        m_OriginalRotation = transform.localRotation;
        m_Target = new GameObject("FP Camera Sway Target").transform;
        m_Target.SetParent(transform);
        m_Target.localPosition = Vector3.forward;
        m_Target.localEulerAngles = Vector3.zero;
    }

    private void Update()
    {
        if (m_IsActive && !IsDisposed && !FPPause.IsPaused) Sway();
    }

    public void SetActive(bool active)
    {
        m_IsActive = active;
        if (!m_IsActive) transform.localEulerAngles = Vector3.zero;
    }

    public void Sway()
    {
        transform.localRotation = m_OriginalRotation;
        Vector3 v = transform.parent.InverseTransformPoint(m_Target.position);
        float yaw = Mathf.Clamp(Mathf.Atan2(v.x, v.z) * 57.29578f, -10.5f, 10.5f);
        transform.localRotation = m_OriginalRotation * Quaternion.Euler(0f, yaw, 0f);
        v = transform.parent.InverseTransformPoint(m_Target.position);
        float pitch = Mathf.Clamp(Mathf.Atan2(v.y, v.z) * 57.29578f, -10.5f, 10.5f);
        m_FollowAngles = Vector3.SmoothDamp(m_FollowAngles,
            new Vector3(m_FollowAngles.x + Mathf.DeltaAngle(m_FollowAngles.x, pitch),
                        m_FollowAngles.y + Mathf.DeltaAngle(m_FollowAngles.y, yaw)),
            ref m_FollowVelocity, m_FollowSpeed);
        transform.localRotation = m_OriginalRotation * Quaternion.Euler(-m_FollowAngles.x, m_FollowAngles.y, 0f);
        float nx = (Mathf.PerlinNoise(0f, Time.time * m_SwaySpeed) - 0.5f) * m_BaseSwayAmount;
        float ny = (Mathf.PerlinNoise(0f, Time.time * m_SwaySpeed + 100f) - 0.5f) * m_BaseSwayAmount;
        float tx = (Mathf.PerlinNoise(0f, Time.time * m_SwaySpeed) - 0.5f + m_TrackingBias) * -m_TrackingSwayAmount * m_FollowVelocity.x;
        float ty = (Mathf.PerlinNoise(0f, Time.time * m_SwaySpeed + 100f) - 0.5f + m_TrackingBias) * m_TrackingSwayAmount * m_FollowVelocity.y;
        float fx = nx + tx;
        float fy = ny + ty;
        transform.Rotate(fx, fy, -fy * 0.5f);
    }

    protected override void OnDisposed() { m_Target = null; base.OnDisposed(); }
}
