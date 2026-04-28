using System;
using UnityEngine;

[Serializable]
public class PlayerTransform
{
	[SerializeField]
	private Vector3 m_Position = Vector3.zero;

	[SerializeField]
	private float m_Rotation;

	[SerializeField]
	private float m_HeadRotation;

	public Vector3 Position => m_Position;

	public Vector3 Rotation => new Vector3(0f, m_Rotation, 0f);

	public Vector3 HeadRotation => new Vector3(m_HeadRotation, 0f, 0f);

	public void Update()
	{
		Transform transform = GameManager.Instance.Player.transform;
		m_Position = transform.position;
		m_Rotation = transform.localEulerAngles.y;
		m_HeadRotation = GameManager.Instance.Player.HeadContainer.localEulerAngles.x;
	}
}
