using System;
using DG.Tweening;
using UnityEngine;

public class LightController : JMonoBehaviour
{
	[Serializable]
	public class LightGroup
	{
		public string Name;

		public bool IsStatic;

		public Light[] Lights;

		public LightFlicker[] LightFlickers;
	}

	[SerializeField]
	private LightGroup[] m_LightGroups;

	public event EventHandler OnTurnedOn;

	public event EventHandler OnTurnedOff;

	public void Activate()
	{
		CameraEffects.ShakeRotation(1f, 2f);
		TurnOn();
		DOTween.Sequence().InsertCallback(1.5f, delegate
		{
			TurnOff(light: true);
		});
		DOTween.Sequence().InsertCallback(3f, delegate
		{
			TurnOn();
		});
		DOTween.Sequence().InsertCallback(4.5f, delegate
		{
			TurnOn(light: true);
		});
	}

	public void TurnOn(bool light = false)
	{
		for (int i = 0; i < m_LightGroups.Length; i++)
		{
			LightGroup lightGroup = m_LightGroups[i];
			for (int j = 0; j < lightGroup.Lights.Length; j++)
			{
				lightGroup.Lights[j].gameObject.SetActive(value: true);
			}
			for (int k = 0; k < lightGroup.LightFlickers.Length; k++)
			{
				lightGroup.LightFlickers[k].TurnOn(light && lightGroup.IsStatic);
			}
		}
		this.OnTurnedOn.Send(this);
	}

	public void TurnOff(bool light = false)
	{
		for (int i = 0; i < m_LightGroups.Length; i++)
		{
			LightGroup lightGroup = m_LightGroups[i];
			for (int j = 0; j < lightGroup.Lights.Length; j++)
			{
				lightGroup.Lights[j].gameObject.SetActive(!light);
			}
			for (int k = 0; k < lightGroup.LightFlickers.Length; k++)
			{
				lightGroup.LightFlickers[k].TurnOff(light);
			}
		}
		this.OnTurnedOff.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnTurnedOn = null;
		this.OnTurnedOff = null;
		base.OnDisposed();
	}
}
