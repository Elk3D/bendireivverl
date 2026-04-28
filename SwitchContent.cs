using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class SwitchContent : ActionEventContent<SwitchContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
	}

	[SerializeField]
	private UnityEvent OnActivated;

	[SerializeField]
	private UnityEvent OnDeactivated;

	protected override void OnInitialized()
	{
		EventActivated = OnActivated;
		EventDeactivated = OnDeactivated;
	}

	protected override void OnActivate()
	{
		InternalActivation();
	}

	protected override void OnForceActivateComplete()
	{
		InternalActivation(isAnimation: false);
	}

	private void InternalActivation(bool isAnimation = true)
	{
		GetActiveProperties();
		if (isAnimation)
		{
			m_Sequencer?.New();
		}
		int animatorHash = GetHashCode();
		for (int i = 0; i < m_ActiveProperties.Count; i++)
		{
			Properties properties = m_ActiveProperties[i];
			if (properties.SwitchType == SwitchType.DOTween)
			{
				ActivationDOTween(properties, ref animatorHash, isAnimation);
			}
			else if (properties.SwitchType == SwitchType.Animation)
			{
				ActivationAnimation(properties);
			}
		}
	}

	private void ActivationDOTween(Properties properties, ref int animatorHash, bool isAnimation = true)
	{
		if (animatorHash != properties.Animator.GetHashCode())
		{
			if (isAnimation)
			{
				DoAnimation(properties, properties.ActiveLocation.localPosition, properties.ActiveLocation.localEulerAngles, 0.5f, Ease.OutBack);
			}
			else
			{
				ForceAnimationComplete(properties, properties.ActiveLocation.localPosition, properties.ActiveLocation.localEulerAngles);
			}
		}
		animatorHash = properties.Animator.GetHashCode();
	}

	private void ActivationAnimation(Properties properties)
	{
		if (properties.AnimatorController != null)
		{
			properties.AnimatorController.SetTrigger(properties.AnimationTriggerDeactivate);
		}
		GameManager.Instance.Player.SetCollision(active: false);
		GameManager.Instance.Player.OnAnimationComplete -= HandleAnimationOnComplete;
		GameManager.Instance.Player.OnAnimationComplete += HandleAnimationOnComplete;
		GameManager.Instance.Player.EnterInteraction(properties.PlayerInteractionTriggerEnter);
		GameManager.Instance.Player.SlideToLocation(properties.AnimationLocation);
	}

	private void HandleAnimationOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnAnimationComplete -= HandleAnimationOnComplete;
		GameManager.Instance.Player.ExitAnimation();
		GameManager.Instance.Player.SetCollision(active: true);
		ActivateComplete();
	}

	protected override void OnDeactivate()
	{
		InternalDeactivation();
	}

	protected override void OnForceDeactivateComplete()
	{
		InternalDeactivation(isAnimation: false);
	}

	private void InternalDeactivation(bool isAnimation = true)
	{
		if (isAnimation)
		{
			m_Sequencer?.New();
		}
		int hashCode = GetHashCode();
		for (int i = 0; i < m_Properties.Length; i++)
		{
			Properties properties = m_Properties[i];
			if (properties.SwitchType == SwitchType.DOTween)
			{
				if (hashCode != properties.Animator.GetHashCode())
				{
					if (isAnimation)
					{
						DoAnimation(properties, properties.m_OriginPosition, properties.m_OriginRotation, 0.4f, Ease.InBack);
					}
					else
					{
						ForceAnimationComplete(properties, properties.m_OriginPosition, properties.m_OriginRotation);
					}
				}
				hashCode = properties.Animator.GetHashCode();
			}
			else if (properties.SwitchType == SwitchType.Animation)
			{
				if (isAnimation)
				{
					DeactivationAnimation(properties);
				}
				else if (properties.AnimatorController != null)
				{
					properties.AnimatorController.SetTrigger("Reset");
				}
			}
		}
		if (!isAnimation)
		{
			DeactivateComplete();
		}
	}

	private void DeactivationAnimation(Properties properties)
	{
		if (properties.AnimatorController != null)
		{
			properties.AnimatorController.SetTrigger(properties.AnimationTriggerActivate);
		}
		GameManager.Instance.Player.SetCollision(active: false);
		GameManager.Instance.Player.OnAnimationComplete -= HandleAnimationDeactivateOnComplete;
		GameManager.Instance.Player.OnAnimationComplete += HandleAnimationDeactivateOnComplete;
		GameManager.Instance.Player.EnterInteraction(properties.PlayerInteractionTriggerExit);
		GameManager.Instance.Player.SlideToLocation(properties.AnimationLocation);
	}

	private void HandleAnimationDeactivateOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnAnimationComplete -= HandleAnimationDeactivateOnComplete;
		GameManager.Instance.Player.ExitAnimation();
		GameManager.Instance.Player.SetCollision(active: true);
		DeactivateComplete();
	}

	private void InternalRemoveListeners()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnAnimationComplete -= HandleAnimationOnComplete;
			GameManager.Instance.Player.OnAnimationComplete -= HandleAnimationDeactivateOnComplete;
		}
	}

	protected override void OnDisposed()
	{
		InternalRemoveListeners();
		base.OnDisposed();
	}
}
