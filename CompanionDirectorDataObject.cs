using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CompanionDirectorDataObject : DataObject<int, CompanionDirectorDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private int m_DirectorState;

	[SerializeField]
	private TransformData m_Transform;

	[SerializeField]
	private int m_NodeID;

	[SerializeField]
	private int m_NodeState;

	[SerializeField]
	private List<int> m_RemovedNodes = new List<int>();

	public override int ID => m_DataID;

	public int DirectorState => m_DirectorState;

	public TransformData Transform => m_Transform;

	public int NodeID => m_NodeID;

	public int NodeState => m_NodeState;

	public List<int> RemovedNodes => m_RemovedNodes;

	public void SetDirectorState(bool active)
	{
		m_DirectorState = (active ? 1 : 0);
	}

	public void SetTransform(Transform transform)
	{
		m_Transform = TransformData.Get(transform);
	}

	public void SetNodeID(int id)
	{
		m_NodeID = id;
	}

	public void SetNodeState(bool active)
	{
		m_NodeState = (active ? 1 : 0);
	}

	public void RemoveNode(int nodeID)
	{
		if (!m_RemovedNodes.Contains(nodeID))
		{
			m_RemovedNodes.Add(nodeID);
		}
	}

	protected override void Deserialize()
	{
		m_DataID = m_ID;
	}
}
