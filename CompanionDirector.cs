using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-59)]
public class CompanionDirector : JMonoBehaviour
{
	[Header("ID")]
	[SerializeField]
	private int m_ID;

	[Header("Nodes")]
	[SerializeField]
	private Transform m_CompanionNodeParent;

	[Header("Scared")]
	[SerializeField]
	private CompanionNode m_ScaredNode;

	[Header("Animation Clips")]
	[SerializeField]
	private AnimationClipOverrideGroup[] m_AnimationClipOverrideGroup;

	[Header("Prefabs")]
	[SerializeField]
	private Companion m_CompanionPrefab;

	private Companion m_Companion;

	private List<CompanionNode> m_Locations = new List<CompanionNode>();

	private SectionID m_SectionID;

	private bool m_IsScared;

	private bool m_IsInitialized;

	public bool IsInitialized => m_IsInitialized;

	public bool IsActive { get; private set; }

	public void SetSectionID(SectionID sectionID)
	{
		m_SectionID = sectionID;
	}

	public void Initialize()
	{
		if (!m_IsInitialized)
		{
			StartCoroutine(InternalInitialize());
		}
	}

	private IEnumerator InternalInitialize()
	{
		SectionDataObject sectionData;
		CompanionDirectorDataObject companionDirectorData = GetCompanionDirectorData(out sectionData);
		if (companionDirectorData != null && companionDirectorData.DirectorState == 1)
		{
			yield return InternalLoadData();
		}
		yield return null;
	}

	public void LoadData()
	{
		StartCoroutine(InternalLoadData());
	}

	private IEnumerator InternalLoadData()
	{
		SectionDataObject sectionData;
		CompanionDirectorDataObject companionDirectorData = GetCompanionDirectorData(out sectionData);
		if (companionDirectorData != null)
		{
			InitializeDirector();
			CompanionNode node = null;
			for (int num = m_Locations.Count - 1; num >= 0; num--)
			{
				CompanionNode companionNode = m_Locations[num];
				if (companionNode.ID != companionDirectorData.NodeID)
				{
					foreach (int removedNode in companionDirectorData.RemovedNodes)
					{
						if (companionNode.ID == removedNode && companionNode.ID != companionDirectorData.NodeID)
						{
							companionNode.OnComplete -= HandleCompanionNodeOnComplete;
							m_Locations.Remove(companionNode);
							break;
						}
					}
				}
				else
				{
					node = companionNode;
				}
			}
			Transform transform = new GameObject().transform;
			transform.position = companionDirectorData.Transform.Position;
			transform.eulerAngles = companionDirectorData.Transform.EulerAngles;
			if (m_ScaredNode != null && companionDirectorData.NodeID == m_ScaredNode.ID)
			{
				node = m_ScaredNode;
			}
			Initialize(transform, node, companionDirectorData.NodeState == 1);
		}
		yield return null;
	}

	public void UnloadData(bool disposeCopanion = false)
	{
		SectionDataObject sectionData;
		CompanionDirectorDataObject companionDirectorData = GetCompanionDirectorData(out sectionData);
		if (companionDirectorData != null)
		{
			sectionData?.CompanionDirectorData.Remove(companionDirectorData);
		}
		IsActive = false;
		if (disposeCopanion && m_Companion != null)
		{
			m_Companion.Dispose();
			m_Companion = null;
		}
		ClearNodes();
	}

	public void UpdateData()
	{
		if (m_Companion != null)
		{
			SectionDataObject sectionData;
			CompanionDirectorDataObject companionDirectorData = GetCompanionDirectorData(out sectionData);
			if (companionDirectorData != null)
			{
				companionDirectorData.SetDirectorState(IsActive);
				companionDirectorData.SetTransform(m_Companion.transform);
				companionDirectorData.SetNodeID(m_Companion.CurrentNode.ID);
				companionDirectorData.SetNodeState(m_Companion.CurrentState == State.Character.Cutscene);
			}
		}
	}

	public void RemoveNodeData(int removeNode)
	{
		GetCompanionDirectorData(out var _)?.RemoveNode(removeNode);
	}

