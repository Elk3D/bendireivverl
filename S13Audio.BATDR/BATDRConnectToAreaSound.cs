using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRConnectToAreaSound : MonoBehaviour
{
	[SerializeField]
	private S13AreaHandlerReference areaSound;

	private bool _targetWithinArea;

	public S13LocalAction onEnterArea;

	public S13LocalAction onExitArea;

	private void OnEnable()
	{
		if (areaSound != null)
		{
			areaSound.onAreaEnter += OnAreaEnter;
			areaSound.onAreaExit += OnAreaExit;
		}
	}

	private void OnDisable()
	{
		if (areaSound != null)
		{
			areaSound.onAreaEnter -= OnAreaEnter;
			areaSound.onAreaExit -= OnAreaExit;
		}
	}

	private void OnAreaEnter()
	{
		if (onEnterArea.IsExecutable)
		{
			onEnterArea.Execute();
		}
	}

	private void OnAreaExit()
	{
		if (onExitArea.IsExecutable)
		{
			onExitArea.Execute();
		}
	}
}
