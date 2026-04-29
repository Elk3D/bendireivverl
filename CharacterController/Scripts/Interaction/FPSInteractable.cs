using System;
using UnityEngine;

namespace CharacterController
{
    // Standalone base class for any object the player can interact with.
    //
    // Stripped from original:
    //   - ActionEvent / DataMonoBehaviour inheritance chain (game-specific)
    //   - ControllerRumble triggers (game-specific)
    //   - GameManager references
    //   - ActionEventData / ActionEventID wiring
    //
    // Usage: subclass this and override OnInternalEnter / OnInternalInteract / etc.
    // Hook up the events (OnEnter, OnExit, OnInteract) in the Inspector via
    // UnityEvent, or subscribe in code.
    public abstract class FPSInteractable : MonoBehaviour
    {
        [Header("Interact Options")]
        [SerializeField] protected FPSInputType m_PlayerInputType = FPSInputType.InteractOnReleased;
        [SerializeField] protected bool         m_IsActive        = true;
        [SerializeField] protected bool         m_IsSingleAction  = true;

        public FPSInputType PlayerInputType => m_PlayerInputType;
        public bool IsActive => m_IsActive;

        public event EventHandler OnEnter;
        public event EventHandler OnExit;
        public event EventHandler OnInteract;

        public virtual void SetActive(bool active) => m_IsActive = active;

        // ── Enter ─────────────────────────────────────────────────────────────────

        public bool Enter(Vector3 origin, RaycastHit hit, object sender = null)
        {
            if (!m_IsActive || !InternalEnterCheck(origin, hit, sender))
                return false;
            OnInternalEnter(origin, hit, sender);
            OnEnter?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public bool EnterCheck(Vector3 origin, RaycastHit hit, object sender = null)
            => InternalEnterCheck(origin, hit, sender);

        protected virtual bool InternalEnterCheck(Vector3 origin, RaycastHit hit, object sender = null) => true;
        protected virtual void OnInternalEnter(Vector3 origin, RaycastHit hit, object sender = null) { }

        // ── Exit ──────────────────────────────────────────────────────────────────

        public bool Exit(Vector3 origin, RaycastHit hit, object sender = null)
        {
            if (!m_IsActive || !InternalExitCheck(origin, hit, sender))
                return false;
            OnInternalExit(origin, hit, sender);
            OnExit?.Invoke(this, EventArgs.Empty);
            return true;
        }

        protected virtual bool InternalExitCheck(Vector3 origin, RaycastHit hit, object sender = null) => true;
        protected virtual void OnInternalExit(Vector3 origin, RaycastHit hit, object sender = null) { }

        // ── Interact ──────────────────────────────────────────────────────────────

        public void Interact(Vector3 origin, RaycastHit hit, object sender = null)
        {
            if (!m_IsActive || !InternalInteractCheck(origin, hit, sender))
                return;
            OnInternalInteract(origin, hit, sender);
            OnInteract?.Invoke(this, EventArgs.Empty);
            if (m_IsSingleAction)
                SetActive(false);
        }

        protected virtual bool InternalInteractCheck(Vector3 origin, RaycastHit hit, object sender = null) => true;
        protected virtual void OnInternalInteract(Vector3 origin, RaycastHit hit, object sender = null) { }

        protected virtual void OnDestroy()
        {
            OnInteract = null;
            OnEnter    = null;
            OnExit     = null;
        }
    }
}
