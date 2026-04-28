using System;
using UnityEngine;

public class FlowMantle : JMonoBehaviour
{
	[SerializeField]
	private InteractableAnimationBase m_InteractableAnimation;

	public InteractableAnimationBase InteractableAnimation => m_InteractableAnimation;

	public Vector3 StartPosition => m_InteractableAnimation.StartLocation.position;

	public event EventHandler OnEnter;

	public void Enter()
	{
		m_InteractableAnimation.Enter();
		this.OnEnter.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnEnter = null;
		base.OnDisposed();
	}
}
