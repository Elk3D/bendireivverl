using System;
using DG.Tweening;
using UnityEngine;

public class LadderContent : ActionEventContent<LadderContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
	}

	private const int LADDER_MINIMUM = 3;

	private const float LADDER_HEIGHT = 1f;

	private const float LADDER_MOVE_SPEED = 0.333f;

	private LadderPart[] m_Ladders;

	private LadderBottom m_LadderBottom;

	private LadderTop m_LadderTop;

	private Ease m_TweenEase = Ease.Linear;

	private UpdateType m_TweenUpdateType = UpdateType.Fixed;

	private int m_LadderIndex;

	private int m_CurrentLadderIndex;

	private int m_LadderSide;

	private int m_ClimbDirection;

	public LadderBottom LadderBottom => m_LadderBottom;

	public LadderTop LadderTop => m_LadderTop;

	public int LadderIndex => m_LadderIndex;

	public int CurrentLadderIndex => m_CurrentLadderIndex;

	public int LadderSide => m_LadderSide;

	public int ClimbDirection => m_ClimbDirection;

	public bool IsActive { get; private set; }

	public bool IsMoving { get; private set; }

	public event EventHandler OnDeath;

	protected override void OnInitialize()
	{
		IsActive = false;
		IsMoving = false;
		m_LadderIndex = 3;
		m_Ladders = GetComponentsInChildren<LadderPart>();
		m_LadderBottom = GetComponentInChildren<LadderBottom>();
		m_LadderTop = GetComponentInChildren<LadderTop>();
		for (int i = 0; i < m_Ladders.Length; i++)
		{
			m_LadderIndex += 2;
		}
		LadderAddListeners();
	}

	private void Update()
	{
		if (!IsActive || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (IsMoving)
		{
			if (GameManager.Instance.Player.BattleStatus == BattleStatus.None)
			{
				GameManager.Instance.Player.SetBattleStatus(BattleStatus.Moving);
			}
			return;
		}
		if (GameManager.Instance.Player.BattleStatus == BattleStatus.Moving)
		{
			GameManager.Instance.Player.SetBattleStatus(BattleStatus.None);
		}
		float num = PlayerInput.MoveY();
		if (num != 0f)
		{
			IsMoving = true;
		}
		if (num > 0f)
		{
			if (m_CurrentLadderIndex >= m_LadderIndex)
			{
				ExitLadder(m_LadderTop);
			}
			else
			{
				DoMoveUp();
			}
		}
		else if (num < 0f)
		{
			if (m_CurrentLadderIndex <= 0)
			{
				ExitLadder(m_LadderBottom);
			}
			else
			{
				DoMoveDown();
			}
		}
	}

	private void EnterLadder(int startingIndex = 0)
	{
		if (startingIndex == 0)
		{
			m_LadderSide = 0;
			m_ClimbDirection = 1;
		}
		else
		{
			m_LadderSide = 1;
			m_ClimbDirection = 2;
		}
		GameManager.Instance.HideInteraction();
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.DisableTeleport();
		}
		m_CurrentLadderIndex = startingIndex;
		LadderRemoveListeners();
		GameManager.Instance.Player.OnAnimationComplete += HandlePlayerOnAnimationComplete;
		GameManager.Instance.Player.OnForceDeath += HandlePlayerOnForceDeath;
		GameManager.Instance.Player.S13SetClimbing(isClimbing: true);
	}

	private void EnterLadderOnComplete()
	{
		LadderRemoveListeners();
		GameManager.Instance.Player.OnAnimationComplete += HandlePlayerOnAnimationComplete;
		GameManager.Instance.Player.OnForceDeath += HandlePlayerOnForceDeath;
		IsActive = true;
	}

	public void ForceEnterBottom(LadderDataObject dataObject)
	{
		PrepareForceEnter(dataObject);
		m_LadderBottom.ForceEnter(dataObject.RotationX, dataObject.RotationY, m_LadderSide, ForceEnter);
	}

	public void ForceEnterTop(LadderDataObject dataObject)
	{
		PrepareForceEnter(dataObject);
		m_LadderTop.ForceEnter(dataObject.RotationX, dataObject.RotationY, m_LadderSide, ForceEnter);
	}

	private void PrepareForceEnter(LadderDataObject dataObject)
	{
		m_ClimbDirection = dataObject.ClimbDirection;
		m_LadderSide = dataObject.LadderSide;
		m_CurrentLadderIndex = dataObject.LadderIndex;
	}

	private void ForceEnter()
	{
		GameManager.Instance.HideInteraction();
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.DisableTeleport();
		}
		Vector3 position = m_LadderBottom.StartLocation.position;
		float num = 1f * (float)m_CurrentLadderIndex;
		if (m_CurrentLadderIndex > 0)
		{
			position.y += num;
		}
		GameManager.Instance.Player.transform.position = position;
		LadderRemoveListeners();
		GameManager.Instance.Player.OnAnimationComplete += HandlePlayerOnAnimationComplete;
		GameManager.Instance.Player.OnForceDeath += HandlePlayerOnForceDeath;
		GameManager.Instance.Player.S13SetClimbing(isClimbing: true);
		IsActive = true;
	}

	private void DoMoveUp()
	{
		m_CurrentLadderIndex++;
		DoMove("LadderMoveUp", GameManager.Instance.Player.transform.position.y + 1f);
	}

	private void DoMoveDown()
	{
		m_CurrentLadderIndex--;
		DoMove("LadderMoveDown", GameManager.Instance.Player.transform.position.y - 1f);
	}

	private void DoMove(string trigger, float positionY)
	{
		if (IsActive)
		{
			if (m_LadderSide == 1)
			{
				m_LadderSide = 2;
			}
			else
			{
				m_LadderSide = 1;
			}
			ResetTriggers();
			GameManager.Instance.Player.SetAnimationTrigger(trigger);
			GameManager.Instance.Player.transform.DOKill();
			GameManager.Instance.Player.transform.DOMoveY(positionY, 0.333f).SetEase(m_TweenEase).SetUpdate(m_TweenUpdateType)
				.OnComplete(EnableMove);
		}
	}

	private void ExitLadder(InteractableAnimation exitLadder)
	{
		IsActive = false;
		IsMoving = false;
		m_LadderBottom.ResetAll();
		m_LadderTop.ResetAll();
		exitLadder.OnExit -= HandleExitLadderOnExit;
		exitLadder.OnExit += HandleExitLadderOnExit;
		exitLadder.Exit();
		LadderRemoveListeners();
		GameManager.Instance.Player.OnForceDeath += HandlePlayerOnForceDeath;
		ResetTriggers();
		LadderAddListeners();
	}

	private void HandleExitLadderOnExit(object sender, EventArgs e)
	{
		(sender as InteractableAnimation).OnExit -= HandleExitLadderOnExit;
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnAnimationComplete -= HandlePlayerOnAnimationComplete;
			GameManager.Instance.Player.OnForceDeath -= HandlePlayerOnForceDeath;
			if (GameManager.Instance.Player.HasTeleport() && GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == AbilityStatus.Active)
			{
				GameManager.Instance.EnableTeleport();
			}
			GameManager.Instance.Player.S13SetClimbing(isClimbing: false);
		}
	}

	private void ResetTriggers()
	{
		GameManager.Instance.Player.ResetAnimationTrigger("LadderEnterBottom");
		GameManager.Instance.Player.ResetAnimationTrigger("LadderEnterTop");
		GameManager.Instance.Player.ResetAnimationTrigger("LadderMoveUp");
		GameManager.Instance.Player.ResetAnimationTrigger("LadderMoveDown");
	}

	private void EnableMove()
	{
		IsMoving = false;
	}

	private void HandleLadderTopOnInteracted(object sender, EventArgs e)
	{
		EnterLadder(m_LadderIndex);
	}

	private void HandleLadderBottomOnInteracted(object sender, EventArgs e)
	{
		EnterLadder();
	}

	private void HandlePlayerOnAnimationComplete(object sender, EventArgs e)
	{
		EnterLadderOnComplete();
	}

	private void HandlePlayerOnForceDeath(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnForceDeath -= HandlePlayerOnForceDeath;
		GameManager.Instance.Player.transform.DOKill();
		IsActive = false;
		IsMoving = false;
		m_LadderBottom.Stop();
		m_LadderTop.Stop();
		m_LadderBottom.ResetAll();
		m_LadderTop.ResetAll();
		m_LadderBottom.OnExit -= HandleExitLadderOnExit;
		m_LadderTop.OnExit -= HandleExitLadderOnExit;
		LadderRemoveListeners();
		ResetTriggers();
		LadderAddListeners();
		if (GameManager.Instance.Player.HasTeleport() && GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == AbilityStatus.Active)
		{
			GameManager.Instance.EnableTeleport();
		}
		GameManager.Instance.Player.UnlockRotation();
		GameManager.Instance.Player.S13SetClimbing(isClimbing: false);
		this.OnDeath.Send(this);
	}

	protected override void InternalEnable()
	{
		SetLadderActive(active: true);
		ActiveSetter[] componentsInChildren = base.transform.GetComponentsInChildren<ActiveSetter>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SetActive(active: true);
		}
	}

	protected override void InternalDisable()
	{
		SetLadderActive(active: false);
	}

	private void SetLadderActive(bool active)
	{
		IsActive = false;
		IsMoving = false;
		m_LadderBottom.SetActive(active);
		m_LadderTop.SetActive(active);
		if (active)
		{
			LadderRemoveListeners();
			LadderAddListeners();
		}
		else
		{
			LadderRemoveListeners();
		}
	}

	protected override void OnForceActivateComplete()
	{
		Enable();
	}

	private void LadderAddListeners()
	{
		if (m_LadderBottom != null)
		{
			m_LadderBottom.OnInteracted += HandleLadderBottomOnInteracted;
		}
		if (m_LadderTop != null)
		{
			m_LadderTop.OnInteracted += HandleLadderTopOnInteracted;
		}
	}

	private void LadderRemoveListeners()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnAnimationComplete -= HandlePlayerOnAnimationComplete;
			GameManager.Instance.Player.OnForceDeath -= HandlePlayerOnForceDeath;
		}
		if (m_LadderBottom != null)
		{
			m_LadderBottom.OnInteracted -= HandleLadderBottomOnInteracted;
		}
		if (m_LadderTop != null)
		{
			m_LadderTop.OnInteracted -= HandleLadderTopOnInteracted;
		}
	}

	protected override void OnDisposed()
	{
		LadderRemoveListeners();
		m_Ladders = null;
		m_LadderTop = null;
		m_LadderBottom = null;
		this.OnDeath = null;
		base.OnDisposed();
	}
}
