using System;
using UnityEngine;

[Serializable]
public class CameraFOV : JDisposable
{
	[SerializeField]
	private bool m_Active = true;

	[SerializeField]
	private float m_BaseFOV = 55f;

	[SerializeField]
	private float m_RunFOV = 60f;

	[SerializeField]
	private float m_TransitionSpeed = 0.5f;

	private Camera[] m_Cameras;

	public float FOV
	{
		get
		{
			if (m_Cameras != null && m_Cameras.Length != 0)
			{
				return m_Cameras[0].fieldOfView;
			}
			return 0f;
		}
	}

	public void Init(params Camera[] cameras)
	{
		if (m_Active)
		{
			m_Cameras = cameras;
			SetFOV(m_BaseFOV);
		}
	}

	public void UpdateVOD(bool isRunning)
	{
		if (!m_Active)
		{
			return;
		}
		if (isRunning)
		{
			if (!(FOV >= m_RunFOV))
			{
				SetFOV(DORun());
			}
		}
		else if (!(FOV <= m_BaseFOV))
		{
			SetFOV(DOBase());
		}
	}

	private float DORun()
	{
		return FOV + m_TransitionSpeed;
	}

	private float DOBase()
	{
		return FOV - m_TransitionSpeed;
	}

	public void SetFOV(float value)
	{
		for (int i = 0; i < m_Cameras.Length; i++)
		{
			if (m_Cameras[i] != null)
			{
				m_Cameras[i].fieldOfView = value;
			}
		}
	}

	public void SetActiveFOV(bool active)
	{
		m_Active = active;
	}

	public void ResetFOV()
	{
		SetFOV(m_BaseFOV);
	}

	protected override void OnDisposed()
	{
		m_Cameras = null;
		base.OnDisposed();
	}
}
