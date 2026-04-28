using System;

public class LeverContent : ActionEventContent<LeverContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
	}

	private void Update()
	{
		if (base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		Properties properties = m_Properties[0];
		if (base.IsInactive)
		{
			return;
		}
		if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat)
		{
			if (properties.ActionEvent.IsActive)
			{
				properties.ActionEvent.ForceDisable();
			}
		}
		else if (!properties.ActionEvent.IsActive)
		{
			Enable();
		}
	}

	protected override void OnActivate()
	{
		InternalActivation();
	}

	protected override void OnForceActivateComplete()
	{
		GetActiveProperties();
		int hashCode = GetHashCode();
		for (int i = 0; i < m_ActiveProperties.Count; i++)
		{
			Properties properties = m_ActiveProperties[i];
			if (hashCode != properties.ActionEvent.GetHashCode())
			{
				InteractableAnimationSingle interactableAnimationSingle = properties.ActionEvent as InteractableAnimationSingle;
				if (interactableAnimationSingle != null)
				{
					for (int j = 0; j < interactableAnimationSingle.Animators.Length; j++)
					{
						interactableAnimationSingle.Animators[j].SetTrigger("InteractForceComplete");
					}
					interactableAnimationSingle.Dispose();
				}
			}
			hashCode = properties.ActionEvent.GetHashCode();
		}
		Disable();
		base.IsActivated = true;
	}

	private void InternalActivation()
	{
		for (int i = 0; i < m_ActiveProperties.Count; i++)
		{
			Properties properties = m_ActiveProperties[i];
			ActivationAnimation(properties);
		}
	}

	private void ActivationAnimation(Properties properties)
	{
		InteractableAnimationBase obj = properties.ActionEvent as InteractableAnimationBase;
		obj.OnInteract -= HandleActivationOnInteract;
		obj.OnInteract += HandleActivationOnInteract;
		obj.OnInteractionComplete -= HandleActivationOnInteractionComplete;
		obj.OnInteractionComplete += HandleActivationOnInteractionComplete;
	}

	private void HandleActivationOnInteract(object sender, EventArgs e)
	{
		(sender as InteractableAnimationBase).OnInteract -= HandleActivationOnInteract;
		GameManager.Instance.Player.SetCollision(active: false);
	}

	private void HandleActivationOnInteractionComplete(object sender, EventArgs e)
	{
		(sender as InteractableAnimationBase).OnInteractionComplete -= HandleActivationOnInteractionComplete;
		GameManager.Instance.Player.SetCollision(active: true);
		ActivateComplete();
	}

	protected override void OnDeactivate()
	{
		InternalDeactivation();
	}

	protected override void OnForceDeactivateComplete()
	{
		GetActiveProperties();
		int hashCode = GetHashCode();
		for (int i = 0; i < m_ActiveProperties.Count; i++)
		{
			Properties properties = m_ActiveProperties[i];
			if (hashCode != properties.ActionEvent.GetHashCode())
			{
				InteractableAnimationSingle interactableAnimationSingle = properties.ActionEvent as InteractableAnimationSingle;
				if (interactableAnimationSingle != null)
				{
					for (int j = 0; j < interactableAnimationSingle.Animators.Length; j++)
					{
						interactableAnimationSingle.Animators[j].SetTrigger("Reset");
					}
				}
			}
			hashCode = properties.ActionEvent.GetHashCode();
		}
		base.IsActivated = false;
	}

	private void InternalDeactivation()
	{
		for (int i = 0; i < m_Properties.Length; i++)
		{
			Properties properties = m_Properties[i];
			DeactivationAnimation(properties);
		}
	}

	private void DeactivationAnimation(Properties properties)
	{
		if (properties != null && properties.ActionEvent != null && properties.ActionEvent is InteractableAnimationBase interactableAnimationBase && interactableAnimationBase != null)
		{
			interactableAnimationBase.OnInteract -= HandleDeactivationOnInteract;
			interactableAnimationBase.OnInteract += HandleDeactivationOnInteract;
			interactableAnimationBase.OnInteractionComplete -= HandleDeactivationOnInteractionComplete;
			interactableAnimationBase.OnInteractionComplete += HandleDeactivationOnInteractionComplete;
		}
	}

	private void HandleDeactivationOnInteract(object sender, EventArgs e)
	{
		(sender as InteractableAnimationBase).OnInteract -= HandleDeactivationOnInteract;
		GameManager.Instance.Player.SetCollision(active: false);
	}

	private void HandleDeactivationOnInteractionComplete(object sender, EventArgs e)
	{
		(sender as InteractableAnimationBase).OnInteractionComplete -= HandleDeactivationOnInteractionComplete;
		GameManager.Instance.Player.SetCollision(active: true);
		DeactivateComplete();
	}

	private void InternalRemoveListeners()
	{
		for (int i = 0; i < m_Properties.Length; i++)
		{
			Properties properties = m_Properties[i];
			if (properties != null && properties.ActionEvent != null && properties.ActionEvent is InteractableAnimationBase interactableAnimationBase && interactableAnimationBase != null)
			{
				interactableAnimationBase.OnInteract -= HandleActivationOnInteract;
				interactableAnimationBase.OnInteractionComplete -= HandleActivationOnInteractionComplete;
				interactableAnimationBase.OnInteract -= HandleDeactivationOnInteract;
				interactableAnimationBase.OnInteractionComplete -= HandleDeactivationOnInteractionComplete;
			}
		}
	}

	protected override void OnDisposed()
	{
		InternalRemoveListeners();
		base.OnDisposed();
	}
}
