using UnityEngine;

public class MovablePath : JMonoBehaviour
{
	[SerializeField]
	private MovableLocation[] m_Path;

	[SerializeField]
	private MovableLocation m_StartLocation;

	public MovableLocation[] Path => m_Path;

	public MovableLocation StartLocation => m_StartLocation;

	public int GetStartLocation()
	{
		int result = 0;
		for (int i = 0; i < m_Path.Length; i++)
		{
			if (m_Path[i] == m_StartLocation)
			{
				result = i;
				break;
			}
		}
		return result;
	}
}
