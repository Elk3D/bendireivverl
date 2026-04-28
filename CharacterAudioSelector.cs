using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Character Audio Selector")]
public class CharacterAudioSelector : ScriptableObject
{
	[SerializeField]
	private GameObject[] m_Prefabs;

	private List<int> m_History = new List<int>();

	private int m_SelectedIndex = -1;

	public GameObject Get()
	{
		int num = Random.Range(0, m_Prefabs.Length);
		if (m_Prefabs.Length > 1)
		{
			if (m_History.Count >= m_Prefabs.Length)
			{
				m_History.Clear();
			}
			while (num == m_SelectedIndex && m_History.Contains(num))
			{
				num = Random.Range(0, m_Prefabs.Length);
			}
		}
		m_SelectedIndex = num;
		if (!m_History.Contains(m_SelectedIndex))
		{
			m_History.Add(m_SelectedIndex);
		}
		return Object.Instantiate(m_Prefabs[m_SelectedIndex]);
	}
}
