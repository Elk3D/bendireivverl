using System;
using UnityEngine;

public class ObjectivePictureFrame : Objective
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Picture Frame")]
	[SerializeField]
	private PictureFrame m_PictureFrame;

	protected override void InternalEnable()
	{
		Initialize();
	}

	protected override void InternalInitialize()
	{
		JDebug.Log("ObjectivePictureFrame :: InternalInitialize", this, JDebug.JDebugType.Objectives);
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		if (!CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
		}
	}

	private bool CheckStatus()
	{
		bool flag = true;
		if (m_Requirements != null)
		{
			flag = m_Requirements.IsComplete();
		}
		if (flag)
		{
			GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
			m_PictureFrame.OnComplete -= HandlePictureFrameOnComplete;
			m_PictureFrame.OnComplete += HandlePictureFrameOnComplete;
			m_PictureFrame.Initialize();
		}
		return flag;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("ObjectivePictureFrame :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
		CheckStatus();
	}

	protected override void InternalForceComplete()
	{
		JDebug.Log("ObjectivePictureFrame :: InternalForceComplete", this, JDebug.JDebugType.Objectives);
		m_PictureFrame.ForceComplete();
		SendOnComplete();
	}

	private void HandlePictureFrameOnComplete(object sender, EventArgs e)
	{
		m_PictureFrame.OnComplete -= HandlePictureFrameOnComplete;
		SendOnComplete();
	}

	protected override void RemoveListeners()
	{
		m_PictureFrame.OnComplete -= HandlePictureFrameOnComplete;
		base.RemoveListeners();
	}
}
