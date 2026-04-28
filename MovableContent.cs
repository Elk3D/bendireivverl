using System;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class MovableContent : ActionEventContent<MovableContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public MovableEnter EnterFront;

		public MovableEnter EnterBack;
	}

	[Header("S13Content")]
	[SerializeField]
	private S13ActionGroupReference onStepAction;

	[SerializeField]
	private S13ActionGroupReference onReleaseCartAction;

	[SerializeField]
	public ControllerRumble.RUMBLE_PRESETS m_presetRumble = ControllerRumble.RUMBLE_PRESETS.Cart_Drag;

	private Movable m_Movable;

	private MovableLocation[] m_MovableLocations;

	private MovableLocation m_MovableStartLocation;

	private MovableEnter m_InteractedEnter;

	private Sequence m_Sequence;

	private bool m_CanMove;

	private bool m_IsMoving;

	private bool m_IsForward = true;

	public event EventHandler OnExit;

	public event EventHandler OnFrontEnter;

	public event EventHandler OnBackEnter;

	protected override void OnInitialize()
	{
		m_Movable = (Movable)base.Connectable;
		for (int i = 0; i < m_Properties.Length; i++)
		{
			Properties properties = m_Properties[i];
			if (properties.EnterFront != null)
			{
				properties.EnterFront.OnEnter -= HandleFrontOnEnter;
				properties.EnterFront.OnEnter += HandleFrontOnEnter;
			}
			if (properties.EnterBack != null)
			{
				properties.EnterBack.OnEnter -= HandleBackOnEnter;
				properties.EnterBack.OnEnter += HandleBackOnEnter;
			}
		}
	}

	public void SetStartLocation(MovableLocation[] path, MovableLocation startLocation)
	{
		m_MovableLocations = path;
		m_MovableStartLocation = startLocation;
		for (int i = 0; i < m_MovableLocations.Length; i++)
		{
			if (m_MovableLocations[i] == m_MovableStartLocation)
			{
				m_Movable.Data.SetCurrentIndex(i);
				break;
			}
		}
		base.transform.position = m_MovableLocations[m_Movable.Data.CurrentIndex].transform.position;
		base.transform.eulerAngles = m_MovableLocations[m_Movable.Data.CurrentIndex].transform.eulerAngles;
	}

	protected override void InternalEnable()
	{
		for (int i = 0; i < m_Properties.Length; i++)
		{
			Properties properties = m_Properties[i];
			if (properties.EnterFront != null)
			{
				properties.EnterFront.gameObject.SetActive(value: true);
			}
			if (properties.EnterBack != null)
			{
				properties.EnterBack.gameObject.SetActive(value: true);
			}
		}
	}

	protected override void InternalDisable()
	{
		for (int i = 0; i < m_Properties.Length; i++)
		{
			Properties properties = m_Properties[i];
			if (properties.EnterFront != null)
			{
				properties.EnterFront.gameObject.SetActive(value: false);
			}
			if (properties.EnterBack != null)
			{
				properties.EnterBack.gameObject.SetActive(value: false);
			}
		}
	}

	private void Update()
	{
		if (!m_CanMove || GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (!m_IsMoving)
		{
			if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat)
			{
				Exit();
			}
			else if (PlayerInput.InteractOnReleased())
			{
				Exit();
			}
			else if (PlayerInput.MoveY() > 0f)
			{
				MoveForward();
			}
			else if (PlayerInput.MoveY() < 0f)
			{
				MoveBack();
			}
			if (GameManager.Instance.Player.BattleStatus == BattleStatus.Moving)
			{
				GameManager.Instance.Player.SetBattleStatus(BattleStatus.None);
			}
		}
		else if (GameManager.Instance.Player.BattleStatus == BattleStatus.None)
		{
			GameManager.Instance.Player.SetBattleStatus(BattleStatus.Moving);
		}
	}

	private void Exit()
	{
		m_CanMove = false;
		GameManager.Instance.Player.transform.SetParent(null);
		GameManager.Instance.Player.OnAnimationComplete -= HandleExitOnAnimationComplete;
		GameManager.Instance.Player.OnAnimationComplete += HandleExitOnAnimationComplete;
		GameManager.Instance.Player.ExitInteraction();
		ResetSequence();
		m_Sequence.Insert(0f, GameManager.Instance.Player.AnimationContainer.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.InOutSine));
		onReleaseCartAction?.Execute();
	}

	private void MoveForward()
	{
		if (m_IsForward)
		{
			if (m_Movable.Data.CurrentIndex >= m_MovableLocations.Length - 1)
			{
				return;
			}
			CheckForward();
		}
		else
		{
			if (m_Movable.Data.CurrentIndex <= 0)
			{
				return;
			}
			CheckBack();
		}
		DOMove();
		onStepAction?.Execute();
	}

	private void MoveBack()
	{
		if (m_IsForward)
		{
			if (m_Movable.Data.CurrentIndex <= 0)
			{
				return;
			}
			CheckBack();
		}
		else
		{
			if (m_Movable.Data.CurrentIndex >= m_MovableLocations.Length - 1)
			{
				return;
			}
			CheckForward();
		}
		DOMove(isForward: false);
		onStepAction?.Execute();
	}

	private void CheckForward()
	{
		m_IsMoving = true;
		m_Movable.Data.SetCurrentIndex(m_Movable.Data.CurrentIndex + 1);
	}

	private void CheckBack()
	{
		m_IsMoving = true;
		m_Movable.Data.SetCurrentIndex(m_Movable.Data.CurrentIndex - 1);
	}

	private void DOMove(bool isForward = true)
	{
		string animationTrigger = (isForward ? "CartForward" : "CartBack");
		GameManager.Instance.Player.SetAnimationTrigger(animationTrigger);
		base.transform.DOKill();
		base.transform.DORotate(m_MovableLocations[m_Movable.Data.CurrentIndex].transform.eulerAngles, 1f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed);
		base.transform.DOMove(m_MovableLocations[m_Movable.Data.CurrentIndex].transform.position, 1f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed)
			.OnComplete(MoveOnComplete);
		if (m_presetRumble != ControllerRumble.RUMBLE_PRESETS.NONE)
		{
			GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[(int)m_presetRumble]);
		}
	}

	private void HandleFrontOnEnter(object sender, EventArgs e)
	{
		Enter((MovableEnter)sender);
		this.OnFrontEnter.Send(this);
	}

	private void HandleBackOnEnter(object sender, EventArgs e)
	{
		Enter((MovableEnter)sender, isFront: false);
		this.OnBackEnter.Send(this);
	}

	private void Enter(MovableEnter interacted, bool isFront = true)
	{
		m_InteractedEnter = interacted;
		m_CanMove = true;
		m_IsForward = isFront;
		GameManager.Instance.Player.transform.SetParent(base.transform);
		GameManager.Instance.Player.S13SetCartMove(isMoving: true);
	}

	public void ForceEnterFront(MovableDataObject dataObject)
	{
		PrepForceEnter();
		for (int i = 0; i < m_Properties.Length; i++)
		{
			Properties properties = m_Properties[i];
			if (properties.EnterFront != null && dataObject.Side == 0)
			{
				properties.EnterFront.ForceEnter(dataObject.RotationX, dataObject.RotationY, dataObject.Side, delegate
				{
					ForceEnter(properties.EnterFront, isFront: true);
				}, isLadder: false);
				break;
			}
		}
	}

	public void ForceEnterBack(MovableDataObject dataObject)
	{
		PrepForceEnter();
		for (int i = 0; i < m_Properties.Length; i++)
		{
			Properties properties = m_Properties[i];
			if (properties.EnterBack != null && dataObject.Side == 1)
			{
				properties.EnterBack.ForceEnter(dataObject.RotationX, dataObject.RotationY, dataObject.Side, delegate
				{
					ForceEnter(properties.EnterBack, isFront: false);
				}, isLadder: false);
				break;
			}
		}
	}

	private void PrepForceEnter()
	{
		SetStartLocation(m_Movable.Path.Path, m_Movable.Path.Path[m_Movable.Data.CurrentIndex]);
		GameManager.Instance.Player.transform.SetParent(base.transform);
	}

	private void ForceEnter(MovableEnter interacted, bool isFront)
	{
		GameManager.Instance.HideInteraction();
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.DisableTeleport();
		}
		m_InteractedEnter = interacted;
		m_CanMove = true;
		m_IsForward = isFront;
		GameManager.Instance.Player.S13SetCartMove(isMoving: true);
	}

	private void HandleExitOnAnimationComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnAnimationComplete -= HandleExitOnAnimationComplete;
		if (!m_InteractedEnter.IsSingleInteraction)
		{
			m_InteractedEnter.Interactable?.ResetAction();
			m_InteractedEnter.SetActive(active: true);
		}
		m_InteractedEnter.SendOnExit();
		this.OnExit.Send(this);
		if (m_InteractedEnter.HideCrosshair)
		{
			GameManager.Instance.ShowCrosshair();
		}
		GameManager.Instance.Player.ExitAnimation();
		GameManager.Instance.Player.S13SetCartMove(isMoving: false);
	}

	private void MoveOnComplete()
	{
		m_IsMoving = false;
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		m_Sequence?.Kill();
		m_Sequence = null;
	}

	protected override void OnDisposed()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnAnimationComplete -= HandleExitOnAnimationComplete;
		}
		for (int i = 0; i < m_Properties.Length; i++)
		{
			Properties properties = m_Properties[i];
			if (properties.EnterFront != null)
			{
				properties.EnterFront.OnEnter -= HandleFrontOnEnter;
			}
			if (properties.EnterBack != null)
			{
				properties.EnterBack.OnEnter -= HandleBackOnEnter;
			}
		}
		this.OnExit = null;
		this.OnFrontEnter = null;
		this.OnBackEnter = null;
		KillSequence();
		base.OnDisposed();
	}
}
