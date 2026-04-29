using System;

// Enum container — naming kept so reflection in FPStateMachineBase resolves correctly:
// typeof(FPState.Player).ToString() → "FPState+Player"
// → RemoveSymbols → "FPStatePlayer"
// → Replace("State","") → "FPPlayer"
// → + "State" + "Default" → "FPPlayerStateDefault" ✓
[Serializable]
public class FPState
{
    public enum Player
    {
        Cutscene,
        CutscenePeek,
        Default,
        Peek
    }
}

public abstract class FPStateBase<TActor, TIdentifier> : FPDisposable
{
    private bool m_IsActive;

    protected TActor Actor { get; private set; }
    public TIdentifier ID { get; protected set; }

    public bool IsActive
    {
        get { return Actor != null && m_IsActive; }
    }

    public FPStateBase(TActor actor)
    {
        Actor = actor;
        SetActive(true);
    }

    public abstract void OnStateEnter();
    public abstract void OnStateExit();
    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void LateUpdate();

    public void Enable() => SetActive(true);
    public void Disable() => SetActive(false);

    protected void SetActive(bool active) => m_IsActive = active;

    protected void GetActions(params Action[] actions)
    {
        if (IsDisposed || !m_IsActive || FPPause.IsPaused || Actor == null) return;
        foreach (var a in actions) a?.Invoke();
    }

    protected override void OnDisposed()
    {
        Actor = default;
        base.OnDisposed();
    }
}
