using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRUIAudioManager : MonoBehaviour
{
	private static BATDRUIAudioManager _Instance;

	[SerializeField]
	private S13LocalAction onHighlightAction;

	[SerializeField]
	private S13LocalAction onInteractPrompt;

	private bool highlightFlag;

	private bool promptFlag;

	private static BATDRUIAudioManager Instance
	{
		get
		{
			if (!_Instance)
			{
				_Instance = Object.FindObjectOfType<BATDRUIAudioManager>();
				if (!_Instance)
				{
					Debug.LogError("BATDRUIAudioManager does not exist within scene. One must be added in this or previously loaded scene for UI audio to function");
				}
			}
			return _Instance;
		}
	}

	public static void UIHighlightBegin()
	{
		if (!Instance.highlightFlag)
		{
			Instance.highlightFlag = true;
			if (Instance.onHighlightAction.IsExecutable)
			{
				Instance.onHighlightAction.Execute();
			}
		}
	}

	public static void UIHighlightEnd()
	{
		Instance.highlightFlag = false;
	}

	public static void UIInteractPromptBegin()
	{
		if (!Instance.promptFlag)
		{
			Instance.promptFlag = true;
			if (!Instance.highlightFlag && Instance.onInteractPrompt.IsExecutable)
			{
				Instance.onInteractPrompt.Execute();
			}
		}
	}

	public static void UIInteractPromptEnd()
	{
		Instance.promptFlag = false;
	}
}
