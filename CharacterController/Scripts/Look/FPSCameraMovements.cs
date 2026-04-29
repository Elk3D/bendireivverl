using UnityEngine;

namespace CharacterController
{
    // Attach to the camera pivot (child of the player root, parent of the Camera).
    // Produces Perlin-noise head-bob and a direction-tracking sway.
    //
    // Removed from original:
    //   - GameManager.Instance.PlayerSettings.ViewSwaying initialisation (now purely Inspector-driven)
    //   - GameManager.Instance.IsPaused guard (disable the component externally to pause)
    public class FPSCameraMovements : FPSMonoBehaviour
    {
        [SerializeField] private bool  m_IsActive          = true;
        [SerializeField] private float m_SwaySpeed         = 0.6f;
        [SerializeField] private float m_BaseSwayAmount    = 1.5f;
        [SerializeField] private float m_TrackingSwayAmount = 1.5f;
        [SerializeField] private float m_TrackingBias;
        [SerializeField] private float m_FollowSpeed       = 1f;

        private Transform  m_Target;
        private Quaternion m_OriginalRotation;
        private Vector3    m_FollowVelocity;
        private Vector3    m_FollowAngles;

        public bool IsActive => m_IsActive;

        public override void Awake()
        {
            base.Awake();
            m_OriginalRotation = transform.localRotation;
            m_Target = new GameObject("FPS_ForwardCameraTarget").transform;
            m_Target.SetParent(transform);
            m_Target.localPosition    = Vector3.forward;
            m_Target.localEulerAngles = Vector3.zero;
        }

        private void Update()
        {
            if (m_IsActive && !IsDisposed)
                Sway();
        }

        public void SetActive(bool active)
        {
            m_IsActive = active;
            if (!m_IsActive)
                transform.localEulerAngles = Vector3.zero;
        }

        public void Sway()
        {
            transform.localRotation = m_OriginalRotation;

            Vector3 local = transform.parent.InverseTransformPoint(m_Target.position);

            float yaw = Mathf.Clamp(Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg, -10.5f, 10.5f);
            transform.localRotation = m_OriginalRotation * Quaternion.Euler(0f, yaw, 0f);

            local = transform.parent.InverseTransformPoint(m_Target.position);
            float pitch = Mathf.Clamp(Mathf.Atan2(local.y, local.z) * Mathf.Rad2Deg, -10.5f, 10.5f);

            m_FollowAngles = Vector3.SmoothDamp(
                m_FollowAngles,
                new Vector3(m_FollowAngles.x + Mathf.DeltaAngle(m_FollowAngles.x, pitch),
                            m_FollowAngles.y + Mathf.DeltaAngle(m_FollowAngles.y, yaw)),
                ref m_FollowVelocity, m_FollowSpeed);

            transform.localRotation = m_OriginalRotation * Quaternion.Euler(-m_FollowAngles.x, m_FollowAngles.y, 0f);

            float t = Time.time * m_SwaySpeed;
            float noiseX = (Mathf.PerlinNoise(0f, t)         - 0.5f) * m_BaseSwayAmount;
            float noiseY = (Mathf.PerlinNoise(0f, t + 100f)  - 0.5f) * m_BaseSwayAmount;

            float trackX = ((Mathf.PerlinNoise(0f, t)        - 0.5f + m_TrackingBias) * (-m_TrackingSwayAmount) * m_FollowVelocity.x);
            float trackY = ((Mathf.PerlinNoise(0f, t + 100f) - 0.5f + m_TrackingBias) * m_TrackingSwayAmount    * m_FollowVelocity.y);

            float xAngle = noiseX + trackX;
            float yAngle = noiseY + trackY;
            transform.Rotate(xAngle, yAngle, -yAngle * 0.5f);
        }

        protected override void OnDisposed()
        {
            m_Target = null;
            base.OnDisposed();
        }
    }
}
