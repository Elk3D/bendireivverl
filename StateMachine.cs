using System;
using System.Collections.Generic;

public abstract class StateMachine<TActor, TIdentifier> : JMonoBehaviour where TIdentifier : Enum
{
	private const string STATE_STRING = "State";

	private Dictionary<TIdentifier, State<TActor, TIdentifier>> m_ActiveStates = new Dictionary<TIdentifier, State<TActor, TIdentifier>>();

	private bool m_IsActive;

	protected bool m_GizmosSelected;

	protected State<TActor, TIdentifier> m_State { get; private set; }

	protected TIdentifier m_PreviousState { get; private set; }

	protected TIdentifier m_NextState { get; private set; }

	protected bool IsActive
	{
		get
		{
			if (m_IsActive)
			{
				return m_State != null;
			}
			return false;
		}
	}

	protected virtual bool UseAwake => true;

	public bool IsInitialized { get; private set; }

	public TIdentifier CurrentState { get; private set; }

	public bool Disabled { get; private set; }

	public bool GizmosSelected => m_GizmosSelected;

	public StateMachine()
	{
		SetActive(active: true);
	}

	protected void SetActive(bool active)
	{
		m_IsActive = active;
	}

	public sealed override void Awake()
	{
		if (UseAwake)
		{
			Initialize();
		}
	}

	public void Initialize()
	{
		if (!IsInitialized)
		{
			InternalInitialize();
			if (!UseAwake)
			{
				InitializeOnComplete();
				IsInitialized = true;
			}
		}
	}

	protected virtual void InternalInitialize()
	{
	}

	public sealed override void Start()
	{
		if (UseAwake)
		{
			InitializeOnComplete();
		}
	}

	private void InitializeOnComplete()
	{
		if (!IsInitialized)
		{
			InternalInitializeOnComplete();
		}
		if (UseAwake)
		{
			IsInitialized = true;
		}
	}

	protected virtual void InternalInitializeOnComplete()
	{
	}

	protected virtual void Update()
	{
		GetActions(delegate
		{
			m_State?.Update();
		});
	}

	protected virtual void FixedUpdate()
	{
		GetActions(delegate
		{
			m_State?.FixedUpdate();
		});
	}

	protected virtual void LateUpdate()
	{
		GetActions(delegate
		{
			m_State?.LateUpdate();
		});
	}

	protected void GetActions(params Action[] actions)
	{
		if (!base.IsDisposed && base.enabled && base.gameObject.activeSelf && m_IsActive && !GameManager.Instance.IsPaused && actions != null)
		{
			for (int i = 0; i < actions.Length; i++)
			{
				actions[i]?.Invoke();
			}
		}
	}

	public void SetDisable(bool disable)
	{
		Disabled = disable;
	}

	public void SetState(TIdentifier state)
	{
		if (m_State == null || !m_State.ID.Equals(state))
		{
			string text = typeof(TIdentifier).ToString();
			text = text.RemoveSymbols().Replace("State", "") + "State" + state.ToString();
			if (!m_ActiveStates.ContainsKey(state))
			{
				m_ActiveStates.Add(state, (State<TActor, TIdentifier>)Activator.CreateInstance(Type.GetType(text), this, state));
			}
			if (m_State != null)
			{
				m_PreviousState = m_State.ID;
			}
			CurrentState = state;
			m_State?.OnStateExit();
			m_State = m_ActiveStates[state];
			m_State?.OnStateEnter();
		}
	}

	public void SetPreviousState()
	{
		if (m_State != null && m_PreviousState != null && !m_PreviousState.Equals(m_State.ID))
		{
			TIdentifier iD = m_State.ID;
			SetState(m_PreviousState);
			m_PreviousState = iD;
		}
	}

	public void SetPreviousState(TIdentifier state)
	{
		m_PreviousState = state;
	}

	public void SetNextState()
	{
		if (!m_NextState.Equals(m_State.ID))
		{
			SetState(m_NextState);
			m_NextState = CurrentState;
		}
	}

	public void SetNextState(TIdentifier state)
	{
		m_NextState = state;
	}

	protected override void OnDisposed()
	{
		m_State = null;
		m_ActiveStates?.Clear();
		m_ActiveStates = null;
		base.OnDisposed();
	}
}
