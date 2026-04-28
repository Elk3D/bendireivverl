using System;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class MeatlyDirector : JMonoBehaviour
{
	[SerializeField]
	private MeatlyID m_MeatlyID;

	[SerializeField]
	private PlayableDirector m_PlayableDirector;

	private bool m_WasPlayingPause;

	public MeatlyID MeatlyID => m_MeatlyID;

	public PlayableDirector PlayableDirector => m_PlayableDirector;

	public bool IsPlaying { get; private set; }

	public bool IsSkipped { get; private set; }

	public bool IsComplete { get; private set; }

	public event EventHandler OnPause;

	public event EventHandler OnStopped;

	public event EventHandler OnSkipped;

	public event EventHandler OnStatusChange;

	public event EventHandler OnComplete;

	public override void Awake()
	{
		if (m_PlayableDirector == null)
		{
			m_PlayableDirector = GetComponent<PlayableDirector>();
		}
		m_PlayableDirector.playOnAwake = false;
	}

	private void Update()
	{
		if (IsComplete || base.IsDisposed)
		{
			return;
		}
		if (GameManager.Instance.IsPaused)
		{
			if (IsPlaying)
			{
				m_WasPlayingPause = true;
				IsPlaying = false;
				m_PlayableDirector.Pause();
			}
		}
		else if (m_WasPlayingPause)
		{
			m_WasPlayingPause = false;
			IsPlaying = true;
			m_PlayableDirector.Play();
		}
	}

	private void FixedUpdate()
	{
		if (IsPlaying && !IsComplete && !GameManager.Instance.IsPaused && !base.IsDisposed && (m_PlayableDirector.time >= m_PlayableDirector.duration || m_PlayableDirector.state != PlayState.Playing))
		{
			IsComplete = true;
			IsPlaying = false;
			ChangeStatus(CutsceneStatus.Complete);
		}
	}

	public void Play()
	{
		JDebug.Log("MeatlyDirector :: Play", this, JDebug.JDebugType.Cutscene);
		IsPlaying = true;
		m_PlayableDirector.Play();
		ChangeStatus(CutsceneStatus.Active);
	}

	public void Pause()
	{
		IsPlaying = false;
		IsSkipped = false;
		m_PlayableDirector.Pause();
		this.OnPause.Send(this);
	}

	public void Stop()
	{
		IsPlaying = false;
		IsSkipped = false;
		m_PlayableDirector.Stop();
		this.OnStopped.Send(this);
	}

	public void Skip()
	{
		IsSkipped = true;
		m_PlayableDirector.time = m_PlayableDirector.duration - 0.01;
		this.OnSkipped.Send(this);
	}

	public void ChangeStatus(CutsceneStatus status)
	{
		this.OnStatusChange.Send(status);
		if (status == CutsceneStatus.Complete)
		{
			this.OnComplete.Send(this);
		}
	}

	public void ResetDirector()
	{
		IsPlaying = false;
		IsSkipped = false;
		IsComplete = false;
		ChangeStatus(CutsceneStatus.None);
		m_PlayableDirector.Stop();
		m_PlayableDirector.time = 0.0;
	}

	protected override void OnDisposed()
	{
		this.OnPause = null;
		this.OnStopped = null;
		this.OnSkipped = null;
		this.OnStatusChange = null;
		this.OnComplete = null;
		m_PlayableDirector = null;
		base.OnDisposed();
	}
}
