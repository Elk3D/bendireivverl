using UnityEngine;

[CreateAssetMenu(menuName = "Character/Enemy Selector")]
public class EnemySelector : ScriptableObject
{
	[SerializeField]
	private Enemy m_Prefab;

	[SerializeField]
	private CharacterContent[] m_Prefabs;

	private int m_SelectedIndex = -1;

	public Enemy Get()
	{
		Enemy enemy = Object.Instantiate(m_Prefab);
		CharacterContent content = GetContent();
		content.transform.SetParent(enemy.transform);
		content.transform.localPosition = Vector3.zero;
		content.transform.localEulerAngles = Vector3.zero;
		content.transform.localScale = Vector3.one;
		return enemy;
	}

	private CharacterContent GetContent()
	{
		int num = Random.Range(0, m_Prefabs.Length);
		if (m_Prefabs.Length > 1)
		{
			while (num == m_SelectedIndex)
			{
				num = Random.Range(0, m_Prefabs.Length);
			}
		}
		m_SelectedIndex = num;
		return Object.Instantiate(m_Prefabs[m_SelectedIndex]);
	}
}
