using UnityEngine;

public class BulletLine : JComponent
{
	[SerializeField]
	private GameObject m_Content;

	[SerializeField]
	private GameObject m_Muzzle;

	public override void Awake()
	{
		SetActive(active: false);
	}

	public void SetActive(bool active)
	{
		m_Content.SetActive(active);
		m_Muzzle.SetActive(active);
	}

	public void Enable(float delay)
	{
		Invoke("InvokeEnable", delay);
	}

	private void InvokeEnable()
	{
		SetActive(active: true);
		Disable(0.015f);
	}

	public void Disable(float delay)
	{
		Invoke("InvokeDisable", delay);
	}

	private void InvokeDisable()
	{
		SetActive(active: false);
	}
}
