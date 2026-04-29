using DG.Tweening;
using UnityEngine;

[DefaultExecutionOrder(-95)]
public class FPCamera : FPMonoBehaviour
{
    // Shader property used by first-person depth projection.
    private static readonly int kFirstPersonProjMatrixId = Shader.PropertyToID("_FirstPersonProjMatrix");

    [Header("First Person")]
    [SerializeField] private Camera m_FirstPersonCamera;
    [SerializeField] private GameObject m_FirstPersonArms;
    [SerializeField] private Transform m_ArmsContainer;

    private Renderer[] m_ArmRenderers;

    // Global accessor so FPCameraEffects can reach shake without needing a scene reference.
    public static FPCamera Instance { get; private set; }

    public Camera FirstPersonCamera => m_FirstPersonCamera;
    public Transform ArmsContainer => m_ArmsContainer;
    public Camera Camera { get; private set; }
    public Animator FirstPersonArmsAnimator { get; private set; }
    public Transform HeadContainer { get; private set; }
    public Transform CameraContainer { get; private set; }

    public override void Awake()
    {
        Instance = this;
        Camera = GetComponent<Camera>();
        if (Camera != null) Camera.enabled = false;
        if (m_FirstPersonArms != null)
        {
            FirstPersonArmsAnimator = m_FirstPersonArms.GetComponent<Animator>();
            m_ArmRenderers = m_FirstPersonArms.GetComponentsInChildren<Renderer>();
        }
    }

    public override void Start()
    {
        if (Camera != null) Camera.enabled = true;
    }

    private void LateUpdate()
    {
        if (m_FirstPersonCamera != null)
            Shader.SetGlobalMatrix(kFirstPersonProjMatrixId, m_FirstPersonCamera.projectionMatrix);
    }

    public void Initialize(Transform headContainer, Transform cameraContainer)
    {
        HeadContainer = headContainer;
        CameraContainer = cameraContainer;
    }

    public void SetFirstPersonArmsActive(bool active)
    {
        if (m_FirstPersonArms == null) return;
        m_FirstPersonArms.SetActive(active);
        if (m_ArmRenderers == null) return;
        foreach (var r in m_ArmRenderers)
        {
            if (active) r.material.EnableKeyword("FIRST_PERSON");
            else r.material.DisableKeyword("FIRST_PERSON");
        }
    }

    public void ShakeCamera(float duration, float strength = 10f, int vibrato = 10, float randomness = 90f, bool fadeOut = true, bool vibrate = false)
    {
        Camera.transform.DOKill();
        Camera.transform.localEulerAngles = Vector3.zero;
        Camera.DOShakeRotation(duration, strength, vibrato, randomness, fadeOut)
              .OnComplete(() => Camera.transform.localEulerAngles = Vector3.zero);
    }

    protected override void OnDisposed()
    {
        Instance = null;
        Camera = null;
        CameraContainer = null;
        HeadContainer = null;
        FirstPersonArmsAnimator = null;
        m_ArmRenderers = null;
        base.OnDisposed();
    }
}