	private CompanionDirectorDataObject GetCompanionDirectorData(out SectionDataObject sectionData)
	{
		sectionData = null;
		CompanionDirectorDataObject result = null;
		sectionData = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(m_SectionID);
		if (sectionData != null)
		{
			result = (CompanionDirectorDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, CompanionDirectorDataObject>(sectionData.ID, m_ID);
		}
		return result;
	}

	public void Initialize(Transform startLocation, CompanionNode node = null, bool isActive = false)
	{
		if (!m_IsInitialized)
		{
			InitializeDirector();
		}
		if (m_Companion == null && GameManager.Instance.Bendy != null)
		{
			m_Companion = GameManager.Instance.Bendy;
		}
		if (m_Companion == null)
		{
			m_Companion = (GameManager.Instance.Bendy = GameManager.Instance.AssetManager.CreateAsset<Companion>(m_CompanionPrefab));
			m_Companion.SetParent(base.transform);
			m_Companion.transform.position = startLocation.position;
			m_Companion.transform.eulerAngles = startLocation.eulerAngles;
			m_Companion.Initialize();
			m_Companion.SetDirector(this);
			if (node != null)
			{
				if (isActive)
				{
					m_Companion.Agent.Agent.enabled = false;
					m_Companion.SetState(State.Character.Cutscene);
					node.OnNodeReached();
					m_Companion.NodeReached(node, isInstant: true);
				}
				else
				{
					m_Companion.Agent.Agent.enabled = true;
					m_Companion.SetState(State.Character.Companion);
				}
				m_Companion.SetNode(node);
			}
			else
			{
				m_Companion.SetNode(GetClosest(startLocation.position));
			}
		}
		else
		{
			m_Companion.ForceExitNode(isInitialize: true);
			m_Companion.Director.ClearNodes();
			m_Companion.SetNode(null);
			m_Companion.SetParent(base.transform);
			m_Companion.Director.UnloadData();
			m_Companion.SetDirector(this);
			UpdateNodes(m_CompanionNodeParent);
			m_Companion.SetNode(GetClosest(GameManager.Instance.Player.transform.position));
		}
		if (node == null)
		{
			CompanionDirectorDataObject companionDirectorData = GetCompanionDirectorData(out var sectionData);
			if (companionDirectorData == null)
			{
				companionDirectorData = DataObject<int, CompanionDirectorDataObject>.Create(m_ID);
				companionDirectorData.SetDirectorState(IsActive);
				companionDirectorData.SetTransform(m_Companion.transform);
				companionDirectorData.SetNodeID(m_Companion.CurrentNode.ID);
				companionDirectorData.SetNodeState(m_Companion.CurrentState == State.Character.Cutscene);
				sectionData.CompanionDirectorData.Add(m_ID, companionDirectorData);
			}
		}
	}

	private void InitializeDirector()
	{
		m_IsInitialized = true;
		IsActive = true;
		CompanionNode[] componentsInChildren = m_CompanionNodeParent.GetComponentsInChildren<CompanionNode>();
		foreach (CompanionNode companionNode in componentsInChildren)
		{
			m_Locations.Add(companionNode);
			companionNode.OnComplete -= HandleCompanionNodeOnComplete;
			companionNode.OnComplete += HandleCompanionNodeOnComplete;
		}
		for (int j = 0; j < m_AnimationClipOverrideGroup.Length; j++)
		{
			m_AnimationClipOverrideGroup[j].Initialize();
		}
	}

	private void HandleCompanionNodeOnComplete(object sender, EventArgs e)
	{
		CompanionNode companionNode = (CompanionNode)sender;
		companionNode.OnComplete -= HandleCompanionNodeOnComplete;
		if (m_Locations.Contains(companionNode))
		{
			m_Locations.Remove(companionNode);
			m_Companion.OnAnimationComplete -= HandleCompanionOnAnimationComplete;
			m_Companion.OnAnimationComplete += HandleCompanionOnAnimationComplete;
			UpdateAnimationClips(companionNode.EventCompleteReaction);
			m_Companion.SetAnimationTrigger("Interact");
			RemoveNodeData(companionNode.ID);
		}
	}

	private void HandleCompanionOnAnimationComplete(object sender, EventArgs e)
	{
		m_Companion.OnAnimationComplete -= HandleCompanionOnAnimationComplete;
		m_Companion.SetNode(null);
		m_Companion.Agent.Agent.enabled = true;
		m_Companion.SetState(State.Character.Companion);
	}

	public void UpdateNodes(Transform nodeParent)
	{
		ClearNodes();
		m_Locations = new List<CompanionNode>();
		CompanionNode[] componentsInChildren = nodeParent.GetComponentsInChildren<CompanionNode>();
		foreach (CompanionNode companionNode in componentsInChildren)
		{
			m_Locations.Add(companionNode);
			companionNode.OnComplete -= HandleCompanionNodeOnComplete;
			companionNode.OnComplete += HandleCompanionNodeOnComplete;
		}
	}

	public void ClearNodes()
	{
		if (m_Locations != null)
		{
			for (int i = 0; i < m_Locations.Count; i++)
			{
				m_Locations[i].OnComplete -= HandleCompanionNodeOnComplete;
			}
			m_Locations.Clear();
			m_Locations = null;
		}
	}

	private void Update()
	{
		if (m_Companion == null || m_Locations == null || GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused || m_Companion.CurrentState == State.Character.Cutscene)
		{
			return;
		}
		if (GameManager.Instance.Player.CombatStatus != CombatStatus.Combat)
		{
			if (m_IsScared)
			{
				m_IsScared = false;
				UpdateAnimationClips("NotScared");
				m_Companion.Movement.SetRunSpeed(10f);
			}
			CompanionNode closest = GetClosest(GameManager.Instance.Player.transform.position);
			if (m_Companion.CurrentNode != closest && closest != null)
			{
				if (m_Companion.CurrentNode != null && m_Companion.CurrentNode is CompanionNode companionNode)
				{
					companionNode.Disable();
				}
				m_Companion.SetNode(closest);
			}
			if (m_Companion.CurrentNode != null && Vector3.Distance(m_Companion.transform.position, m_Companion.CurrentNode.Position) <= m_Companion.CharacterVision.CloseRange)
			{
				m_Companion.CurrentNode.OnNodeReached();
			}
		}
		else if (m_ScaredNode != null && m_Companion.CurrentNode != m_ScaredNode && !m_IsScared)
		{
			m_IsScared = true;
			UpdateAnimationClips("Scared");
			m_Companion.SetNode(m_ScaredNode);
			m_Companion.Movement.SetRunSpeed(2f);
		}
	}

	public CompanionNode GetClosest(Vector3 _currentPosition)
	{
		CompanionNode result = null;
		float num = float.PositiveInfinity;
		foreach (CompanionNode location in m_Locations)
		{
			float num2 = Vector3.Distance(location.transform.position, _currentPosition);
			if (num2 < num)
			{
				result = location;
				num = num2;
			}
		}
		return result;
	}

	public void UpdateAnimationClips(string name)
	{
		List<AnimationClip> list = new List<AnimationClip>();
		for (int i = 0; i < m_AnimationClipOverrideGroup.Length; i++)
		{
			AnimationClipOverrideGroup animationClipOverrideGroup = m_AnimationClipOverrideGroup[i];
			if (animationClipOverrideGroup.Name == name)
			{
				for (int j = 0; j < animationClipOverrideGroup.AnimationClipGroups.Length; j++)
				{
					list.Add(animationClipOverrideGroup.AnimationClipGroups[j].AnimationClip);
				}
				break;
			}
		}
		m_Companion.UpdateAnimationClipOverrides(list.ToArray());
	}

	public void UpdateNode(CompanionNode node)
	{
		if (m_Companion.CurrentNode != node)
		{
			m_Companion.SetNode(node);
		}
	}

	protected override void OnDisposed()
	{
		if (m_Companion != null)
		{
			m_Companion.OnAnimationComplete -= HandleCompanionOnAnimationComplete;
			m_Companion = null;
		}
		if (m_Locations != null)
		{
			m_Locations.Clear();
			m_Locations = null;
		}
		base.OnDisposed();
	}
}
