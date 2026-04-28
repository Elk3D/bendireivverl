using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class SewerDrainController : SectionController
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Button")]
	[SerializeField]
	private Interactable m_InteractableButton;

	[SerializeField]
	private Transform m_Button;

	[SerializeField]
	private LightFixture m_LightFixture;

	[Header("Plug")]
	[SerializeField]
	private Door m_Plug;

	[SerializeField]
	private GameObject m_PlugSparkles;

	[SerializeField]
	private Ladder m_PlugLadder;

	[SerializeField]
	private LightFlicker m_LightFlicker;

	[Header("Ink Door")]
	[SerializeField]
	private Door m_InkDoor;

	[Header("Ink Raft")]
	[SerializeField]
	private Transform m_InkRaft;

	[SerializeField]
	private Transform m_InkRaftTopLocation;

	[SerializeField]
	private Transform m_InkRaftBottomLocation;

	[SerializeField]
	private Ladder m_InkRaftLadder;

	[SerializeField]
	private Lever m_InkRaftLever;

	private Sequence m_ButtonSequence;

	public event EventHandler OnPush;

	protected override IEnumerator InternalInitialize()
	{
		if (m_InkDoor.Data.Status == DoorStatus.Open)
		{
			m_LightFixture.SetEmission(1f);
			m_PlugLadder.Content.Disable();
			if (m_Plug.Data.Status == DoorStatus.Open)
			{
				m_InkRaft.position = m_InkRaftTopLocation.position;
				m_InkRaftLadder.Content.Disable();
				m_InkRaftLever.Content.Enable();
				InteractableInactive componentInChildren = m_InkRaftLever.GetComponentInChildren<InteractableInactive>(includeInactive: true);
				if (componentInChildren != null)
				{
					componentInChildren.InitializeInactive();
				}
			}
		}
		else
		{
			m_LightFixture.SetEmission(0f);
		}
		if (m_Plug.Data.Status != DoorStatus.Open)
		{
			m_Plug.OnActivate -= HandlePlugOnActivate;
			m_Plug.OnActivate += HandlePlugOnActivate;
		}
		else
		{
			m_Plug.Content.Disable();
		}
		m_Plug.Content.Disable();
		m_PlugSparkles.SetActive(value: false);
		if (!CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
			m_InteractableButton.OnInteract -= HandleButtonOnInteract;
			m_InteractableButton.OnInteract += HandleButtonOnInteract;
		}
		yield return null;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("SewerDrainController :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
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
			m_InteractableButton.SetActive(active: false);
		}
		return flag;
	}

	private void HandlePlugOnActivate(object sender, EventArgs e)
	{
		m_Plug.OnActivate -= HandlePlugOnActivate;
		m_PlugSparkles.SetActive(value: false);
		m_LightFlicker.TurnOff(light: true);
	}

	private void HandleButtonOnInteract(object sender, EventArgs e)
	{
		m_InteractableButton.OnInteract -= HandleButtonOnInteract;
		m_InkDoor.OnActivated -= HandleDoorComplete;
		m_InkDoor.OnDeactivated -= HandleDoorComplete;
		if (m_InkDoor.Data.Status == DoorStatus.Open)
		{
			if (m_Plug.Data.Status == DoorStatus.Open)
			{
				m_InkRaft.position = m_InkRaftBottomLocation.position;
				m_InkRaftLadder.Content.Enable();
				m_InkRaftLever.Content.Disable();
			}
			m_PlugSparkles.SetActive(value: false);
			if (m_Plug.Data.Status == DoorStatus.Closed)
			{
				m_Plug.Content.Disable();
			}
			m_PlugLadder.Content.Disable();
			m_InkDoor.Content.ForceDeactivate();
			m_LightFixture.SetEmission(0f);
			m_InkDoor.OnDeactivated += HandleDoorComplete;
		}
		else
		{
			if (m_Plug.Data.Status == DoorStatus.Open)
			{
				m_InkRaft.position = m_InkRaftTopLocation.position;
				m_InkRaftLadder.Content.Disable();
				m_InkRaftLever.Content.Enable();
			}
			m_PlugSparkles.SetActive(value: false);
			if (m_Plug.Data.Status == DoorStatus.Closed)
			{
				m_Plug.Content.Disable();
			}
			m_PlugLadder.Content.Disable();
			m_InkDoor.Content.ForceActivate();
			m_LightFixture.SetEmission(1f);
			m_InkDoor.OnActivated += HandleDoorComplete;
		}
		ResetSequence();
		m_ButtonSequence.Insert(0f, m_Button.DOLocalMoveX(0.23f, 0.2f).SetEase(Ease.Linear));
		m_ButtonSequence.Insert(0.2f, m_Button.DOLocalMoveX(0f, 0.2f).SetEase(Ease.OutSine));
		this.OnPush.Send(this);
	}

	private void HandleDoorComplete(object sender, EventArgs e)
	{
		m_InkDoor.OnActivated -= HandleDoorComplete;
		m_InkDoor.OnDeactivated -= HandleDoorComplete;
		m_InteractableButton.OnInteract -= HandleButtonOnInteract;
		m_InteractableButton.OnInteract += HandleButtonOnInteract;
		m_InteractableButton.ResetAction();
		if (m_InkDoor.Data.Status == DoorStatus.Closed)
		{
			if (m_Plug.Data.Status == DoorStatus.Closed)
			{
				m_Plug.Content.Enable();
				m_PlugSparkles.SetActive(value: true);
			}
			m_PlugLadder.Content.Enable();
		}
	}

	private void ResetSequence()
	{
		KillSequence();
		m_ButtonSequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_ButtonSequence != null)
		{
			m_ButtonSequence.Kill();
			m_ButtonSequence = null;
		}
	}

	protected override void OnDisposed()
	{
		this.OnPush = null;
		KillSequence();
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		m_Plug.OnActivate -= HandlePlugOnActivate;
		m_InkDoor.OnActivated -= HandleDoorComplete;
		m_InkDoor.OnDeactivated -= HandleDoorComplete;
		m_InteractableButton.OnInteract -= HandleButtonOnInteract;
		base.OnDisposed();
	}
}
