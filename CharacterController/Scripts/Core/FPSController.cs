using UnityEngine;

namespace CharacterController
{
    // Top-level MonoBehaviour that wires FPSMovement, FPSLook, FPSCameraFOV, and
    // FPSInteraction together.  Replaces the Player / PlayerStateMachine /
    // PlayerStateDefault orchestration from the source project.
    //
    // Required hierarchy (minimum):
    //   [Player root]           — this component + CharacterController
    //     [CameraPivot]         — FPSCameraMovements (optional but recommended)
    //       [Main Camera]       — assign to m_CameraTransform and m_Cameras[0]
    //
    // SmoothCamera:
    //   false (default) — rotations applied every Update  (same as SmoothCamera off in original)
    //   true            — rotations applied every FixedUpdate (same as SmoothCamera on in original)
    [RequireComponent(typeof(UnityEngine.CharacterController))]
    public class FPSController : MonoBehaviour
    {
        [Header("Camera")]
        [Tooltip("The camera's Transform (or camera pivot). Look input is applied here.")]
        [SerializeField] private Transform m_CameraTransform;

        [Tooltip("All Camera components whose FOV should be driven. Usually just the main camera.")]
        [SerializeField] private Camera[] m_Cameras;

        [Tooltip("Apply rotations in FixedUpdate instead of Update (smoother on low frame-rates).")]
        [SerializeField] private bool m_SmoothCamera = false;

        [Header("Systems")]
        [SerializeField] private FPSMovement   m_Movement;
        [SerializeField] private FPSLook       m_Look;
        [SerializeField] private FPSCameraFOV  m_CameraFOV;
        [SerializeField] private FPSInteraction m_Interaction;

        private UnityEngine.CharacterController m_CharacterController;

        // ── Public accessors ──────────────────────────────────────────────────────

        public FPSMovement    Movement    => m_Movement;
        public FPSLook        Look        => m_Look;
        public FPSCameraFOV   CameraFOV   => m_CameraFOV;
        public FPSInteraction Interaction => m_Interaction;

        // ── Unity lifecycle ───────────────────────────────────────────────────────

        private void Awake()
        {
            m_CharacterController = GetComponent<UnityEngine.CharacterController>();
            m_Movement.Initialize(m_CharacterController);
            m_Look.Initialize(transform, m_CameraTransform);
            m_CameraFOV.Init(m_Cameras);
        }

        private void Update()
        {
            m_Look.GetInput();

            if (!m_SmoothCamera)
                m_Look.Rotation(transform, m_CameraTransform);

            m_Movement.UpdateMovementInput();

            // Interaction ray fires from the camera position / forward, not the body.
            Transform interactOrigin = m_CameraTransform != null ? m_CameraTransform : transform;
            m_Interaction.Update(interactOrigin);

            m_CameraFOV.UpdateFOV(m_Movement.IsRunning);
        }

        private void FixedUpdate()
        {
            if (m_SmoothCamera)
                m_Look.Rotation(transform, m_CameraTransform);

            m_Movement.UpdateMovement(transform);
        }

        private void OnDestroy()
        {
            m_Movement?.Dispose();
            m_Look?.Dispose();
            m_CameraFOV?.Dispose();
            m_Interaction?.Dispose();
        }
    }
}
