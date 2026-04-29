using System;
using UnityEngine;

// Standalone base class for all interactable world objects.
// Rewritten without ActionEvent inheritance — no game-specific data IDs, no rumble, no GameManager.
[DefaultExecutionOrder(100)]
public class FPInteractable : FPMonoBehaviour
{
    [Header("Interact Options")]
    [SerializeField] protected FPInputType m_InputType = FPInputType.InteractOnReleased;
    [SerializeField] protected bool m_IsActive = true;
    [SerializeField] protected bool m_IsSingleAction = true;
    [SerializeField] protected bool m_DisposeOnAction = true;

    public FPInputType InputType => m_InputType;
    public bool IsActive => m_IsActive;
    public bool IsActioned { get; private set; }

    public event EventHandler OnEnter;
    public event EventHandler OnExit;
    public event EventHandler OnInteract;

    public virtual void SetActive(bool active) => m_IsActive = active;
    public void ResetAction() => IsActioned = false;

    // Called by FPInteraction when the crosshair first lands on this object.
    public bool Enter(Vector3 origin, RaycastHit hit, object sender = null)
    {
        if (!m_IsActive || IsActioned || !InternalEnterCheck(origin, hit, sender)) return false;
        IsActioned = false;
        OnInternalEnter(origin, hit, sender);
        this.OnEnter.Send(this);
        return true;
    }

    public bool EnterCheck(Vector3 origin, RaycastHit hit, object sender = null)
        => InternalEnterCheck(origin, hit, sender);

    protected virtual bool InternalEnterCheck(Vector3 origin, RaycastHit hit, object sender = null) => true;
    protected virtual void OnInternalEnter(Vector3 origin, RaycastHit hit, object sender = null) { }

    // Called by FPInteraction when the crosshair leaves.
    public bool Exit(Vector3 origin, RaycastHit hit, object sender = null)
    {
        if (!m_IsActive || IsActioned || !InternalExitCheck(origin, hit, sender)) return false;
        OnInternalExit(origin, hit, sender);
        this.OnExit.Send(this);
        return true;
    }

    protected virtual bool InternalExitCheck(Vector3 origin, RaycastHit hit, object sender = null) => true;
    protected virtual void OnInternalExit(Vector3 origin, RaycastHit hit, object sender = null) { }

    // Called by FPInteraction on the appropriate input event.
    public bool Interact(Vector3 origin, RaycastHit hit, object sender = null)
    {
        if (!m_IsActive || IsActioned || !InternalInteractCheck(origin, hit, sender)) return false;
        IsActioned = true;
        OnInternalInteract(origin, hit, sender);
        this.OnInteract.Send(this);
        if (m_IsSingleAction) Clear();
        return true;
    }

    protected virtual bool InternalInteractCheck(Vector3 origin, RaycastHit hit, object sender = null) => true;
    protected virtual void OnInternalInteract(Vector3 origin, RaycastHit hit, object sender = null) { }

    protected void Clear()
    {
        IsActioned = true;
        if (m_DisposeOnAction && InternalClear()) Dispose();
    }

    protected virtual bool InternalClear() => true;

    protected override void OnDisposed()
    {
        OnInteract = null;
        OnEnter = null;
        OnExit = null;
        base.OnDisposed();
    }
}
