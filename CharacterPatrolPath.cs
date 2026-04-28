using UnityEngine;

public class CharacterPatrolPath : JMonoBehaviour
{
	[SerializeField]
	private Transform[] m_Path;

	[SerializeField]
	private bool m_IsSinglePath;

	[SerializeField]
	private bool m_IsYoYo;

	public Transform[] Path => m_Path;

	public int PathLength => m_Path.Length;

	public bool IsSinglePath => m_IsSinglePath;

	public bool IsYoYo => m_IsYoYo;
}
