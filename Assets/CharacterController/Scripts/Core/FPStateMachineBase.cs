using System;
using System.Collections.Generic;

public abstract class FPStateMachineBase<TActor, TIdentifier> : FPMonoBehaviour where TIdentifier : Enum
{
    private Dictionary<TIdentifier, FPStateBase<TActor, TIdentifier>> m_ActiveStates
        = new Dictionary<TIdentifier, FPStateBase<TActor, TIdentifier>>();

    private bool m_IsActive;

    protected FPStateBase<TActor, TIdentifier> m_State { get; private set; }
    protected TIdentifier m_PreviousState { get; private set; }
    protected TIdentifier m_NextState { get; private set; }

    protected bool IsActive => m_IsActive && m_State != null;
    protected virtual bool UseAwake => true;

    public bool IsInitialized { get; private set; }
    public TIdentifier CurrentState { get; private set; }
    public bool Disabled { get; private set; }

    public FPStateMachineBase() => SetActive(true);

    protected void SetActive(bool active) => m_IsActive = active;

    public sealed override void Awake()
    {
        if (UseAwake) Initialize();
    }

    public void Initialize()
    {
        if (IsInitialized) return;
        InternalInitialize();
        if (!UseAwake)
        {
            InitializeOnComplete();
            IsInitialized = true;
        }
    }

    protected virtual void InternalInitialize() { }

    public sealed override void Start()
    {
        if (UseAwake) InitializeOnComplete();
    }

    private void InitializeOnComplete()
    {
        if (!IsInitialized) InternalInitializeOnComplete();
        if (UseAwake) IsInitialized = true;
    }

    protected virtual void InternalInitializeOnComplete() { }

    protected virtual void Update()
    {
        GetActions(() => m_State?.Update());
    }

    protected virtual void FixedUpdate()
    {
        GetActions(() => m_State?.FixedUpdate());
    }

    protected virtual void LateUpdate()
    {
        GetActions(() => m_State?.LateUpdate());
    }

    protected void GetActions(params Action[] actions)
    {
        if (IsDisposed || !enabled || !gameObject.activeSelf || !m_IsActive || FPPause.IsPaused || actions == null) return;
        foreach (var a in actions) a?.Invoke();
    }

    public void SetDisable(bool disable) => Disabled = disable;

    public void SetState(TIdentifier state)
    {
        if (m_State != null && m_State.ID.Equals(state)) return;

        string typeName = typeof(TIdentifier).ToString();
        typeName = typeName.FPRemoveSymbols().Replace("State", "") + "State" + state.ToString();

        if (!m_ActiveStates.ContainsKey(state))
            m_ActiveStates.Add(state, (FPStateBase<TActor, TIdentifier>)Activator.CreateInstance(Type.GetType(typeName), this, state));

        if (m_State != null) m_PreviousState = m_State.ID;
        CurrentState = state;
        m_State?.OnStateExit();
        m_State = m_ActiveStates[state];
        m_State?.OnStateEnter();
    }

    public void SetPreviousState()
    {
        if (m_State == null || m_PreviousState == null || m_PreviousState.Equals(m_State.ID)) return;
        var id = m_State.ID;
        SetState(m_PreviousState);
        m_PreviousState = id;
    }

    public void SetPreviousState(TIdentifier state) => m_PreviousState = state;
    public void SetNextState() { if (!m_NextState.Equals(m_State.ID)) { SetState(m_NextState); m_NextState = CurrentState; } }
    public void SetNextState(TIdentifier state) => m_NextState = state;

    protected override void OnDisposed()
    {
        m_State = null;
        m_ActiveStates?.Clear();
        m_ActiveStates = null;
        base.OnDisposed();
    }
}
