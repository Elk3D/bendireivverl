using I2.Loc;
using UnityEngine;

public class DialogueDataVO : JDisposable
{
	public string Dialogue;

	public AudioClip DialogueClip;

	public UISubtitleDataVO Subtitles;

	public static DialogueDataVO Create(string dialogue, string subtitles, bool isTrimmed = false)
	{
		string Translation = subtitles;
		if (LocalizationManager.TryGetTranslation(subtitles, out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
		{
			subtitles = Translation;
		}
		UISubtitleDataVO subtitles2 = UISubtitleDataVO.Create(subtitles, 0f, isTrimmed);
		return new DialogueDataVO
		{
			Dialogue = dialogue,
			Subtitles = subtitles2
		};
	}

	public static DialogueDataVO Create(AudioClip dialogue, string subtitles, bool isTrimmed = false)
	{
		string Translation = subtitles;
		if (LocalizationManager.TryGetTranslation(subtitles, out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
		{
			subtitles = Translation;
		}
		UISubtitleDataVO subtitles2 = UISubtitleDataVO.Create(subtitles, 0f, isTrimmed);
		return new DialogueDataVO
		{
			DialogueClip = dialogue,
			Subtitles = subtitles2
		};
	}

	protected override void OnDisposed()
	{
		Subtitles.Dispose();
		Subtitles = null;
		base.OnDisposed();
	}
}
