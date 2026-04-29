using System;
using UnityEngine;

namespace CharacterController
{
    // Raycast-based interaction detector.  Call Update(senderTransform) every frame.
    // The sender transform should be the camera / head — not the character root —
    // so the sphere-cast fires from the player's eye position.
    //
    // Stripped from original:
    //   - GameManager.Instance.IsPaused guard (disable via SetActive or stop calling Update)
    //   - SpecialAction / SpecialActionReleased input branches (ability-specific)
    [Serializable]
    public class FPSInteraction : FPSDisposable
    {
        [SerializeField] private bool      m_IsActive          = true;
        [SerializeField] private LayerMask m_IgnoreLayers;
        [SerializeField] private float     m_LookDistance      = 7f;
        [SerializeField] private float     m_SphereCastThickness = 0.25f;

        public bool IsActive => m_IsActive;
        public RaycastHit Hit { get; private set; }
        public FPSInteractable Interactable { get; private set; }

        public void ResetInteraction()
        {
            if (Interactable != null)
                ExitInteraction(Interactable.transform.position);
            Interactable = null;
        }

        // Call every Update frame, passing the camera/head transform.
        public void Update(Transform sender)
        {
            if (m_IsActive && !IsDisposed)
                InternalUpdate(sender.position, sender.forward, m_LookDistance);
        }

        private void InternalUpdate(Vector3 origin, Vector3 direction, float distance)
        {
            if (Physics.SphereCast(origin, m_SphereCastThickness, direction, out var hitInfo, distance, ~(int)m_IgnoreLayers))
            {
                Hit = hitInfo;

                // Check root first, then direct, then parent chain.
                FPSInteractable interactable = Hit.transform.GetComponent<FPSInteractable>();
                if (interactable == null)
                {
                    FPSInteractable rootComp = Hit.transform.root.GetComponent<FPSInteractable>();
                    interactable = rootComp ?? Hit.transform.GetComponentInParent<FPSInteractable>();
                }

                if (interactable != null)
                {
                    if (Interactable != interactable)
                    {
                        ExitInteraction(origin);
                        Interactable = interactable;
                        EnterInteraction(origin);
                    }
                    else if ((Interactable.PlayerInputType == FPSInputType.InteractOnPressed ||
                              Interactable.PlayerInputType == FPSInputType.InteractOnReleased) &&
                             FPSInput.InteractOnReleased())
                    {
                        TriggerInteract(origin);
                    }
                    else if (!interactable.EnterCheck(origin, Hit))
                    {
                        ExitInteraction(origin);
                    }
                    return;
                }
            }

            ExitInteraction(origin);
        }

        private void TriggerInteract(Vector3 origin) => Interactable?.Interact(origin, Hit, this);

        private void EnterInteraction(Vector3 origin)
        {
            if (Interactable != null && !Interactable.Enter(origin, Hit, this))
                ExitInteraction(origin);
        }

        private void ExitInteraction(Vector3 origin)
        {
            Interactable?.Exit(origin, Hit, this);
            Interactable = null;
        }

        public void SetActive(bool active) => m_IsActive = active;

        protected override void OnDisposed()
        {
            Interactable = null;
            base.OnDisposed();
        }
    }
}
