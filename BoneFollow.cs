using System;
using UnityEngine;

public class BoneFollow : JMonoBehaviour
{
	[Serializable]
	public class BoneSimulator
	{
		public Transform Simulation;

		public Transform Bone;
	}

	[SerializeField]
	private BoneSimulator[] m_BoneConnectors;

	public bool IsActive { get; private set; }

	public override void Start()
	{
		if (m_BoneConnectors.Length != 0)
		{
			m_BoneConnectors[0].Simulation.SetParent(null);
		}
		IsActive = true;
	}

	private void LateUpdate()
	{
		if (IsActive && !(GameManager.Instance.Player == null) && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			for (int i = 0; i < m_BoneConnectors.Length; i++)
			{
				BoneSimulator boneSimulator = m_BoneConnectors[i];
				boneSimulator.Bone.position = boneSimulator.Simulation.position;
				boneSimulator.Bone.eulerAngles = boneSimulator.Simulation.eulerAngles;
			}
		}
	}

	public void Enable()
	{
		for (int i = 0; i < m_BoneConnectors.Length; i++)
		{
			BoneSimulator boneSimulator = m_BoneConnectors[i];
			Rigidbody component = boneSimulator.Bone.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = false;
			}
			Collider component2 = boneSimulator.Bone.GetComponent<Collider>();
			if (component2 != null)
			{
				component2.enabled = true;
			}
			boneSimulator.Bone.position = boneSimulator.Simulation.position;
			boneSimulator.Bone.eulerAngles = boneSimulator.Simulation.eulerAngles;
		}
		IsActive = true;
	}

	public void Disable()
	{
		IsActive = false;
		for (int i = 0; i < m_BoneConnectors.Length; i++)
		{
			BoneSimulator obj = m_BoneConnectors[i];
			Rigidbody component = obj.Bone.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = true;
			}
			Collider component2 = obj.Bone.GetComponent<Collider>();
			if (component2 != null)
			{
				component2.enabled = false;
			}
		}
	}

	public void ClearSimulations()
	{
		for (int i = 0; i < m_BoneConnectors.Length; i++)
		{
			BoneSimulator boneSimulator = m_BoneConnectors[i];
			if (boneSimulator != null && boneSimulator.Simulation != null)
			{
				UnityEngine.Object.Destroy(boneSimulator.Simulation.gameObject);
			}
		}
	}

	[ContextMenu("Get Bones")]
	private void GetBones()
	{
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (transform.name.ToLower().Contains("bn_pipe") && transform.name.ToLower().Contains("04"))
			{
				m_BoneConnectors[0].Bone = transform;
			}
			else if (transform.name.ToLower().Contains("04"))
			{
				m_BoneConnectors[0].Simulation = transform;
				m_BoneConnectors[0].Simulation.localPosition = m_BoneConnectors[0].Bone.localPosition;
			}
			else if (transform.name.ToLower().Contains("bn_pipe") && transform.name.ToLower().Contains("05"))
			{
				m_BoneConnectors[1].Bone = transform;
			}
			else if (transform.name.ToLower().Contains("05"))
			{
				m_BoneConnectors[1].Simulation = transform;
				m_BoneConnectors[1].Simulation.localPosition = m_BoneConnectors[1].Bone.localPosition;
			}
			else if (transform.name.ToLower().Contains("bn_pipe") && transform.name.ToLower().Contains("06"))
			{
				m_BoneConnectors[2].Bone = transform;
			}
			else if (transform.name.ToLower().Contains("06"))
			{
				m_BoneConnectors[2].Simulation = transform;
				m_BoneConnectors[2].Simulation.localPosition = m_BoneConnectors[2].Bone.localPosition;
			}
			else if (transform.name.ToLower().Contains("bn_pipe") && transform.name.ToLower().Contains("07"))
			{
				m_BoneConnectors[3].Bone = transform;
			}
			else if (transform.name.ToLower().Contains("07"))
			{
				m_BoneConnectors[3].Simulation = transform;
				m_BoneConnectors[3].Simulation.localPosition = m_BoneConnectors[3].Bone.localPosition;
			}
			else if (transform.name.ToLower().Contains("bn_pipe") && transform.name.ToLower().Contains("08"))
			{
				m_BoneConnectors[4].Bone = transform;
			}
			else if (transform.name.ToLower().Contains("08"))
			{
				m_BoneConnectors[4].Simulation = transform;
				m_BoneConnectors[4].Simulation.localPosition = m_BoneConnectors[4].Bone.localPosition;
			}
		}
	}
}
