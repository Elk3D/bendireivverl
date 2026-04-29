using System;
using UnityEngine;

namespace CharacterController
{
    [Serializable]
    public class FPSCameraFOV : FPSDisposable
    {
        [SerializeField] private bool  m_Active          = true;
        [SerializeField] private float m_BaseFOV         = 55f;
        [SerializeField] private float m_RunFOV          = 60f;
        [SerializeField] private float m_TransitionSpeed = 0.5f;

        private Camera[] m_Cameras;

        public float FOV
        {
            get
            {
                if (m_Cameras != null && m_Cameras.Length > 0)
                    return m_Cameras[0].fieldOfView;
                return 0f;
            }
        }

        public void Init(params Camera[] cameras)
        {
            if (!m_Active) return;
            m_Cameras = cameras;
            SetFOV(m_BaseFOV);
        }

        // Call every frame (Update or LateUpdate) with the current run state.
        public void UpdateFOV(bool isRunning)
        {
            if (!m_Active) return;
            if (isRunning)
            {
                if (FOV < m_RunFOV) SetFOV(FOV + m_TransitionSpeed);
            }
            else
            {
                if (FOV > m_BaseFOV) SetFOV(FOV - m_TransitionSpeed);
            }
        }

        public void SetFOV(float value)
        {
            if (m_Cameras == null) return;
            foreach (Camera cam in m_Cameras)
                if (cam != null)
                    cam.fieldOfView = value;
        }

        public void ResetFOV()       => SetFOV(m_BaseFOV);
        public void SetActive(bool active) => m_Active = active;

        protected override void OnDisposed()
        {
            m_Cameras = null;
            base.OnDisposed();
        }
    }
}
