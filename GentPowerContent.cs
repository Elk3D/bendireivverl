using System;
using UnityEngine;

public class GentPowerContent : JMonoBehaviour
{
	[SerializeField]
	private GentPowerDirector m_Director;

	[SerializeField]
	private CutsceneActivator m_Activator;

	[SerializeField]
	private Transform m_StartLocation;

	public GentPowerDirector Director => m_Director;

	public CutsceneActivator Activator => m_Activator;

	public Transform StartLocation => m_StartLocation;

	public event EventHandler OnForceComplete;

	public void ForceComplete()
	{
		this.OnForceComplete.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnForceComplete = null;
		base.OnDisposed();
	}
}
