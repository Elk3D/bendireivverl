using System;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class FPTransformTracker : FPComponent
{
    [Serializable]
    public class ClampRotations
    {
        public bool IsActive;
        public float LowClamp = 230f;
        public float HighClamp = 290f;
    }

    [SerializeField] private bool m_IsActive = true;

    [Header("Transform Options")]
    [SerializeField] private Transform m_Target;
    [SerializeField] private Vector3 m_ForwardOffset = new Vector3(0f, 270f, 270f);
    [SerializeField] private float m_Speed = 15f;

    [Header("Clamp Options")]
    [SerializeField] private ClampRotations m_ClampX;
    [SerializeField] private ClampRotations m_ClampY;
    [SerializeField] private ClampRotations m_ClampZ;

    private Quaternion m_LastLookRotation;

    public override void Awake() => m_LastLookRotation = transform.rotation;

    private void LateUpdate()
    {
        if (!m_IsActive || m_Target == null) return;
        var q = Quaternion.LookRotation(m_Target.forward) * Quaternion.Euler(m_ForwardOffset);
        var e = q.eulerAngles;
        Clamp(ref e.x, ref m_ClampX);
        Clamp(ref e.y, ref m_ClampY);
        Clamp(ref e.z, ref m_ClampZ);
        transform.rotation = Quaternion.Slerp(m_LastLookRotation, Quaternion.Euler(e), m_Speed * Time.deltaTime);
        m_LastLookRotation = transform.rotation;
    }

    private void Clamp(ref float rot, ref ClampRotations axis)
    {
        if (!axis.IsActive) return;
        if (rot < axis.LowClamp) rot = axis.LowClamp;
        else if (rot > axis.HighClamp) rot = axis.HighClamp;
    }

    public void SetActive(bool active) { m_LastLookRotation = transform.rotation; m_IsActive = active; }
    public void SetTarget(Transform target) => m_Target = target;
}
