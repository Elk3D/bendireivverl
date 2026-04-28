using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRRadioPlayer : MonoBehaviour
{
	[SerializeField]
	private S13LocalAction switchOnAction;

	[SerializeField]
	private S13LocalAction switchOffAction;

	[SerializeField]
	private S13ObjectDynamic radioBroadcastObject;

	public bool IsPlaying => radioBroadcastObject.isPlaying;

	public void Play(AudioClip clip = null)
	{
		if (switchOnAction.IsExecutable)
		{
			switchOnAction.Execute();
		}
		if (clip != null)
		{
			S13HandlerDynamic s13HandlerDynamic = S13Manager.GetGlobalHandler(radioBroadcastObject) as S13HandlerDynamic;
			if (s13HandlerDynamic != null)
			{
				s13HandlerDynamic.SetNextClip(clip);
				s13HandlerDynamic.Play(base.transform);
			}
			else
			{
				S13Debug.LogWarning("No handler found for radio object.", this);
			}
		}
		else
		{
			radioBroadcastObject.Play(base.transform);
		}
	}

	public void Stop()
	{
		if (switchOffAction.IsExecutable)
		{
			switchOffAction.Execute();
		}
		radioBroadcastObject.Stop();
	}
}
