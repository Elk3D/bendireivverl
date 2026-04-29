using System;
using UnityEngine;

[Serializable]
public class FPInteraction : FPDisposable
{
    [SerializeField] private bool m_IsActive = true;
    [SerializeField] private LayerMask m_IgnoreLayers;
    [SerializeField] private float m_LookDistance = 7f;
    [SerializeField] private float m_SphereCastThickness = 0.25f;

    public bool IsActive => m_IsActive;
    public RaycastHit Hit { get; private set; }
    public FPInteractable Interactable { get; private set; }

    public void ResetInteraction()
    {
        if (Interactable != null) ExitInteraction(Interactable.transform.position);
        Interactable = null;
    }

    public void Update(Transform sender)
    {
        if (m_IsActive && !IsDisposed && !FPPause.IsPaused)
            InternalUpdate(sender.position, sender.forward, m_LookDistance);
    }

    private void InternalUpdate(Vector3 origin, Vector3 direction, float distance)
    {
        if (Physics.SphereCast(origin, m_SphereCastThickness, direction, out var hitInfo, distance, ~(int)m_IgnoreLayers))
        {
            Hit = hitInfo;
            var interactable = Hit.transform.root.GetComponent<FPInteractable>()
                ?? Hit.transform.GetComponent<FPInteractable>()
                ?? Hit.transform.GetComponentInParent<FPInteractable>();

            if (interactable != null)
            {
                if (Interactable != interactable)
                {
                    ExitInteraction(origin);
                    Interactable = interactable;
                    EnterInteraction(origin);
                }
                else if ((Interactable.InputType == FPInputType.InteractOnPressed || Interactable.InputType == FPInputType.InteractOnReleased)
                         && FPInput.InteractOnReleased())
                    Interact(origin);
                else if (!interactable.EnterCheck(origin, Hit))
                    ExitInteraction(origin);
                return;
            }
        }
        ExitInteraction(origin);
    }

    private void Interact(Vector3 origin) => Interactable?.Interact(origin, Hit, this);

    private void EnterInteraction(Vector3 origin)
    {
        if (Interactable == null || !Interactable.Enter(origin, Hit, this)) ExitInteraction(origin);
    }

    private void ExitInteraction(Vector3 origin)
    {
        Interactable?.Exit(origin, Hit, this);
        Interactable = null;
    }

    public void SetActive(bool active) => m_IsActive = active;

    protected override void OnDisposed() { Interactable = null; base.OnDisposed(); }
}
