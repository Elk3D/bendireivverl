using UnityEngine;

public class TimelineChapterTitle : JMonoBehaviour
{
	[SerializeField]
	private string m_Chapter;

	[SerializeField]
	private string m_Title;

	public void Action()
	{
		m_Chapter += ":";
		GameManager.Instance.ShowChapterTitle(m_Chapter, m_Title);
	}
}
