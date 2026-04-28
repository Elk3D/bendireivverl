using System;
using DG.Tweening;
using UnityEngine;

public class GentCardExchangeContent : ActionEventContent<GentCardExchangeContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
	}

	private bool m_IsActivated;

	private void Update()
	{
		if (m_IsActivated || GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
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

	protected override void InternalEnable()
	{
		if (m_IsActivated)
		{
			GameManager.Instance.Player.ResetRotation();
			GameManager.Instance.Player.SetState(State.Player.Default);
			GameManager.Instance.Player.ShowFirstPersonArms();
			ResetActionEvents();
			AddListeners();
			m_IsActivated = false;
		}
	}

	protected override void OnActivate()
	{
		Properties properties = m_Properties[0];
		GameManager.Instance.Player.SetState(State.Player.Cutscene);
		GameManager.Instance.Player.HideFirstPersonArms();
		GameManager.Instance.Player.SlideToLocation(properties.AnimationLocation);
		GameManager.Instance.Player.HeadContainer.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.InOutSine).OnComplete(InternalActivated);
	}

	private void InternalActivated()
	{
		GameManager.Instance.ShowGentExchange(base.Connectable as GentCardExchange);
		m_IsActivated = true;
	}
}
