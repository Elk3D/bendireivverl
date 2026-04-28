using System;
using DG.Tweening;
using UnityEngine;

public class DoorContent : ActionEventContent<DoorContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public DoorType DoorType;

		public float Speed = 1f;

		public Ease Ease = Ease.InOutSine;

		public bool SyncSpeed;

		public float Delay;
	}

	private Door m_Door;

	private OcclusionPortal m_Portal;

	[SerializeField]
	protected DoorFunctionType m_DoorFunctionType;

	[SerializeField]
	protected EventTrigger m_ProximityEventTrigger;

	public event EventHandler OnForceActivateCompleted;

	public event EventHandler OnForceDeactivateCompleted;

	protected override void OnInitialize()
	{
		m_Door = (Door)base.Connectable;
		m_Portal = GetComponent<OcclusionPortal>();
		if (m_Portal == null)
		{
			m_Portal = GetComponentInChildren<OcclusionPortal>(includeInactive: true);
		}
		if (m_Portal != null)
		{
			m_Portal.open = false;
		}
		if (m_DoorFunctionType == DoorFunctionType.Proximity && m_ProximityEventTrigger != null)
		{
			RemoveDoorListeners();
			m_ProximityEventTrigger.OnEnter += HandleProximityEventTriggerOnEnter;
			m_ProximityEventTrigger.OnExit += HandleProximityEventTriggerOnExit;
		}
	}

	protected override void OnActivate()
	{
		InternalActivation();
		GameManager.Instance.Player.Interaction.ResetInteraction();
	}

	private void InternalActivation(bool isAnimation = true)
	{
		if ((bool)m_Portal)
		{
			m_Portal.open = true;
		}
		if (isAnimation)
		{
			m_Sequencer.New();
		}
		for (int i = 0; i < m_ActiveProperties.Count; i++)
		{
			Properties properties = m_ActiveProperties[i];
			if (isAnimation)
			{
				CheckAnimation(properties);
			}
			else
			{
				CheckAnimationComplete(properties);
			}
		}
	}

	private void CheckAnimation(Properties properties)
	{
		if (properties.DoorType == DoorType.Single)
		{
			m_Door.Data.SetType(DoorType.Single);
		}
		else if (properties.DoorType == DoorType.Front)
		{
			m_Door.Data.SetType(DoorType.Front);
		}
		else if (properties.DoorType == DoorType.Back)
		{
			m_Door.Data.SetType(DoorType.Back);
		}
		else if (properties.DoorType == DoorType.Left)
		{
			m_Door.Data.SetType(DoorType.Left);
		}
		else if (properties.DoorType == DoorType.Right)
		{
			m_Door.Data.SetType(DoorType.Right);
		}
		properties.Container.SetParent(properties.Animator);
		DoAnimation(properties, properties.ActiveLocation.localPosition, properties.ActiveLocation.localEulerAngles, properties.Speed, properties.Ease, properties.Delay);
	}

	private void CheckAnimationComplete(Properties properties)
	{
		DoorType type = m_Door.Data.Type;
		if (type == DoorType.Single || m_Door.Content.IsConnected)
		{
			InternalActivationForceComplete(properties);
		}
		else if (type == DoorType.Front && properties.DoorType == DoorType.Front)
		{
			InternalActivationForceComplete(properties);
		}
		else if (type == DoorType.Back && properties.DoorType == DoorType.Back)
		{
			InternalActivationForceComplete(properties);
		}
		else if (type == DoorType.Left && properties.DoorType == DoorType.Left)
		{
			InternalActivationForceComplete(properties);
		}
		else if (type == DoorType.Right && properties.DoorType == DoorType.Right)
		{
			InternalActivationForceComplete(properties);
		}
	}

	private void InternalActivationForceComplete(Properties properties)
	{
		properties.Container.SetParent(properties.Animator);
		ForceAnimationComplete(properties, properties.ActiveLocation.localPosition, properties.ActiveLocation.localEulerAngles);
	}

	protected override void OnForceActivateComplete()
	{
		if (base.IsActivated)
		{
			this.OnForceActivateCompleted.Send(this);
		}
		m_Sequencer?.Kill();
		RemoveListeners();
		GetActiveProperties();
		InternalActivation(isAnimation: false);
		ResetActionEvents();
		AddListeners();
		base.IsActivated = true;
	}

	public override void OnIsInactive()
	{
		ResetActionEvents();
	}

	protected override void OnDeactivate()
	{
		InternalDeactivation();
		GameManager.Instance.Player.Interaction.ResetInteraction();
	}

	protected override void OnForceDeactivateComplete()
	{
		if (!base.IsActivated)
		{
			this.OnForceDeactivateCompleted.Send(this);
		}
		m_Sequencer?.Kill();
		InternalDeactivation(isAnimation: false);
		ResetPivots();
		base.IsActivated = false;
	}

	private void InternalDeactivation(bool isAnimation = true)
	{
		if (isAnimation)
		{
			m_Sequencer.New();
		}
		for (int i = 0; i < m_Properties.Length; i++)
		{
			Properties properties = m_Properties[i];
			if (isAnimation)
			{
				float duration = (properties.SyncSpeed ? properties.Speed : 0.4f);
				DoAnimation(properties, Vector3.zero, Vector3.zero, duration, Ease.InOutSine);
			}
			else
			{
				ForceAnimationComplete(properties, Vector3.zero, Vector3.zero);
			}
		}
	}

	protected override void OnDeactivateComplete()
	{
		if ((bool)m_Portal)
		{
			m_Portal.open = false;
		}
		ResetPivots();
	}

	private void ResetPivots()
	{
		for (int i = 0; i < m_Properties.Length; i++)
		{
			m_Properties[i].Animator.localEulerAngles = Vector3.zero;
		}
	}

	private void HandleProximityEventTriggerOnEnter(object sender, EventArgs e)
	{
		if (!base.IsDisabled && (!base.IsActivated || m_Sequencer == null || m_Sequencer.IsPlaying) && (m_Door.Data.Status == DoorStatus.Open || m_Door.Data.Status == DoorStatus.Closed))
		{
			ForceActivate();
		}
	}

	private void HandleProximityEventTriggerOnExit(object sender, EventArgs e)
	{
		if (!base.IsDisabled && (m_Door.Data.Status == DoorStatus.Open || m_Door.Data.Status == DoorStatus.Closed))
		{
			ForceDeactivate();
			Enable();
		}
	}

	private void RemoveDoorListeners()
	{
		if (m_DoorFunctionType == DoorFunctionType.Proximity && m_ProximityEventTrigger != null)
		{
			m_ProximityEventTrigger.OnEnter -= HandleProximityEventTriggerOnEnter;
			m_ProximityEventTrigger.OnExit -= HandleProximityEventTriggerOnExit;
		}
	}

	protected override void OnDisposed()
	{
		this.OnForceActivateCompleted = null;
		this.OnForceDeactivateCompleted = null;
		RemoveDoorListeners();
		m_Door = null;
		m_Portal = null;
		base.OnDisposed();
	}
}
