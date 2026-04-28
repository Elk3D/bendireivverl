using System;
using UnityEngine;

public class UIConsole : UIController
{
	[SerializeField]
	private VideoController m_Video;

	protected override void OnInitialized(object _data)
	{
		m_Video.SetCamera(GameManager.Instance.UIManager.Camera);
	}

	public override void PlayIn()
	{
		m_Video.Play();
		m_Video.OnComplete -= HandleVideoOnComplete;
		m_Video.OnComplete += HandleVideoOnComplete;
		PlayInComplete();
	}

	private void HandleVideoOnComplete(object sender, EventArgs e)
	{
		m_Video.OnComplete -= HandleVideoOnComplete;
		PlayOut();
	}

	private void Update()
	{
		if (!base.IsDisposed && (Input.GetMouseButtonDown(2) || PlayerInput.ControllerRightBumper()))
		{
			m_Video.OnComplete -= HandleVideoOnComplete;
			m_Video.Stop();
			PlayOut();
		}
	}

	protected override void OnDisposed()
	{
		m_Video.OnComplete -= HandleVideoOnComplete;
		base.OnDisposed();
	}
}
