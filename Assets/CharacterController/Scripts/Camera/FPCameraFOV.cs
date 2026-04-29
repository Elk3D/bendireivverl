using System;
using UnityEngine;

[Serializable]
public class FPCameraFOV : FPDisposable
{
    [SerializeField] private bool m_Active = true;
    [SerializeField] private float m_BaseFOV = 55f;
    [SerializeField] private float m_RunFOV = 60f;
    [SerializeField] private float m_TransitionSpeed = 0.5f;

    private Camera[] m_Cameras;

    public float FOV => m_Cameras != null && m_Cameras.Length > 0 ? m_Cameras[0].fieldOfView : 0f;

    public void Init(params Camera[] cameras)
    {
        if (!m_Active) return;
        m_Cameras = cameras;
        SetFOV(m_BaseFOV);
    }

    public void UpdateVOD(bool isRunning)
    {
        if (!m_Active) return;
        if (isRunning) { if (FOV < m_RunFOV) SetFOV(FOV + m_TransitionSpeed); }
        else { if (FOV > m_BaseFOV) SetFOV(FOV - m_TransitionSpeed); }
    }

    public void SetFOV(float value)
    {
        if (m_Cameras == null) return;
        foreach (var cam in m_Cameras) if (cam != null) cam.fieldOfView = value;
    }

    public void SetActiveFOV(bool active) => m_Active = active;
    public void ResetFOV() => SetFOV(m_BaseFOV);

    protected override void OnDisposed() { m_Cameras = null; base.OnDisposed(); }
}
