using S13Audio;
using UnityEngine;

public class PlayerEvents : JMonoBehaviour
{
	[SerializeField]
	private S13ScriptableEvent m_AbilitiesReturn;

	[SerializeField]
	private S13ScriptableEvent m_AbilitiesLose;

	public void AbilitiesReturn()
	{
		m_AbilitiesReturn?.Raise();
	}

	public void AbilitiesLose()
	{
		m_AbilitiesLose?.Raise();
	}
}
