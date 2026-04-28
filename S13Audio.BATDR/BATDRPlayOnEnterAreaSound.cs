using System.Collections.Generic;
using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRPlayOnEnterAreaSound : MonoBehaviour
{
	public S13AreaHandlerReference targetAreaSound;

	public List<S13AudioHandler> audioHandlers;

	private void Awake()
	{
		if (targetAreaSound != null)
		{
			targetAreaSound.onAreaEnter += Play;
			targetAreaSound.onAreaExit += Stop;
		}
	}

	private void OnDestroy()
	{
		if (targetAreaSound != null)
		{
			targetAreaSound.onAreaEnter -= Play;
			targetAreaSound.onAreaExit -= Stop;
		}
	}

	public void Play()
	{
		foreach (S13AudioHandler audioHandler in audioHandlers)
		{
			audioHandler.Play();
		}
	}

	public void Stop()
	{
		foreach (S13AudioHandler audioHandler in audioHandlers)
		{
			audioHandler.Stop();
		}
	}
}
