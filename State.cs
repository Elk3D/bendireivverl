using System;

[Serializable]
public class State
{
	public enum Character
	{
		Alert,
		Attack,
		Companion,
		Controllable,
		Cutscene,
		Death,
		Evade,
		Flee,
		Follow,
		Hit,
		Idle,
		Patrol,
		Seasonal,
		SeasonalHit,
		Stun
	}

	public enum Player
	{
		Ability,
		Cutscene,
		CutscenePeek,
		Default,
		Peek
	}

	public enum PlayerAbility
	{
		Banish,
		FastTravel,
		Flow
	}
}
public abstract class State<TActor, TIdentifier> : JDisposable
{
	private bool m_IsActive;

	protected TActor Actor { get; private set; }

	public TIdentifier ID { get; protected set; }

	public bool IsActive
	{
		get
		{
			if (Actor != null)
			{
				return m_IsActive;
			}
			return false;
		}
	}

	public State(TActor actor)
	{
		Actor = actor;
		SetActive(active: true);
	}

	public abstract void OnStateEnter();

	public abstract void OnStateExit();

	public abstract void Update();

	public abstract void FixedUpdate();

	public abstract void LateUpdate();

	public void Enable()
	{
		SetActive(active: true);
	}

	public void Disable()
	{
		SetActive(active: false);
	}

	protected void SetActive(bool active)
	{
		m_IsActive = active;
	}

	protected void GetActions(params Action[] actions)
	{
		if (!base.IsDisposed && m_IsActive && !GameManager.Instance.IsPaused && Actor != null)
		{
			for (int i = 0; i < actions.Length; i++)
			{
				actions[i]?.Invoke();
			}
		}
	}

	protected override void OnDisposed()
	{
		Actor = default(TActor);
		base.OnDisposed();
	}
}
