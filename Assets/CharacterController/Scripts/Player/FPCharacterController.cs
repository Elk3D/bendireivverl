using System;
using DG.Tweening;
using UnityEngine;

[DefaultExecutionOrder(-90)]
public class FPCharacterController : FPPlayerStateMachine
{
    [Header("Camera Transforms")]
    [SerializeField] private Transform m_CameraPivot;
    [SerializeField] private Transform m_HeadContainer;
    [SerializeField] private Transform m_CameraContainer;
    [SerializeField] private Transform m_AnimationContainer;

    [Header("Head Tracking")]
    [SerializeField] private Transform m_HeadBobTracker;
    [SerializeField] private FPTransformTracker m_HeadTracker;

    [Header("Arm Tracking")]
    [SerializeField] private Transform m_ArmPivot;

    [Header("Controllers")]
    [SerializeField] private FPMovement m_PlayerMovement;
    [SerializeField] private FPLook m_PlayerLook;
    [SerializeField] private FPLook m_PlayerArmLook;
    [SerializeField] private FPInteraction m_PlayerInteraction;
    [SerializeField] private FPCameraFOV m_CameraFOV;

    [Header("Settings")]
    [SerializeField] private FPControllerSettings m_Settings;

    private GameObject m_InternalTracker;
    private float m_RunTimer;
    private float m_RunCooldown = 5f;
    private float m_ShimmyX;
    private bool m_CanHeadContainerSlerp = true;
    private bool m_CanCameraPivotLerp = true;
    private bool m_IsRunCooldown;
    private bool m_InternalJump;
    private Sequence m_CrouchSequence;
    private bool m_InternalCrouch;

    public Transform CameraPivot => m_CameraPivot;
    public Transform HeadContainer => m_HeadContainer;
    public Transform CameraParent => m_CameraContainer;
    public Transform AnimationContainer => m_AnimationContainer;

    public FPMovement Movement => m_PlayerMovement;
    public FPInteraction Interaction => m_PlayerInteraction;
    public FPCameraFOV CameraFOV => m_CameraFOV;
    public FPControllerSettings Settings => m_Settings;

    protected override bool UseAwake => false;

    public FPCameraMovements CameraMovement { get; private set; }
    public FPCamera Camera { get; private set; }
    public FPContent PlayerContent { get; private set; }
    public FPModelLayers ModelLayers { get; private set; }
    public CharacterController CharacterController { get; private set; }
    public CapsuleCollider Collider { get; private set; }

    public Animator AnimatorBody => PlayerContent?.Animator;
    public Animator AnimatorArms => Camera?.FirstPersonArmsAnimator;

    public bool IsGrounded => CharacterController.isGrounded;
    public bool IsCrouchSequenceActive => m_CrouchSequence != null && m_CrouchSequence.IsPlaying();
    public bool IsCrouched => m_InternalCrouch;

    public event EventHandler OnAnimationComplete;
    public event EventHandler OnAnimationInteract;

    protected override void InternalInitializeOnComplete()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        CharacterController = GetComponent<CharacterController>();
        Collider = gameObject.AddComponent<CapsuleCollider>();
        UpdateCollider();

        CameraMovement = GetComponentInChildren<FPCameraMovements>(includeInactive: true);
        Camera = GetComponentInChildren<FPCamera>(includeInactive: true);

        if (Camera != null)
        {
            Camera.Initialize(m_HeadContainer, m_CameraContainer);
            m_CameraFOV.Init(Camera.Camera, Camera.FirstPersonCamera);
        }

        m_PlayerMovement.Initialize(CharacterController);
        m_PlayerLook.Initialize(base.transform, m_HeadContainer);
        if (m_ArmPivot != null) m_PlayerArmLook.Initialize(base.transform, m_ArmPivot);

        m_InternalTracker = new GameObject("FPInternalTracker");
        m_InternalTracker.transform.SetParent(base.transform);
        m_InternalTracker.transform.position = m_HeadBobTracker.position;

