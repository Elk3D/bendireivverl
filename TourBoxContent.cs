using System;
using DG.Tweening;
using UnityEngine;

public class TourBoxContent : ActionEventContent<TourBoxContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public LightBulbController Light;
	}

	private AudioClip m_AudioClip;

	private float m_AudioClipTimer;

	private float m_AudioClipLength;

	public bool IsAudioPlaying { get; private set; }

	public event EventHandler OnActivateContent;

	public event EventHandler OnDeactivateContent;

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (IsAudioPlaying)
		{
			if (m_AudioClipTimer > m_AudioClipLength + 0.5f)
			{
				ResetAudio();
			}
			else
			{
				m_AudioClipTimer += Time.deltaTime;
			}
		}
		else if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat)
		{
			CombatDisable();
		}
		else
		{
			CombatEnable();
		}
	}

	protected override void OnInitialized()
	{
		base.Connectable.OnActivated -= HandleConnectableOnActivated;
		base.Connectable.OnActivated += HandleConnectableOnActivated;
	}

	protected override void OnActivate()
	{
		this.OnActivateContent.Send(this);
		m_Sequencer?.New();
		int animatorHash = GetHashCode();
		for (int i = 0; i < m_ActiveProperties.Count; i++)
		{
			Properties properties = m_ActiveProperties[i];
			ActivationDOTween(properties, ref animatorHash);
		}
	}

	private void ActivationDOTween(Properties properties, ref int animatorHash)
	{
		if (animatorHash != properties.Animator.GetHashCode())
		{
			DoAnimation(properties, properties.ActiveLocation.localPosition, properties.ActiveLocation.localEulerAngles, 0.3f, Ease.OutBack);
		}
		animatorHash = properties.Animator.GetHashCode();
	}

	protected override void OnDeactivate()
	{
		this.OnDeactivateContent.Send(this);
		m_Sequencer?.New();
		int animatorHash = GetHashCode();
		for (int i = 0; i < m_ActiveProperties.Count; i++)
		{
			Properties properties = m_ActiveProperties[i];
			DeactivationDOTween(properties, ref animatorHash);
		}
	}

	private void DeactivationDOTween(Properties properties, ref int animatorHash)
	{
		if (animatorHash != properties.Animator.GetHashCode())
		{
			DoAnimation(properties, properties.m_OriginPosition, properties.m_OriginRotation, 0.3f, Ease.InBack);
		}
		animatorHash = properties.Animator.GetHashCode();
	}

	public void SetAudioClip(AudioClip audioClip)
	{
		m_AudioClip = audioClip;
		m_AudioClipLength = m_AudioClip.length;
	}

	private void PlayAudioClip()
	{
		if (m_AudioClip != null)
		{
			for (int i = 0; i < m_ActiveProperties.Count; i++)
			{
				Properties properties = m_ActiveProperties[i];
				properties.ActionEvent.SetActive(active: false);
				GameManager.Instance.Player.Interaction.ResetInteraction();
				properties.Light.TurnOn();
				m_AudioClipLength = m_AudioClip.length;
				IsAudioPlaying = true;
			}
		}
	}

	private void ResetAudio()
	{
		if (m_AudioClip != null)
		{
			for (int i = 0; i < m_ActiveProperties.Count; i++)
			{
				Properties properties = m_ActiveProperties[i];
				IsAudioPlaying = false;
				m_AudioClipTimer = 0f;
				m_AudioClipLength = 0f;
				properties.Light.TurnOff();
				properties.ActionEvent.ResetAction();
				properties.ActionEvent.SetActive(active: true);
			}
			Deactivate();
		}
	}

	private void HandleConnectableOnActivated(object sender, EventArgs e)
	{
		PlayAudioClip();
	}

	protected override void OnDisposed()
	{
		this.OnActivateContent = null;
		this.OnDeactivateContent = null;
		base.Connectable.OnActivated -= HandleConnectableOnActivated;
		base.OnDisposed();
	}
}
