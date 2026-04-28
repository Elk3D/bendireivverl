using System;
using UnityEngine;

[Serializable]
public class TransformData
{
	[SerializeField]
	private Vector3 m_Position;

	[SerializeField]
	private Quaternion m_Rotation;

	[SerializeField]
	private Vector3 m_EulerAngles;

	[SerializeField]
	private Vector3 m_LocalScale;

	public Vector3 Position => m_Position;

	public Quaternion Rotation => m_Rotation;

	public Vector3 EulerAngles => m_EulerAngles;

	public Vector3 LocalScale => m_LocalScale;

	public static TransformData Get(Transform transform)
	{
		return new TransformData
		{
			m_Position = transform.position,
			m_Rotation = transform.rotation,
			m_EulerAngles = transform.eulerAngles,
			m_LocalScale = transform.localScale
		};
	}
}
