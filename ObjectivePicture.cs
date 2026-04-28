using System;
using UnityEngine;

public class ObjectivePicture : Objective
{
	[SerializeField]
	private Picture m_Picture;

	protected override void InternalEnable()
	{
		Initialize();
	}

	protected override void InternalInitialize()
	{
		RemoveListeners();
		m_Picture.OnActivate += HandleOnActivate;
	}

	private void HandleOnActivate(object sender, EventArgs e)
	{
		m_Picture.OnActivate -= HandleOnActivate;
		SendOnComplete();
	}

	protected override void RemoveListeners()
	{
		m_Picture.OnActivate -= HandleOnActivate;
	}
}
