using System;
using DG.Tweening;
using UnityEngine;

public class EnemyFleeController : JMonoBehaviour
{
	[SerializeField]
	private bool m_IsActive;

	[SerializeField]
	private Transform m_NodeParent;

	[SerializeField]
	private Transform m_InkBulbParent;

	private CharacterNode[] m_Nodes;

	private InkBulb[] m_InkBulbs;

	public override void Awake()
	{
		m_Nodes = m_NodeParent.GetComponentsInChildren<CharacterNode>(includeInactive: true);
		m_InkBulbs = m_InkBulbParent.GetComponentsInChildren<InkBulb>(includeInactive: true);
		if (m_InkBulbs != null)
		{
			for (int i = 0; i < m_InkBulbs.Length; i++)
			{
				InkBulb obj = m_InkBulbs[i];
				obj.OnFlee -= HandleInkBulbOnFlee;
				obj.OnFlee += HandleInkBulbOnFlee;
			}
		}
	}

	public void SetActive(bool active)
	{
		m_IsActive = active;
	}

	public void Flee(bool deactivate = false)
	{
		if (m_Nodes == null || m_InkBulbs == null)
		{
			return;
		}
		CharacterNode characterNode = m_Nodes[UnityEngine.Random.Range(0, m_Nodes.Length)];
		CharacterNode lastNode = characterNode;
		for (int i = 0; i < m_InkBulbs.Length; i++)
		{
			InkBulb inkBulb = m_InkBulbs[i];
			for (int j = 0; j < inkBulb.InkWidows.Count; j++)
			{
				Enemy inkWidow = (Enemy)inkBulb.InkWidows[j];
				Flee(inkBulb, inkWidow, characterNode, ref lastNode);
			}
			if (inkBulb.gameObject.activeInHierarchy && deactivate)
			{
				inkBulb.Deactivate();
			}
		}
	}

	public void Flee(InkBulb inkBulb)
	{
		CharacterNode characterNode = m_Nodes[UnityEngine.Random.Range(0, m_Nodes.Length)];
		CharacterNode lastNode = characterNode;
		for (int i = 0; i < inkBulb.InkWidows.Count; i++)
		{
			Enemy inkWidow = (Enemy)inkBulb.InkWidows[i];
			Flee(inkBulb, inkWidow, characterNode, ref lastNode);
		}
	}

	private void Flee(InkBulb inkBulb, Enemy inkWidow, CharacterNode node, ref CharacterNode lastNode)
	{
		if (!inkWidow.gameObject.activeInHierarchy)
		{
			return;
		}
		if (inkWidow.CurrentState != State.Character.Death)
		{
			if (inkWidow.IsInitialized)
			{
				inkWidow.CancelPath();
				inkWidow.SetTarget(null);
				if (m_Nodes.Length > 1)
				{
					while (node == lastNode)
					{
						node = m_Nodes[UnityEngine.Random.Range(0, m_Nodes.Length)];
					}
				}
				lastNode = node;
				inkWidow.SetDisableVision(active: true);
				inkWidow.SetNode(node);
				inkWidow.OnNodeReached -= HandleEnemyOnNodeReached;
				inkWidow.OnNodeReached += HandleEnemyOnNodeReached;
				inkWidow.OnCharacterDeath -= HandleEnemyOnCharacterDeath;
				inkWidow.OnCharacterDeath += HandleEnemyOnCharacterDeath;
				inkWidow.SetState(State.Character.Flee);
			}
			else
			{
				KillInkWidow(inkBulb, inkWidow);
			}
		}
		else
		{
			KillInkWidow(inkBulb, inkWidow);
		}
	}

	private void HandleEnemyOnNodeReached(object sender, EventArgs e)
	{
		Character character = (Character)sender;
		character.OnNodeReached -= HandleEnemyOnNodeReached;
		character.SetState(State.Character.Cutscene);
		character.Agent.Agent.isStopped = true;
		character.Agent.Agent.enabled = false;
		character.Controller.enabled = false;
		character.Content.Animator.SetMovementState(1f, smooth: false);
		character.Content.Animator.SetMovementSpeed(2f, smooth: false);
		character.transform.DOKill();
		character.transform.DOLocalMove(character.CurrentNode.transform.position + character.CurrentNode.transform.forward * 4f, 1f).SetEase(Ease.Linear).OnComplete(delegate
		{
			KillInkWidow(character);
		});
	}

	private void HandleEnemyOnCharacterDeath(object sender, EventArgs e)
	{
		Enemy obj = sender as Enemy;
		obj.OnCharacterDeath -= HandleEnemyOnCharacterDeath;
		obj.OnNodeReached -= HandleEnemyOnNodeReached;
	}

	private void HandleInkBulbOnFlee(object sender, EventArgs e)
	{
		if (m_IsActive)
		{
			InkBulb inkBulb = sender as InkBulb;
			Flee(inkBulb);
		}
	}

	private void KillInkWidow(InkBulb inkBulb, Character inkWidow)
	{
		if (!(inkWidow == null))
		{
			inkBulb?.KillInkWidow(inkWidow);
		}
	}

	private void KillInkWidow(Character inkWidow)
	{
		if (!(inkWidow == null))
		{
			for (int i = 0; i < m_InkBulbs.Length; i++)
			{
				m_InkBulbs[i].KillInkWidow(inkWidow);
			}
		}
	}

	private void RemoveListeners()
	{
		if (m_InkBulbs != null)
		{
			for (int i = 0; i < m_InkBulbs.Length; i++)
			{
				m_InkBulbs[i].OnFlee -= HandleInkBulbOnFlee;
			}
		}
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		m_Nodes = null;
		m_InkBulbs = null;
		base.OnDisposed();
	}
}
