using System;
using UnityEngine;

[Serializable]
public abstract class ActionEventProperties
{
	protected internal Vector3 m_OriginPosition;

	protected internal Vector3 m_OriginRotation;

	public string Name = "New Properties";

	public SwitchType SwitchType;

	public Transform Container;

	public Transform Animator;

	public Transform ActiveLocation;

	public Animator AnimatorController;

	public string AnimationTriggerActivate = "Activate";

	public string AnimationTriggerDeactivate = "Deactivate";

	public string PlayerInteractionTriggerEnter = "SwitchInteractEnter";

	public string PlayerInteractionTriggerExit = "SwitchInteractExit";

	public Transform AnimationLocation;

	public ActionEvent ActionEvent;

	public Vector3 OriginPosition => m_OriginPosition;

	public Vector3 OriginRotation => m_OriginRotation;

	public bool HasActionEvent => ActionEvent != null;

	public bool HasActivePosition
	{
		get
		{
			if (ActiveLocation != null)
			{
				return ActiveLocation.localPosition != m_OriginPosition;
			}
			return false;
		}
	}

	public bool HasActiveRotation
	{
		get
		{
			if (ActiveLocation != null)
			{
				return ActiveLocation.localEulerAngles != m_OriginRotation;
			}
			return false;
		}
	}

	public void Initialize()
	{
		OnInitialize();
		if (Container != null)
		{
			m_OriginPosition = Container.localPosition;
			m_OriginRotation = Container.localEulerAngles;
		}
	}

	public virtual void OnInitialize()
	{
	}
}
