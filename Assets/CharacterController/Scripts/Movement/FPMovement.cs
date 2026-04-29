using System;
using UnityEngine;

[Serializable]
public class FPMovement : FPDisposable
{
    [Header("Movement Options")]
    [SerializeField] private float m_MoveSpeed = 2f;
    [SerializeField] private bool m_CanRun = true;
    [SerializeField] private float m_RunSpeed = 4f;
    [SerializeField] private bool m_CanJump = true;
    [SerializeField] private float m_JumpSpeed = 4f;
    [SerializeField] private bool m_CanCrouch = true;

    private readonly float m_Gravity = 0.025f;
    private float m_ActiveGravity;
    private Vector3 m_ExternalForce = Vector3.zero;
    private float m_DefaultMoveSpeed = 2f;
    private CharacterController m_CharacterController;
    private Vector3 m_MoveDirection = Vector3.zero;
    private Vector3 m_InitialAirPosition;

    public bool IsSlowed;

    public Vector3 MoveDirection => m_MoveDirection;
    public Vector2 MovementInput { get; private set; }
    public Vector3 PreviousPosition { get; private set; }
    public Vector3 CurrentPosition { get; private set; }
    public bool PreviouslyGrounded { get; private set; }
    public bool CrouchInput { get; private set; }
    public bool JumpInput { get; private set; }
    public float CurrentSpeed { get; private set; }
    public bool CanRun { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsRunLocked { get; private set; }
    public bool CanCrouch { get; private set; }
    public bool IsCrouched { get; private set; }
    public bool CanJump { get; private set; }
    public Vector3 InitialAirPosition { get => m_InitialAirPosition; set => m_InitialAirPosition = value; }

    public void LockRun() => IsRunLocked = true;
    public void UnlockRun() => IsRunLocked = false;
    public void StopRun() => IsRunning = false;
    public void ForceLockRun() => CanRun = false;
    public void ForceUnlockRun() => CanRun = true;
    public void LockCrouch() => CanCrouch = false;
    public void UnlockCrouch() => CanCrouch = true;
    public void LockJump() => CanJump = false;
    public void UnlockJump() => CanJump = true;

    public void Initialize(CharacterController cc)
    {
        m_CharacterController = cc;
        CanJump = m_CanJump;
        CanRun = m_CanRun;
        CanCrouch = m_CanCrouch;
        PreviouslyGrounded = true;
        m_DefaultMoveSpeed = m_MoveSpeed;
        m_MoveDirection.y = -0.27f;
    }

    public void SetMoveSpeed(float speed, bool isSlowed = false) { m_MoveSpeed = speed; IsSlowed = isSlowed; }
    public void ResetMoveSpeed() { m_MoveSpeed = m_DefaultMoveSpeed; IsSlowed = false; }

    public void UpdateMovementInput()
    {
        GetJumpInput();
        GetCrouchInput();
        GetMovementInput();
    }

    private void GetJumpInput()
    {
        if (JumpInput || !m_CharacterController.isGrounded || !CanJump) return;
        if (!Physics.SphereCast(m_CharacterController.transform.position, m_CharacterController.radius, Vector3.up, out _,
                6.5f - m_CharacterController.radius, ~(1 << LayerMask.NameToLayer("Player")), QueryTriggerInteraction.Ignore))
        {
            JumpInput = FPInput.Jump();
            if (JumpInput)
            {
                m_ActiveGravity += m_JumpSpeed * (IsRunning && MovementInput.y > 0f ? 0.11f : 0.1f);
                IsCrouched = false;
            }
        }
        JumpInput = false;
    }

    private void GetCrouchInput()
    {
        if (!CrouchInput && m_CharacterController.isGrounded && CanCrouch)
        {
            CrouchInput = FPInput.Crouch();
            if (CrouchInput) IsCrouched = !IsCrouched;
            CrouchInput = false;
        }
    }

    private void GetMovementInput()
    {
        float x = MovementInput.x;
        float y = MovementInput.y;
        if (m_CharacterController.isGrounded)
        {
            x = FPInput.MoveX();
            y = FPInput.MoveY();
        }
        else
        {
            if (MovementInput.x == 0f) x = FPInput.MoveXRaw() / 2f;
            if (MovementInput.y == 0f) y = FPInput.MoveYRaw() / 2f;
        }
        IsRunning = y > 0f && CanRun && !IsRunLocked && FPInput.Run();
        if (IsRunning && IsCrouched &&
            !Physics.SphereCast(m_CharacterController.transform.position, m_CharacterController.radius, Vector3.up, out _,
                6.5f - m_CharacterController.radius, ~(1 << LayerMask.NameToLayer("Player")), QueryTriggerInteraction.Ignore))
            IsCrouched = false;
        if (IsCrouched) IsRunning = false;
        CurrentSpeed = IsRunning ? m_RunSpeed : m_MoveSpeed;
        if (IsCrouched) CurrentSpeed = m_MoveSpeed / 2f;
        MovementInput = new Vector2(x, y);
        if (MovementInput.magnitude > 1f || MovementInput.magnitude < -1f) MovementInput.Normalize();
        CurrentSpeed *= 0.1f;
    }

    public void UpdateMovement(Transform t)
    {
        GetMovement(t);
        GetPhysics();
        GetGrounding();
        GetCrouch();
    }

    private void GetMovement(Transform t)
    {
        if (!m_CharacterController.enabled) return;
        Vector3 v = t.forward * MovementInput.y + t.right * MovementInput.x;
        m_MoveDirection.x = Mathf.Clamp(v.x, -1f, 1f);
        m_MoveDirection.z = Mathf.Clamp(v.z, -1f, 1f);
        float y = MoveDirection.y;
        m_MoveDirection = m_MoveDirection.normalized * CurrentSpeed;
        m_MoveDirection.y = y;
        bool grounded = m_ActiveGravity <= 0f && m_CharacterController.isGrounded;
        if (grounded) { m_ActiveGravity = 0f; m_MoveDirection.y = -0.27f; }
        else { m_ActiveGravity -= m_Gravity; m_MoveDirection.y = m_ActiveGravity; }
        PreviousPosition = m_CharacterController.transform.position;
        var flags = m_CharacterController.Move(MoveDirection);
        if (m_ActiveGravity < 0f) m_CharacterController.Move(Vector3.up * -0.01f);
        if (flags == CollisionFlags.Above && m_ActiveGravity > 0f) m_ActiveGravity = 0f;
        CurrentPosition = m_CharacterController.transform.position;
    }

    private void GetPhysics()
    {
        if (m_ExternalForce.magnitude <= 0f) return;
        m_CharacterController.Move(m_ExternalForce * Time.fixedDeltaTime);
        m_ExternalForce = Vector3.MoveTowards(m_ExternalForce, Vector3.zero, 0.95f);
    }

    private void GetGrounding()
    {
        if (m_CharacterController.isGrounded && !PreviouslyGrounded && m_ActiveGravity <= 0f)
            m_ActiveGravity = 0f;
        if (PreviouslyGrounded) m_InitialAirPosition = m_CharacterController.transform.position;
        PreviouslyGrounded = m_CharacterController.isGrounded;
    }

    public void SetPreviouslyGrounded(bool active) => PreviouslyGrounded = active;

    private void GetCrouch()
    {
        if (m_CharacterController.isGrounded && CrouchInput) IsCrouched = !IsCrouched;
    }

    public void CancelMovement()
    {
        MovementInput = Vector2.zero;
        CurrentSpeed = 0f;
        m_ActiveGravity = 0f;
        m_ExternalForce = Vector3.zero;
        CrouchInput = false;
        JumpInput = false;
    }

    public void ForceStand() { IsCrouched = false; CrouchInput = false; }
    public void ForceCrouch() { UnlockCrouch(); IsCrouched = true; CrouchInput = false; }
    public void AddForce(Vector3 force) => m_ExternalForce += force;
    public void SetCrouchInput(bool isCrouched) => IsCrouched = isCrouched;

    protected override void OnDisposed() { m_CharacterController = null; base.OnDisposed(); }
}
