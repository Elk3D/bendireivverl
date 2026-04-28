using System;
using S13Audio;
using UnityEngine;

public class LostOneTiedUp : JMonoBehaviour
{
	[Header("Interactable")]
	[SerializeField]
	private Interactable m_Interactable;

	[Header("Character Content")]
	[SerializeField]
	private CharacterContent m_CharacterContent;

	[Header("Animations")]
	[SerializeField]
	private AnimationClip m_TalkAnimationClip;

	[SerializeField]
	private AnimationClip m_ExecutionAnimationClip;

	[Header("Audio and Subttiles")]
	[SerializeField]
	private string m_SubtitleKey;

	[SerializeField]
	private S13AudioHandler m_DialogueAudioHandler;

	private S13Handler m_DialogueHandler;

	private S13ObjectSimple m_DialogueAudio;

	private bool m_IsInteracted;

	public event EventHandler OnInteract;

	public event EventHandler OnComplete;

	public override void Start()
	{
		m_DialogueAudio = m_DialogueAudioHandler.GetObject() as S13ObjectSimple;
		m_DialogueHandler = m_DialogueAudioHandler.GetHandler();
		RemoveListeners();
		AddListeners();
	}

	private void Update()
	{
		if (m_IsInteracted && !base.IsDisposed && !GameManager.Instance.IsPaused && !m_DialogueHandler.isPlaying)
		{
			Enable(onComplete: true);
		}
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		SetAnimation("Interact", m_TalkAnimationClip);
		m_DialogueAudioHandler.Play();
		GameManager.Instance.ShowSubtitles(TextUtility.GetKey(m_SubtitleKey), m_DialogueAudio.clip.length, isTrimmed: true);
		Disable(onInteract: true);
	}

	public void Execute()
	{
		SetAnimation("Death", m_ExecutionAnimationClip);
		Disable();
	}

	public void Enable(bool onComplete = false)
	{
		m_IsInteracted = false;
		AddListeners();
		m_Interactable.SetActive(active: true);
		m_Interactable.ResetAction();
		if (onComplete)
		{
			this.OnComplete.Send(this);
		}
	}

	public void Disable(bool onInteract = false)
	{
		RemoveListeners();
		m_Interactable.SetActive(active: false);
		m_IsInteracted = onInteract;
		if (onInteract)
		{
			this.OnInteract.Send(this);
		}
	}

	private void SetAnimation(string trigger, AnimationClip animationClip)
	{
		animationClip.name = trigger;
		m_CharacterContent.UpdateClipOverrides(animationClip);
		m_CharacterContent.SetAnimationTrigger(trigger);
	}

	private void AddListeners()
	{
		m_Interactable.OnInteract += HandleInteractableOnInteract;
	}

	private void RemoveListeners()
	{
		m_Interactable.OnInteract -= HandleInteractableOnInteract;
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		this.OnInteract = null;
		this.OnComplete = null;
		m_DialogueHandler = null;
		m_DialogueAudio = null;
		base.OnDisposed();
	}
}
