using System;
using UnityEngine;

public class AnimationEventHeadTracker : JMonoBehaviour
{
	[SerializeField]
	private InteractableAnimationSingle m_InteractableAnimation;

	[SerializeField]
	private HeadTracker m_HeadTracker;

	public override void Start()
	{
		if (m_InteractableAnimation != null && m_HeadTracker != null)
		{
			m_InteractableAnimation.OnInteract -= HandleOnInteract;
			m_InteractableAnimation.OnInteract += HandleOnInteract;
			m_InteractableAnimation.OnInteractionComplete -= HandleOnInteractionComplete;
			m_InteractableAnimation.OnInteractionComplete += HandleOnInteractionComplete;
		}
	}

	private void HandleOnInteract(object sender, EventArgs e)
	{
		if (m_HeadTracker != null)
		{
			m_HeadTracker.SetActive(active: false);
		}
	}

	private void HandleOnInteractionComplete(object sender, EventArgs e)
	{
		if (m_HeadTracker != null)
		{
			m_HeadTracker.SetActive(active: true);
		}
	}

	protected override void OnDisposed()
	{
		m_InteractableAnimation.OnInteract -= HandleOnInteract;
		m_InteractableAnimation.OnInteractionComplete -= HandleOnInteractionComplete;
		base.OnDisposed();
	}
}
