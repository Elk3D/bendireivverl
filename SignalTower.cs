using System;
using UnityEngine;

public class SignalTower : JMonoBehaviour
{
	[SerializeField]
	private bool m_IsActive = true;

	[SerializeField]
	private Transform m_Lighting;

	public event EventHandler On;

	public event EventHandler Off;

	public override void Awake()
	{
		if (!m_IsActive)
		{
			TurnOff();
		}
	}

	public void TurnOn()
	{
		SetLighting(active: true);
		this.On.Send(this);
	}

	public void TurnOff()
	{
		SetLighting(active: false);
		this.Off.Send(this);
	}

	private void SetLighting(bool active)
	{
		m_Lighting.gameObject.SetActive(active);
	}
}