        PlayerContent = GetComponentInChildren<FPContent>();
        ModelLayers = PlayerContent?.GetComponentInChildren<FPModelLayers>();

        if (CameraMovement != null && m_Settings != null)
            CameraMovement.ApplySettings(m_Settings);

        SetState(FPState.Player.Default);
        DisableAllTrackers();
        ModelLayers?.EnableFirstPerson();
        SetHeadTracker(true);
        ShowFirstPersonArms();
    }

    private void UpdateCollider(float height)
    {
        CharacterController.height = height;
        CharacterController.center = new Vector3(0f, height / 2f, 0f);
        UpdateCollider();
    }

    private void UpdateCollider()
    {
        Collider.center = CharacterController.center;
        Collider.radius = CharacterController.radius;
        Collider.height = CharacterController.height;
    }

    public void UpdateClipOverrides(params AnimationClip[] clips)
    {
        PlayerContent?.UpdateClipOverrides(clips);
    }

    public void ResetRunTimer()
    {
        m_RunTimer = 0f;
        m_PlayerMovement.UnlockRun();
        m_IsRunCooldown = false;
    }

    protected override void InternalUpdate()
    {
        if (base.Disabled)
        {
            m_CameraFOV.UpdateVOD(m_PlayerMovement.IsRunning);
            return;
        }

        float stamina = m_Settings != null ? m_Settings.Stamina : 5f;
        if (m_PlayerMovement.CanRun)
        {
            if (!m_PlayerMovement.IsRunLocked && m_PlayerMovement.CanJump && CharacterController.isGrounded
                && FPInput.Jump() && base.CurrentState == FPState.Player.Default)
                m_RunTimer += stamina * 0.15f;

            if (m_PlayerMovement.IsRunning && !m_PlayerMovement.IsRunLocked)
                m_RunTimer += Time.deltaTime;
            else if (!m_PlayerMovement.IsRunning && m_PlayerMovement.IsRunLocked)
            {
                m_RunTimer -= Time.deltaTime;
                if (m_RunTimer <= 0f) ResetRunTimer();
            }
            else if (!m_PlayerMovement.IsRunning && !m_PlayerMovement.IsRunLocked && m_RunTimer > 0f)
            {
                m_RunTimer -= Time.deltaTime * 2f;
                if (m_RunTimer < 0f) m_RunTimer = 0f;
            }

            if (m_RunTimer > stamina)
            {
                m_PlayerMovement.StopRun();
                m_PlayerMovement.LockRun();
                m_RunTimer = m_RunCooldown;
                m_IsRunCooldown = true;
            }
        }

        m_CameraFOV.UpdateVOD(m_PlayerMovement.IsRunning);
    }

    protected override void InternalFixedUpdate()
    {
        m_PlayerLook.UpdateCursorLock();
        GetHeadTracker();
    }

    protected override void InternalLateUpdate() { }

    public void UpdateLookInput()
    {
        m_PlayerLook.GetInput();
        if (m_ArmPivot != null) m_PlayerArmLook.GetInput();
    }

    public void UpdateMovementInput()
    {
        m_PlayerMovement.UpdateMovementInput();
        m_ShimmyX = FPInput.LookX();
        float multiplier = m_PlayerMovement.IsCrouched ? 1f : 2f;
        m_ShimmyX = Mathf.Clamp(m_ShimmyX, -1f, 1f) * multiplier;
    }

    public void UpdateInteractionInput()
    {
        m_PlayerInteraction.Update(m_CameraContainer);
    }

    public void UpdateMovement()
    {
        m_PlayerMovement.UpdateMovement(base.transform);
    }

    public void UpdateRotations()
    {
        m_PlayerLook.Rotation(base.transform, m_HeadContainer);
        if (m_ArmPivot != null) m_PlayerArmLook.Rotation(base.transform, m_ArmPivot);
    }

    public void UpdateAnimationRotations()
    {
        m_PlayerLook.Rotation(m_AnimationContainer, m_HeadContainer);
    }

    public void UpdateAnimations()
    {
        InternalUpdateAnimations();
    }

    private void InternalUpdateAnimations()
    {
        float strafeX = m_PlayerMovement.IsRunning
            ? m_PlayerMovement.MovementInput.x * 2f
            : m_PlayerMovement.MovementInput.x;
        float forwardY = m_PlayerMovement.IsRunning
            ? m_PlayerMovement.MovementInput.y * 2f
            : m_PlayerMovement.MovementInput.y;

        if (Vector3.Distance(m_PlayerMovement.CurrentPosition, m_PlayerMovement.PreviousPosition) <= 0.01f)
        {
            if (forwardY != 0f) forwardY = 0f;
            if (strafeX != 0f) strafeX = 0f;
        }

        if (m_PlayerMovement.IsSlowed)
        {
            if (strafeX != 0f) strafeX = m_PlayerMovement.MovementInput.x;
            if (forwardY != 0f) forwardY = m_PlayerMovement.MovementInput.y;
            float reduce = 0.25f;
            if (strafeX > 0f) strafeX -= reduce;
            else if (strafeX < 0f) strafeX += reduce;
            if (forwardY > 0f) forwardY -= reduce;
            else if (forwardY < 0f) forwardY += reduce;
        }

        if (!IsGrounded && !m_PlayerMovement.PreviouslyGrounded && !m_InternalJump)
        {
            Vector3 end = base.transform.position + Vector3.down * 0.3f;
            if (!Physics.Linecast(base.transform.position, end, out var _, QueryTriggerInteraction.Ignore))
                m_InternalJump = true;
        }
        else if (IsGrounded && m_PlayerMovement.PreviouslyGrounded && m_InternalJump)
        {
            m_InternalJump = false;
            FPCameraEffects.ShakeRotation(0.2f, 0.8f, 2, 90f, fadeOut: true, vibrate: false);
        }

        if (IsGrounded && m_PlayerMovement.IsCrouched && !m_InternalCrouch)
        {
            m_InternalCrouch = true;
            float h = CharacterController.height;
            m_CrouchSequence?.Kill();
            m_CrouchSequence = DOTween.Sequence();
            m_CrouchSequence.Insert(0f, DOTween.To(() => h, v => { h = v; }, 4.5f, 0.5f)
                .SetUpdate(UpdateType.Fixed).SetEase(Ease.InOutSine)
                .OnUpdate(() => UpdateCollider(h)));
            m_CrouchSequence.OnComplete(() => { UpdateCollider(); m_CrouchSequence = null; });
        }
        else if (IsGrounded && !m_PlayerMovement.IsCrouched && m_InternalCrouch)
        {
            if (!Physics.SphereCast(base.transform.position, CharacterController.radius, Vector3.up, out var _,
                6.5f - CharacterController.radius, ~(1 << LayerMask.NameToLayer("Player")), QueryTriggerInteraction.Ignore))
            {
                m_InternalCrouch = false;
                float h2 = CharacterController.height;
                m_CrouchSequence?.Kill();
                m_CrouchSequence = DOTween.Sequence();
                m_CrouchSequence.Insert(0f, DOTween.To(() => h2, v => { h2 = v; }, 6.5f, 0.25f)
                    .SetUpdate(UpdateType.Fixed).SetEase(Ease.InOutSine)
                    .OnUpdate(() => UpdateCollider(h2)));
                m_CrouchSequence.OnComplete(() => { UpdateCollider(); m_CrouchSequence = null; });
            }
            else
            {
                m_PlayerMovement.SetCrouchInput(true);
            }
        }

        if (forwardY > 1f) SetAnimationMovementState(2f);
        else if (forwardY > 0f) SetAnimationMovementState(1f);
        else if (forwardY < 0f) SetAnimationMovementState(-1f);
        else SetAnimationMovementState(0f);

        SetAnimationCrouchState(IsGrounded ? (m_PlayerMovement.IsCrouched ? 1f : 0f) : -1f);

        if (strafeX == 0f && forwardY == 0f) SetAnimationShimmySpeed(m_ShimmyX);
        else SetAnimationShimmySpeed(0f);

        SetAnimationStrafeSpeed(strafeX);
        SetAnimationSpeed(forwardY);
        SetAnimationCrouchSpeed(forwardY);
    }

    private void GetHeadTracker()
    {
        m_InternalTracker.transform.position = m_HeadBobTracker.position;
        bool smooth = m_Settings != null && m_Settings.SmoothCamera;

        if (m_CanCameraPivotLerp)
        {
            if (smooth)
            {
                Vector3 local = m_InternalTracker.transform.localPosition;
                Vector3 pivot = m_CameraPivot.localPosition;
                pivot.x = Mathf.Lerp(pivot.x, local.x, 5f * Time.deltaTime);
                pivot.y = Mathf.Lerp(pivot.y, local.y, 5f * Time.deltaTime);
                pivot.z = Mathf.Lerp(pivot.z, local.z, 5f * Time.deltaTime);
                m_CameraPivot.localPosition = pivot;

                if (Camera != null && Camera.ArmsContainer != null)
                {
                    Vector3 armsPos = Camera.ArmsContainer.localPosition;
                    float a = Mathf.Lerp(m_CameraPivot.localPosition.x * 1.5f, local.x * 1.5f, 1.5f * Time.deltaTime);
                    armsPos.x = -a;
                    Camera.ArmsContainer.localPosition = armsPos;
                }
            }
            else
            {
                m_CameraPivot.localPosition = m_InternalTracker.transform.localPosition;
            }
        }
        else
        {
            m_CameraPivot.localPosition = m_InternalTracker.transform.localPosition;
        }

        if (m_CanHeadContainerSlerp)
        {
            if (m_HeadContainer.localPosition != Vector3.zero)
                m_HeadContainer.localPosition = Vector3.Slerp(m_HeadContainer.localPosition, Vector3.zero, 5f * Time.deltaTime);
            if (m_HeadContainer.localRotation != Quaternion.identity)
                m_HeadContainer.localRotation = Quaternion.Slerp(m_HeadContainer.localRotation, Quaternion.identity, 5f * Time.deltaTime);
        }
    }

    public void SetInitialLocation(Transform t)
    {
        base.transform.position = t.position;
        base.transform.eulerAngles = t.eulerAngles;
        ForcePlayerRotation(t.rotation);
        ResetRotation();
    }

    public void SetInitialLocation(FPPlayerTransform pt)
    {
        base.transform.position = pt.Position;
        base.transform.eulerAngles = pt.Rotation;
        ResetRotation();
        ForceRotation(Quaternion.Euler(pt.Rotation), Quaternion.Euler(pt.HeadRotation));
    }

    public void ResetRotation()
    {
        m_PlayerLook.Initialize(base.transform, m_HeadContainer);
        if (m_ArmPivot != null) m_PlayerArmLook.Initialize(base.transform, m_ArmPivot);
    }

    public void ForceRotation(Quaternion playerRotation, Quaternion cameraRotation)
    {
        ForcePlayerRotation(playerRotation);
        ForceCameraRotation(cameraRotation);
    }

    public void ForcePlayerRotation(Quaternion q) => m_PlayerLook.ForceRotation(q);
    public void ForceCameraRotation(Quaternion q) => m_PlayerLook.ForceCameraRotation(q);

    public void LockRotation(float x, float y)
    {
        m_PlayerLook.HorizontalClampSetActive(true);
        m_PlayerLook.SetHorizontalClamp(x);
        m_PlayerLook.SetVerticalClamp(y);
    }

    public void UnlockRotation()
    {
        m_PlayerLook.HorizontalClampSetActive(false);
        m_PlayerLook.ResetVerticalClamp();
    }

    public void AddForce(Vector3 force) => m_PlayerMovement.AddForce(force);
    public void CancelMovement() => m_PlayerMovement.CancelMovement();
    public void SetInteraction(bool active) => m_PlayerInteraction.SetActive(active);

    public void SetCollision(bool active)
    {
        CharacterController.enabled = active;
        Collider.enabled = active;
    }

    public void SetCameraPivotLerp(bool active) => m_CanCameraPivotLerp = active;
    public void SetHeadContainerSlerp(bool active) => m_CanHeadContainerSlerp = active;

    public void ResetAnimation()
    {
        SetAnimationMovementState(0f);
        SetAnimationSpeed(0f);
        SetAnimationStrafeSpeed(0f);
        SetAnimationCrouchSpeed(0f);
        m_ShimmyX = 0f;
    }

    public void ForceResetAnimation()
    {
        AnimatorBody?.SetMovementState(0f, smooth: false);
        AnimatorBody?.SetMovementSpeed(0f, smooth: false);
        AnimatorBody?.SetStrafeSpeed(0f, smooth: false);
        AnimatorBody?.SetCrouchSpeed(0f, smooth: false);
        m_ShimmyX = 0f;
    }

    public void SetAnimationInt(string name, int value)
    {
        if (name == "") return;
        AnimatorBody?.SetInteger(name, value);
    }

    public void SetAnimationTrigger(string trigger)
    {
        SetBodyAnimationTrigger(trigger);
        SetArmAnimationTrigger(trigger);
    }

    public void ResetAnimationTrigger(string trigger)
    {
        if (trigger == "") return;
        AnimatorBody?.ResetTrigger(trigger);
        AnimatorArms?.ResetTrigger(trigger);
    }

    public void SetBodyAnimationTrigger(string trigger)
    {
        if (trigger != "") AnimatorBody?.SetTrigger(trigger);
    }

    public void SetArmAnimationTrigger(string trigger)
    {
        if (trigger != "") AnimatorArms?.SetTrigger(trigger);
    }

    public void SetAnimationType(string name, int value) => AnimatorBody?.SetInteger(name, value);

    private void SetAnimationMovementState(float state) => AnimatorBody?.SetMovementState(state);
    private void SetAnimationCrouchState(float state) => AnimatorBody?.SetCrouchState(state, smooth: true, 0.3f);
    private void SetAnimationShimmySpeed(float speed, bool smooth = true) => AnimatorBody?.SetShimmySpeed(speed, smooth);
    private void SetAnimationSpeed(float speed) => AnimatorBody?.SetMovementSpeed(speed, speed == 0f);
    private void SetAnimationStrafeSpeed(float speed) => AnimatorBody?.SetStrafeSpeed(speed, speed == 0f);
    private void SetAnimationCrouchSpeed(float speed) => AnimatorBody?.SetCrouchSpeed(speed, speed == 0f);

    private void SetAnimationLayerWeight(int layer, float weight)
    {
        AnimatorBody?.SetLayerWeight(layer, weight);
        AnimatorArms?.SetLayerWeight(layer, weight);
    }

    private void EnableAllTrackers() => SetAllTrackers(true);
    private void DisableAllTrackers() => SetAllTrackers(false);

    public void SetHeadTracker(bool active) => m_HeadTracker.SetActive(active);

    public void SetAllTrackers(bool active)
    {
        m_HeadTracker.SetActive(active);
        SetAnimationLayerWeight(1, 0f);
        SetAnimationLayerWeight(2, 0f);
    }

    public void ShowFirstPersonArms()
    {
        Camera?.SetFirstPersonArmsActive(true);
        ModelLayers?.EnableFirstPerson();
    }

    public void HideFirstPersonArms()
    {
        Camera?.SetFirstPersonArmsActive(false);
        ModelLayers?.EnableAll();
    }

    public void EnableAnimationRotation(float x = 15f, float y = 20f, bool isCutscene = false)
    {
        FPState.Player state = isCutscene ? FPState.Player.CutscenePeek : FPState.Player.Peek;
        SetState(state);
        m_PlayerLook.Initialize(m_AnimationContainer, m_HeadContainer);
        LockRotation(x, y);
    }

    public void EnterInteraction(string trigger)
    {
        SetState(FPState.Player.Cutscene);
        CancelMovement();
        ResetAnimation();
        ModelLayers?.EnableCutscene();
        SetHeadTracker(false);
        HideFirstPersonArms();
        SetBodyAnimationTrigger(trigger);
        DOTween.Sequence().InsertCallback(0.05f, EnterInternalAnimation);
    }

    private void EnterInternalAnimation()
    {
        m_HeadContainer.SetParent(m_AnimationContainer);
    }

    public void EnterInteractionInstant(string trigger)
    {
        SetState(FPState.Player.Cutscene);
        CancelMovement();
        ForceResetAnimation();
        ResetAnimation();
        ModelLayers?.EnableCutscene();
        SetHeadTracker(false);
        if (trigger.ToLower().Contains("instant"))
        {
            SetAnimationLayerWeight(1, 0f);
            SetAnimationLayerWeight(2, 0f);
        }
        HideFirstPersonArms();
        SetBodyAnimationTrigger(trigger);
        m_HeadContainer.SetParent(m_AnimationContainer);
        m_HeadContainer.localPosition = Vector3.zero;
        m_HeadContainer.localEulerAngles = Vector3.zero;
    }

    public void ExitInteraction()
    {
        SetState(FPState.Player.Cutscene);
        SetAnimationTrigger("ExitInteraction");
        if (gameObject.scene.buildIndex != -1 && gameObject.transform.parent == null)
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
        UnlockRotation();
    }

    public void ExitAnimation()
    {
        AnimationComplete();
        ResetRotation();
        DOTween.Sequence().InsertCallback(0.05f, ExitInternalAnimation);
    }

    private void ExitInternalAnimation()
    {
        EnableAllTrackers();
        ShowFirstPersonArms();
        ExitAnimationCamera();
        ResetRotation();
        SetState(FPState.Player.Default);
    }

    public void ExitAnimationCamera()
    {
        m_HeadContainer.SetParent(m_CameraPivot);
        m_AnimationContainer.localPosition = Vector3.zero;
        m_AnimationContainer.localEulerAngles = Vector3.zero;
    }

    public void EnterCutscene(AnimationClip animationClip)
    {
        animationClip.name = "Cutscene";
        PlayerContent?.UpdateClipOverrides(animationClip);
        EnterInteraction("Cutscene");
    }

    public void AnimationInteract() => this.OnAnimationInteract.Send(this);
    public void AnimationComplete() => this.OnAnimationComplete.Send(this);

    public void AnimationClear()
    {
        m_AnimationContainer.DOLocalMove(m_HeadBobTracker.localPosition, 0.25f).SetEase(Ease.InOutSine);
    }

    public void SlideToLocation(Transform location, float duration = 0.25f, Ease ease = Ease.InOutSine)
    {
        SlideToLocation(location.position, location.eulerAngles, duration, ease);
    }

    public void SlideToLocation(Vector3 position, Vector3 rotation, float duration = 0.25f, Ease ease = Ease.InOutSine)
    {
        base.transform.DOMove(position, duration).SetEase(ease);
        base.transform.DORotate(rotation, duration).SetEase(ease).OnComplete(() =>
        {
            Camera?.SetFirstPersonArmsActive(false);
        });
    }

    public void ForceStand()
    {
        m_CrouchSequence?.Kill();
        AnimatorBody?.SetCrouchState(0f, smooth: false);
        SetAnimationShimmySpeed(0f, false);
        m_InternalCrouch = false;
        m_PlayerMovement.ForceStand();
        UpdateCollider(6.5f);
    }

    public void ForceCrouch(bool isInternalCrouch = true)
    {
        m_CrouchSequence?.Kill();
        AnimatorBody?.SetCrouchState(1f, smooth: false);
        SetAnimationShimmySpeed(0f, false);
        m_InternalCrouch = isInternalCrouch;
        if (isInternalCrouch)
        {
            m_PlayerMovement.ForceCrouch();
            UpdateCollider(4.5f);
        }
    }

    protected override void OnDisposed()
    {
        this.OnAnimationComplete = null;
        this.OnAnimationInteract = null;
        m_CrouchSequence?.Kill();
        m_CrouchSequence = null;
        CharacterController = null;
        base.OnDisposed();
    }
}
