using UnityEngine;

public class AudioClipSubtitleChecker : JMonoBehaviour
{
	[SerializeField]
	private AudioSource m_AudioSource;

	private bool m_IsPlaying;

	private void Update()
	{
		if (base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (m_AudioSource.isPlaying && !m_IsPlaying)
		{
			m_IsPlaying = true;
			if (m_AudioSource.clip.name.ToLower().Contains("vo_allison_calling_from_pipe_01_fixed"))
			{
				ShowSubtitles("Cutscene_S108_ID03_AllisonInAPipe_Dialogue_01", m_AudioSource.clip.length, isTrimmed: true);
			}
			else if (m_AudioSource.clip.name.ToLower().Contains("vo_allison_calling_from_pipe_02_fixed"))
			{
				ShowSubtitles("Cutscene_S108_ID03_AllisonInAPipe_Dialogue_02", m_AudioSource.clip.length, isTrimmed: true);
			}
			else if (m_AudioSource.clip.name.ToLower().Contains("vo_lostone_artist_door_knock_01"))
			{
				ShowSubtitles("Cutscene_S110_ID03_LostOneAtDoor_Dialogue_01", m_AudioSource.clip.length, isTrimmed: true);
			}
			else if (m_AudioSource.clip.name.ToLower().Contains("vo_lostone_artist_door_knock_02"))
			{
				ShowSubtitles("Cutscene_S110_ID03_LostOneAtDoor_Dialogue_02", m_AudioSource.clip.length, isTrimmed: true);
			}
			else if (m_AudioSource.clip.name.ToLower().Contains("vo_audrey_nothing_01"))
			{
				ShowSubtitles("Cutscene_S119_ID08_Clue_Dialogue_01", m_AudioSource.clip.length, isTrimmed: true);
			}
			else if (m_AudioSource.clip.name.ToLower().Contains("vo_audrey_nothing_02"))
			{
				ShowSubtitles("Cutscene_S119_ID08_Clue_Dialogue_02", m_AudioSource.clip.length, isTrimmed: true);
			}
			else if (m_AudioSource.clip.name.ToLower().Contains("vo_audrey_nothing_03"))
			{
				ShowSubtitles("Cutscene_S119_ID08_Clue_Dialogue_03", m_AudioSource.clip.length, isTrimmed: true);
			}
			else if (m_AudioSource.clip.name.ToLower().Contains("vo_audrey_nothing_04"))
			{
				ShowSubtitles("Cutscene_S119_ID08_Clue_Dialogue_04", m_AudioSource.clip.length, isTrimmed: true);
			}
			else if (m_AudioSource.clip.name.ToLower().Contains("vo_keeper_station_dialogue_01"))
			{
				ShowSubtitles("Subway_Keeper_Dialogue_01", m_AudioSource.clip.length, isTrimmed: true);
			}
			else if (m_AudioSource.clip.name.ToLower().Contains("vo_keeper_station_dialogue_02"))
			{
				ShowSubtitles("Subway_Keeper_Dialogue_02", m_AudioSource.clip.length, isTrimmed: true);
			}
			else if (m_AudioSource.clip.name.ToLower().Contains("vo_keeper_station_dialogue_03"))
			{
				ShowSubtitles("Subway_Keeper_Dialogue_03", m_AudioSource.clip.length, isTrimmed: true);
			}
			else if (m_AudioSource.clip.name.ToLower().Contains("vo_keeper_station_dialogue_04"))
			{
				ShowSubtitles("Subway_Keeper_Dialogue_04", m_AudioSource.clip.length, isTrimmed: true);
			}
			else if (m_AudioSource.clip.name.ToLower().Contains("vo_keeper_station_dialogue_05"))
			{
				ShowSubtitles("Subway_Keeper_Dialogue_05", m_AudioSource.clip.length, isTrimmed: true);
			}
			else if (m_AudioSource.clip.name.ToLower().Contains("vo_keeper_station_dialogue_06"))
			{
				ShowSubtitles("Subway_Keeper_Dialogue_06", m_AudioSource.clip.length, isTrimmed: true);
			}
			else if (m_AudioSource.clip.name.ToLower().Contains("vo_keeper_station_dialogue_07"))
			{
				ShowSubtitles("Subway_Keeper_Dialogue_07", m_AudioSource.clip.length, isTrimmed: true);
			}
			else if (m_AudioSource.clip.name.ToLower().Contains("vo_audrey_wrong_book_01"))
			{
				ShowSubtitles("Cutscene_S124_ID15_Gilson_WrongBook_01", m_AudioSource.clip.length, isTrimmed: true);
			}
			else if (m_AudioSource.clip.name.ToLower().Contains("vo_audrey_wrong_book_02"))
			{
				ShowSubtitles("Cutscene_S124_ID15_Gilson_WrongBook_02", m_AudioSource.clip.length, isTrimmed: true);
			}
			else if (m_AudioSource.clip.name.ToLower().Contains("vo_audrey_wrong_book_03"))
			{
				ShowSubtitles("Cutscene_S124_ID15_Gilson_WrongBook_03", m_AudioSource.clip.length, isTrimmed: true);
			}
		}
		else if (!m_AudioSource.isPlaying && m_IsPlaying)
		{
			m_IsPlaying = false;
		}
	}

	private void ShowSubtitles(string key, float length, bool isTrimmed)
	{
		GameManager.Instance.ShowSubtitles(TextUtility.GetKey(key), length, isTrimmed);
	}
}
