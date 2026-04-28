using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public abstract class ActionEventContent<T> : ActionEventContent where T : ActionEventProperties
{
	protected UnityEvent EventActivated;

	protected UnityEvent EventDeactivated;

	[SerializeField]
	protected bool m_Connected;

	[SerializeField]
	protected T[] m_Properties;

	protected Dictionary<int, T> m_PropertiesDictionary = new Dictionary<int, T>();

	protected List<T> m_ActiveProperties = new List<T>();

	protected Sequencer m_Sequencer;

	public override bool IsConnected => m_Connected;

	public override bool IsPlaying
	{
		get
		{
			if (m_Sequencer != null)
			{
				return m_Sequencer.IsPlaying;
			}
			return false;
		}
	}

	public ActionEventController Connectable { get; private set; }

	public override void Initialize(object connected)
	{
		Connectable = (ActionEventController)connected;
		OnInitialize();
		for (int i = 0; i < m_Properties.Length; i++)
		{
			T val = m_Properties[i];
			val.Initialize();
			if (val.Animator != null)
			{
				m_Sequencer = new Sequencer(isIndependent: false, UpdateType.Fixed).New();
			}
			if (val.ActionEvent != null)
			{
				int hashCode = val.ActionEvent.GetHashCode();
				if (!m_PropertiesDictionary.ContainsKey(hashCode))
				{
					m_PropertiesDictionary.Add(hashCode, val);
				}
			}
		}
		RemoveListeners();
		AddListeners();
		Initialized();
	}

	protected virtual void OnInitialize()
	{
	}

	private void Initialized()
	{
		OnInitialized();
	}

	protected virtual void OnInitialized()
	{
	}

	protected void CombatDisable()
	{
		if (!base.IsInactive)
		{
			Disable();
		}
	}

	protected void CombatEnable()
	{
		if (base.IsInactive)
		{
			Enable();
		}
	}

	private void HandleOnInteract(object sender, EventArgs e)
	{
		if (CheckIsInactive())
		{
			return;
		}
		ActionEvent actionEvent = (ActionEvent)sender;
		if (!base.IsActivated && !m_Connected)
		{
			int hashCode = actionEvent.GetHashCode();
			m_ActiveProperties.Clear();
			if (m_PropertiesDictionary.ContainsKey(hashCode))
			{
				m_ActiveProperties.Add(m_PropertiesDictionary[hashCode]);
			}
		}
		else
		{
			GetActiveProperties();
		}
		RemoveListeners();
		OnAction();
	}

	public override void Action()
	{
		if (!base.IsActivated)
		{
			Activate();
		}
		else
		{
			Deactivate();
		}
	}

	protected void OnAction()
	{
		if (!base.IsActivated)
		{
			if (!CheckIsInactive())
			{
				InternalActivate();
			}
		}
		else
		{
			Deactivate();
		}
	}

	public override void Activate()
	{
		if (!CheckIsInactive())
		{
			GetActiveProperties();
			InternalActivate();
		}
	}

	public sealed override void ForceActivate()
	{
		GetActiveProperties();
		OnForceActivate();
		InternalActivate();
	}

	protected virtual void OnForceActivate()
	{
	}

	private void InternalActivate()
	{
		OnActivate();
		if (!base.IsTooExpensive)
		{
			if ((bool)Connectable)
			{
				Connectable.SendOnActivate();
			}
			if (m_Sequencer != null)
			{
				m_Sequencer.OnComplete(ActivateComplete);
			}
		}
	}

	protected virtual void OnActivate()
	{
	}

	public sealed override void ForceActivateComplete()
	{
		OnForceActivateComplete();
	}

	protected virtual void OnForceActivateComplete()
	{
	}

	protected void ActivateComplete()
	{
		ActivateComplete(isForceComplete: false);
	}

	protected void ActivateComplete(bool isForceComplete)
	{
		OnActivateComplete();
		ResetActionEvents();
		RemoveListeners();
		AddListeners();
		base.IsActivated = true;
		EventActivated?.Invoke();
		if ((bool)Connectable && !isForceComplete)
		{
			Connectable.SendOnActivated();
		}
	}

	protected virtual void OnActivateComplete()
	{
	}

	public void AnimationComplete()
	{
		ActivateComplete();
	}

	public override void Deactivate()
	{
		if (!CheckIsInactive())
		{
			InternalDeactivate();
		}
	}

	public sealed override void ForceDeactivate()
	{
		OnForceDeactivate();
		InternalDeactivate();
	}

	protected virtual void OnForceDeactivate()
	{
	}

	private void InternalDeactivate()
	{
		OnDeactivate();
		if ((bool)Connectable)
		{
			Connectable.SendOnOnDeactivate();
		}
		if (m_Sequencer != null)
		{
			m_Sequencer.OnComplete(DeactivateComplete);
		}
	}

	protected virtual void OnDeactivate()
	{
	}

	public sealed override void ForceDeactivateComplete()
	{
		OnForceDeactivateComplete();
	}

	protected virtual void OnForceDeactivateComplete()
	{
	}

	protected void DeactivateComplete()
	{
		OnDeactivateComplete();
		ResetActionEvents();
		RemoveListeners();
		AddListeners();
		base.IsActivated = false;
		EventDeactivated?.Invoke();
		if ((bool)Connectable)
		{
			Connectable.SendOnOnDeactivated();
		}
	}

	protected virtual void OnDeactivateComplete()
	{
	}

	public sealed override void ForceInactive()
	{
		base.IsInactive = true;
	}

	public sealed override void ForceActive()
	{
		base.IsInactive = false;
		base.IsDisabled = false;
	}

	public sealed override void Disable()
	{
		base.IsInactive = true;
		base.IsDisabled = true;
		for (int i = 0; i < m_Properties.Length; i++)
		{
			m_Properties[i].ActionEvent?.SetActive(active: false);
		}
		if ((bool)Connectable)
		{
			Connectable.SendOnDisabled();
		}
		InternalDisable();
	}

	protected virtual void InternalDisable()
	{
	}

	public sealed override void Enable()
	{
		base.IsInactive = false;
		base.IsDisabled = false;
		for (int i = 0; i < m_Properties.Length; i++)
		{
			T val = m_Properties[i];
			if (val.HasActionEvent)
			{
				val.ActionEvent?.ResetAction();
			}
			val.ActionEvent?.SetActive(active: true);
		}
		if ((bool)Connectable)
		{
			Connectable.SendOnEnabled();
		}
		InternalEnable();
	}

	protected virtual void InternalEnable()
	{
	}

	protected bool CheckIsInactive()
	{
		if (base.IsInactive)
		{
			if (!base.IsActivated)
			{
				OnIsInactive();
				if ((bool)Connectable)
				{
					Connectable.SendOnInactive();
				}
			}
			return true;
		}
		return false;
	}

	public virtual void OnIsInactive()
	{
	}

	protected void DoAnimation(T properties, Vector3 position, Vector3 rotation, float duration, Ease ease, float delay = 0f)
	{
		if (properties.HasActivePosition)
		{
			m_Sequencer.Insert(delay, properties.Animator.DOLocalMove(position, duration).SetEase(ease));
		}
		if (properties.HasActiveRotation)
		{
			m_Sequencer.Insert(delay, properties.Animator.DOLocalRotate(rotation, duration).SetEase(ease));
		}
	}

	protected void ForceAnimationComplete(T properties, Vector3 position, Vector3 rotation)
	{
		if (properties.HasActivePosition)
		{
			properties.Animator.localPosition = position;
		}
		if (properties.HasActiveRotation)
		{
			properties.Animator.localEulerAngles = rotation;
		}
		ActivateComplete(isForceComplete: true);
	}

	protected void GetActiveProperties()
	{
		m_ActiveProperties.Clear();
		for (int i = 0; i < m_Properties.Length; i++)
		{
			m_ActiveProperties.Add(m_Properties[i]);
		}
	}

	public override void ResetActionEvents()
	{
		for (int i = 0; i < m_Properties.Length; i++)
		{
			T val = m_Properties[i];
			if (val.HasActionEvent)
			{
				val.ActionEvent.ResetAction();
			}
		}
	}

	protected void AddListeners()
	{
		for (int i = 0; i < m_Properties.Length; i++)
		{
			T val = m_Properties[i];
			if (val.HasActionEvent)
			{
				val.ActionEvent.OnInteract += HandleOnInteract;
			}
		}
	}

	protected void RemoveListeners()
	{
		for (int i = 0; i < m_Properties.Length; i++)
		{
			T val = m_Properties[i];
			if (val.HasActionEvent)
			{
				val.ActionEvent.OnInteract -= HandleOnInteract;
			}
		}
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		EventActivated = null;
		EventDeactivated = null;
		Connectable = null;
		if (m_Sequencer != null)
		{
			m_Sequencer.Dispose();
			m_Sequencer = null;
		}
		if (m_PropertiesDictionary != null)
		{
			m_PropertiesDictionary.Clear();
			m_PropertiesDictionary = null;
		}
		if (m_ActiveProperties != null)
		{
			m_ActiveProperties.Clear();
			m_ActiveProperties = null;
		}
		base.OnDisposed();
	}
}
public abstract class ActionEventContent : JMonoBehaviour
{
	public bool IsDisabled { get; protected set; }

	public bool IsInactive { get; protected set; }

	public bool IsActivated { get; protected set; }

	public bool IsTooExpensive { get; protected set; }

	public abstract bool IsConnected { get; }

	public abstract bool IsPlaying { get; }

	public abstract void Initialize(object connected);

	public abstract void ResetActionEvents();

	public abstract void Action();

	public abstract void Activate();

	public abstract void ForceActivate();

	public abstract void ForceActivateComplete();

	public abstract void Deactivate();

	public abstract void ForceDeactivate();

	public abstract void ForceDeactivateComplete();

	public abstract void Enable();

	public abstract void Disable();

	public abstract void ForceInactive();

	public abstract void ForceActive();

	public void ResetInactive()
	{
		ResetActionEvents();
		Enable();
		ForceInactive();
	}
}
