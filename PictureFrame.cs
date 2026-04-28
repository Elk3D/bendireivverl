using System;
using UnityEngine;

public class PictureFrame : JMonoBehaviour
{
	[SerializeField]
	private Interactable m_Interactable;

	[SerializeField]
	private ActiveSetter m_ActiveSetter;

	public event EventHandler OnComplete;

	public void Initialize()
	{
		m_Interactable.OnInteract -= HandleOnInteract;
		m_Interactable.OnInteract += HandleOnInteract;
		m_Interactable.SetActive(active: true);
	}

	private void HandleOnInteract(object sender, EventArgs e)
	{
		m_Interactable.OnInteract -= HandleOnInteract;
		m_ActiveSetter.SetActiveTrue();
		this.OnComplete.Send(this);
	}

	public void ForceComplete()
	{
		m_Interactable.OnInteract -= HandleOnInteract;
		m_Interactable.Dispose();
		m_ActiveSetter.SetActiveTrue();
	}

	protected override void OnDisposed()
	{
		this.OnComplete = null;
		if (m_Interactable != null)
		{
			m_Interactable.OnInteract -= HandleOnInteract;
		}
		base.OnDisposed();
	}
}
