using System;
using DG.Tweening;
using UnityEngine;

public class InteractableAnimationBase : InteractableInputDisplay
{
	[Header("Locations")]
	[SerializeField]
	protected Transform m_StartLocation;

	[SerializeField]
	protected Transform m_EndLocation;

	[Header("Options")]
	[SerializeField]
	protected bool m_IsSingleInteraction;

	[SerializeField]
	protected bool m_HideCrosshair = true;

	protected Sequence m_Sequence;

	public Transform StartLocation => m_StartLocation;

	public Transform EndLocation => m_EndLocation;

	public bool HasStartLocation => m_StartLocation != null;

	public bool HasEndLocation => m_EndLocation != null;

	public event EventHandler OnInteractionComplete;

	public event EventHandler OnInteractionExit;

	protected override void OnInternalInteract(Vector3 origin, RaycastHit hit, object sender = null)
	{
		base.OnInternalInteract(origin, hit, sender);
		GameManager.Instance.Player.Interaction.ResetInteraction();
		Enter();
	}

	public virtual void Enter()
	{
	}

	public virtual void ForceEnter(Quaternion rotationX, Quaternion rotationY)
	{
	}

	public virtual void Exit()
	{
	}

	protected void SendToEndLocation()
	{
		if (HasEndLocation)
		{
			Vector3 vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
			GameManager.Instance.Player.transform.position = m_EndLocation.position + vector;
			GameManager.Instance.Player.transform.eulerAngles = m_EndLocation.eulerAngles;
		}
	}

	protected void CheckCrosshair(bool show)
	{
		if (m_HideCrosshair)
		{
			if (show)
			{
				GameManager.Instance.ShowCrosshair(m_IsReal);
			}
			else
			{
				GameManager.Instance.HideCrosshair();
			}
		}
	}

	protected void SendOnInteractionComplete()
	{
		this.OnInteractionComplete.Send(this);
	}

	protected void SendOnInteractionExit()
	{
		this.OnInteractionExit.Send(this);
	}

	protected void ResetSequence()
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
		this.OnInteractionComplete = null;
		this.OnInteractionExit = null;
		KillSequence();
		base.OnDisposed();
	}
}
