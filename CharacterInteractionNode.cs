using System;
using UnityEngine;

public class CharacterInteractionNode : CharacterNode
{
	[Space]
	[SerializeField]
	private bool m_IsActive = true;

	[SerializeField]
	private CharacterNodeType m_NodeType;

	public AnimationClip InteractionClip;

	public AnimationClip InteractionLoopClip;

	public Transform EndLocation;

	[SerializeField]
	private CharacterInteraction m_Interactable;

	public override bool IsActive => m_IsActive;

	public CharacterNodeType NodeType => m_NodeType;

	public CharacterInteraction Interactable => m_Interactable;

	public event EventHandler OnInteract;

	public override void SetActive(bool active)
	{
		m_IsActive = active;
	}

	public void SendOnInteract()
	{
		this.OnInteract.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnInteract = null;
		base.OnDisposed();
	}
}
