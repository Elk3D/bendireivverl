using System;
using DG.Tweening;
using UnityEngine;

public class PlayerRespawn : JMonoBehaviour
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[SerializeField]
	private bool m_IsActive = true;

	[SerializeField]
	private Transform m_AnimationLocation;

	[SerializeField]
	private ParticleSystem m_InkDrip;

	private Sequence m_Sequence;

	public bool IsActive => m_IsActive;

	public Transform AnimationLocation => m_AnimationLocation;

	public bool IsRespawning { get; private set; }

	public void Initialize()
	{
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		if (!CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
		}
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("PlayerRespawn :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
		CheckStatus();
	}

	private bool CheckStatus()
	{
		bool flag = true;
		if (m_Requirements != null)
		{
			flag = m_Requirements.IsComplete();
		}
		if (flag)
		{
			GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
			SetActive(active: true);
		}
		return flag;
	}

	public void SetActive(bool active)
	{
		m_IsActive = active;
		if (m_IsActive)
		{
			GameManager.Instance.RespawnManager.Add(this);
		}
	}

	public void Respawn()
	{
		IsRespawning = true;
		GameManager.Instance.HideHealthBar();
		GameManager.Instance.HideCrosshair();
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == AbilityStatus.Active && GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.HideTeleport();
		}
		GameManager.Instance.Player.SetCollision(active: false);
		GameManager.Instance.Player.SetCombatStatus(CombatStatus.Hide);
		GameManager.Instance.Player.ClearEnemies();
		GameManager.Instance.ShowScreenBlocker(0f);
		GameManager.Instance.Player.transform.position = m_AnimationLocation.position;
		GameManager.Instance.Player.transform.eulerAngles = m_AnimationLocation.eulerAngles;
		GameManager.Instance.Player.SetState(State.Player.Cutscene);
		ResetSequence();
		m_Sequence.InsertCallback(2f, InternalRespawn);
		m_Sequence.InsertCallback(4f, delegate
		{
			m_InkDrip.Emit(3);
		});
	}

	private void InternalRespawn()
	{
		GameManager.Instance.GameCamera.InkDemon();
		GameManager.Instance.HideScreenBlocker(1.75f, 0.2f);
		GameManager.Instance.Player.SetAnimationType("RespawnType", 1);
		GameManager.Instance.Player.EnterInteraction("Respawn");
		GameManager.Instance.Player.OnAnimationComplete -= HandlePlayerOnAnimationComplete;
		GameManager.Instance.Player.OnAnimationComplete += HandlePlayerOnAnimationComplete;
	}

	private void HandlePlayerOnAnimationComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnAnimationComplete -= HandlePlayerOnAnimationComplete;
		GameManager.Instance.Player.ExitAnimation();
		GameManager.Instance.ShowCrosshair();
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == AbilityStatus.Active && GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.DisplayTeleport();
		}
		GameManager.Instance.Player.SetCollision(active: true);
		GameManager.Instance.Player.SetCombatStatus(CombatStatus.None);
		GameManager.Instance.Player.Respawn();
		IsRespawning = false;
		GameManager.Instance.Player.PlayerMovement.ResetMoveSpeed();
		GameManager.Instance.Player.PlayerMovement.UnlockJump();
		GameManager.Instance.Player.PlayerMovement.ForceUnlockRun();
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			m_Sequence.Kill();
			m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnAnimationComplete -= HandlePlayerOnAnimationComplete;
		}
		if (GameManager.Instance.RespawnManager != null)
		{
			GameManager.Instance.RespawnManager.Remove(this);
		}
		KillSequence();
		base.OnDisposed();
	}
}
