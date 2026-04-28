using UnityEngine;

public class GroundSlam : JMonoBehaviour
{
	[SerializeField]
	private GameObject m_Content;

	public override void Awake()
	{
		Disable();
	}

	public void Enable()
	{
		m_Content?.SetActive(value: true);
	}

	public void Disable()
	{
		m_Content?.SetActive(value: false);
	}
}
