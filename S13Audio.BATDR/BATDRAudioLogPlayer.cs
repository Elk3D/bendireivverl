using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace S13Audio.BATDR;

public class BATDRAudioLogPlayer : MonoBehaviour
{
	private static BATDRAudioLogPlayer _Instance;

	[SerializeField]
	[FormerlySerializedAs("startAction")]
	private S13LocalAction tapeSFXStartAction;

	[SerializeField]
	[FormerlySerializedAs("stopAction")]
	private S13LocalAction tapeSFXStopAction;

	[SerializeField]
	[Tooltip("Delay's the playing of the actual audiolog clip by n seconds when using tape SFX")]
	private float tapeSFXAudiologDelay = 0.5f;

	[Header("Settings")]
	[SerializeField]
	[Tooltip("S13ObjectDynamic so that we can play incoming AudioClips")]
	private S13ObjectDynamic audioLogPlayerObject;

	[FormerlySerializedAs("stopCallbackDelay")]
	[SerializeField]
	[Tooltip("Delay's the closing of the AudioLog UI for n seconds")]
	private float stopUICallbackDelay = 0.5f;

	[SerializeField]
	[Tooltip("Delay's the playing of a new clip after an interrupt by n seconds. This is ignored if enableTapeSFX is false")]
	private float interruptPlayDelay = 0.75f;

	[SerializeField]
	[Tooltip("Should tape SFX be played? Also effects delays. This value can change at runtime")]
	private bool enableTapeSFX;

	public static BATDRAudioLogPlayer Instance
	{
		get
		{
			if (!_Instance)
			{
				_Instance = UnityEngine.Object.FindObjectOfType<BATDRAudioLogPlayer>();
				if (!_Instance)
				{
					Debug.LogError("BATDRAudioLogPlayer does not exist within scene. One must be added in this or previously loaded scene for stamp to function");
				}
			}
			return _Instance;
		}
		set
		{
			_Instance = value;
		}
	}

	public static void Play(AudioClip clip)
	{
		if (Instance.audioLogPlayerObject.isPlaying)
		{
			Instance.InterruptHandler(clip);
		}
		else
		{
			Instance.PlayHandler(clip);
		}
	}

	public static void Stop()
	{
		Instance.StopHandler();
	}

	public static void ForceStop()
	{
		if (Instance != null && Instance.audioLogPlayerObject.isPlaying)
		{
			Instance.audioLogPlayerObject.Stop();
			Instance.tapeSFXStopAction.Execute();
		}
	}

	public static void Stop(Action callback)
	{
		if (Instance == null)
		{
			callback?.Invoke();
		}
		else
		{
			Instance.StartCoroutine(Instance.StopAudioLogCoroutine(Instance.stopUICallbackDelay, callback));
		}
	}

	public static void EnableTapeSFX(bool enabledTapeSFX)
	{
		Instance.enableTapeSFX = enabledTapeSFX;
	}

	private void InterruptHandler(AudioClip clip)
	{
		if (enableTapeSFX)
		{
			StartCoroutine(StopAudioLogCoroutine(interruptPlayDelay, delegate
			{
				PlayHandler(clip);
			}));
		}
		else
		{
			StartCoroutine(StopAudioLogCoroutine(0f, delegate
			{
				PlayHandler(clip);
			}));
		}
	}

	private void PlayHandler(AudioClip clip)
	{
		S13HandlerDynamic s13HandlerDynamic = S13Manager.GetGlobalHandler(audioLogPlayerObject) as S13HandlerDynamic;
		if (s13HandlerDynamic != null)
		{
			s13HandlerDynamic.SetNextClip(clip);
		}
		if (enableTapeSFX)
		{
			if (s13HandlerDynamic != null)
			{
				StartCoroutine(S13CoroutineUtil.WaitForDuration(tapeSFXAudiologDelay, ignoreTimeScale: true, s13HandlerDynamic.Play));
			}
			if (tapeSFXStartAction.IsExecutable)
			{
				tapeSFXStartAction.Execute();
			}
		}
		else if (!enableTapeSFX && s13HandlerDynamic != null)
		{
			s13HandlerDynamic.Play();
		}
	}

	private IEnumerator StopAudioLogCoroutine(float callbackDelay, Action callback)
	{
		StopHandler();
		yield return new WaitForSecondsRealtime(callbackDelay);
		callback?.Invoke();
	}

	private void StopHandler()
	{
		audioLogPlayerObject.Stop();
		if (enableTapeSFX && tapeSFXStopAction.IsExecutable)
		{
			tapeSFXStopAction.Execute();
		}
	}
}
