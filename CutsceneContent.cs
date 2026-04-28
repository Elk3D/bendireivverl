using System;
using UnityEngine;

public class CutsceneContent : JMonoBehaviour
{
	[SerializeField]
	private CutsceneDirector m_Director;

	[SerializeField]
	private CutsceneActivator m_Activator;

	[SerializeField]
	private GameObject m_InitializeContent;

	[SerializeField]
	private Transform m_StartLocation;

	[SerializeField]
	private bool m_CompleteOnPlay;

	public CutsceneDirector Director => m_Director;

	public CutsceneActivator Activator => m_Activator;

	public GameObject InitializeContent => m_InitializeContent;

	public Transform StartLocation => m_StartLocation;

	public bool CompleteOnPlay => m_CompleteOnPlay;

	public event EventHandler OnForceComplete;

	public void ForceComplete()
	{
		this.OnForceComplete.Send(this);
	}

	public void Initialize()
	{
		if (m_InitializeContent != null)
		{
			m_InitializeContent.SetActive(value: true);
		}
	}

	protected override void OnDisposed()
	{
		this.OnForceComplete = null;
		base.OnDisposed();
	}
}
