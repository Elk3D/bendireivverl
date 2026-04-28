using System;
using UnityEngine;

public class LightBulbController : JMonoBehaviour
{
	[SerializeField]
	private Light m_Light;

	[SerializeField]
	private MeshRenderer m_LightRenderer;

	[SerializeField]
	private int m_MaterialIndex = 1;

	[SerializeField]
	private bool m_IsOn = true;

	public bool IsOn => m_Light.enabled;

	public event EventHandler OnTurnedOn;

	public event EventHandler OnTurnedOff;

	public override void Start()
	{
		if (!m_IsOn)
		{
			TurnOff();
		}
	}

	public void TurnOn()
	{
		m_LightRenderer.materials[m_MaterialIndex].SetInt("_Power", 1);
		m_Light.gameObject.SetActive(value: true);
		this.OnTurnedOn.Send(this);
	}

	public void TurnOff()
	{
		m_LightRenderer.materials[m_MaterialIndex].SetInt("_Power", 0);
		m_Light.gameObject.SetActive(value: false);
		this.OnTurnedOff.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnTurnedOn = null;
		this.OnTurnedOff = null;
		base.OnDisposed();
	}
}
