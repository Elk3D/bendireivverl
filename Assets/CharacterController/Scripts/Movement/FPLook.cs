using System;
using UnityEngine;

[Serializable]
public class FPLook : FPDisposable
{
    [SerializeField] private float m_Sensitivity = 3f;
    [SerializeField] private bool m_ClampVerticalRotation = true;
    [SerializeField] private float m_VerticalMinClamp = -82f;
    [SerializeField] private float m_VerticalMaxClamp = 82f;

    private Quaternion m_CharacterTargetRotation;
    private Quaternion m_CameraTargetRotation;
    private bool m_IsRotationInitialized;
    private bool m_IsInitialVerticalClampInitialized;
    private float m_HorizontalClamp;
    private float m_InitialVerticalMaxClamp;
    private float m_InitialVerticalMinClamp;
    private float m_TurnSpeedBoostTimer;
    private float m_InputX;
    private float m_InputY;

    public bool ClampVert => m_ClampVerticalRotation;
    public bool HasHorizontalLock { get; private set; }

    public void Initialize(Transform character, Transform camera)
    {
        ResetRotation(character, camera);
        if (!m_IsInitialVerticalClampInitialized)
        {
            m_IsInitialVerticalClampInitialized = true;
            m_InitialVerticalMinClamp = m_VerticalMinClamp;
            m_InitialVerticalMaxClamp = m_VerticalMaxClamp;
        }
    }

    public void ForceRotation(Quaternion q) => m_CharacterTargetRotation = q;
    public void ForceCameraRotation(Quaternion q) => m_CameraTargetRotation = q;

    public void GetInput()
    {
        float tSpeed = m_TurnSpeedBoostTimer > 0.3f ? 0f : -0.5f;
        float lx = FPInput.LookX(tSpeed);
        if (Mathf.Abs(lx) > 0.1f) m_TurnSpeedBoostTimer += Time.deltaTime;
        else m_TurnSpeedBoostTimer = 0f;
        float hMult = FPInput.HasController ? 80f : 150f;
        float vMult = FPInput.HasController ? 40f : 150f;
        m_InputX = lx * m_Sensitivity * hMult * Time.fixedDeltaTime;
        m_InputY = -FPInput.LookY() * m_Sensitivity * vMult * Time.fixedDeltaTime;
    }

    public void Rotation(Transform character) => Rotation(character, null, hasGravity: true);
    public void Rotation(Transform character, bool hasGravity) => Rotation(character, null, hasGravity);
    public void Rotation(Transform character, Transform camera) => Rotation(character, camera, hasGravity: true);

    public void Rotation(Transform character, params Transform[] cameras)
    {
        foreach (var cam in cameras) if (cam) Rotation(character, cam, hasGravity: true);
    }

    public void Rotation(Transform character, Transform camera, bool hasGravity)
    {
        if (!IsNullRotation(m_InputX, m_InputY))
        {
            if (hasGravity)
            {
                m_CharacterTargetRotation *= Quaternion.Euler(0f, m_InputX, 0f);
                if (HasHorizontalLock)
                    m_CharacterTargetRotation = ClampRotationY(m_CharacterTargetRotation, -m_HorizontalClamp, m_HorizontalClamp);
                character.localRotation = m_CharacterTargetRotation;
                if (camera)
                {
                    m_CameraTargetRotation *= Quaternion.Euler(m_InputY, 0f, 0f);
                    if (m_ClampVerticalRotation)
                        m_CameraTargetRotation = ClampRotationX(m_CameraTargetRotation, m_VerticalMinClamp, m_VerticalMaxClamp);
                    camera.localRotation = m_CameraTargetRotation;
                    var e = camera.localEulerAngles; e.z = 0f; camera.localEulerAngles = e;
                }
            }
            else
            {
                m_CharacterTargetRotation *= Quaternion.Euler(m_InputY, m_InputX, 0f);
                character.localRotation = m_CharacterTargetRotation;
            }
        }
        else
        {
            character.localRotation = m_CharacterTargetRotation;
            if (camera) camera.localRotation = m_CameraTargetRotation;
        }
        UpdateCursorLock();
    }

    public void SmoothLook(Transform character, Transform camera, Vector3 direction)
    {
        var q = Quaternion.LookRotation(direction);
        character.localRotation = Quaternion.Lerp(character.rotation, q, 2f * Time.deltaTime);
        character.localEulerAngles = new Vector3(0f, character.localEulerAngles.y, 0f);
        camera.localRotation = Quaternion.Lerp(camera.rotation, q, 2f * Time.deltaTime);
        var lr = camera.localRotation;
        lr = ClampRotationX(lr, m_VerticalMinClamp, m_VerticalMaxClamp);
        lr = ClampRotationY(lr, -m_HorizontalClamp, m_HorizontalClamp);
        camera.localRotation = lr;
        var e = camera.localEulerAngles; e.z = 0f; camera.localEulerAngles = e;
    }

    public void ResetRotation(Transform character) => ResetRotation(character, null);

    public void ResetRotation(Transform character, Transform camera)
    {
        m_CharacterTargetRotation.eulerAngles = new Vector3(0f, character.localRotation.eulerAngles.y, 0f);
        if (camera)
            m_CameraTargetRotation.eulerAngles = new Vector3(camera.localRotation.eulerAngles.x, 0f, 0f);
    }

    public void ResetVerticalClamp()
    {
        m_VerticalMinClamp = m_InitialVerticalMinClamp;
        m_VerticalMaxClamp = m_InitialVerticalMaxClamp;
    }

    public void SetVerticalClamp(float clamp) { m_VerticalMinClamp = -clamp; m_VerticalMaxClamp = clamp; }
    public void SetHorizontalClamp(float clamp) => m_HorizontalClamp = clamp;
    public void HorizontalClampSetActive(bool active) => HasHorizontalLock = active;
    public void UpdateCursorLock() => Cursor.lockState = CursorLockMode.Locked;
    public void SetSensitivity(float sensitivity) => m_Sensitivity = sensitivity * 5f;

    private bool IsNullRotation(float h, float v)
    {
        if (h == 0f && v == 0f) return true;
        if (!m_IsRotationInitialized) return m_IsRotationInitialized = true;
        return false;
    }

    private Quaternion ClampRotationX(Quaternion q, float min, float max)
    {
        q.x /= q.w; q.y /= q.w; q.z /= q.w; q.w = 1f;
        float v = 114.59156f * Mathf.Atan(q.x);
        v = Mathf.Clamp(v, min, max);
        q.x = Mathf.Tan(MathF.PI / 360f * v);
        return q;
    }

    private Quaternion ClampRotationY(Quaternion q, float min, float max)
    {
        q.x /= q.w; q.y /= q.w; q.z /= q.w; q.w = 1f;
        float v = 114.59156f * Mathf.Atan(q.y);
        v = Mathf.Clamp(v, min, max);
        q.y = Mathf.Tan(MathF.PI / 360f * v);
        return q;
    }

    protected override void OnDisposed() => base.OnDisposed();
}
