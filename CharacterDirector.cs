using System;
using System.Collections;
using DG.Tweening;
using S13Audio;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(-59)]
public class CharacterDirector : JMonoBehaviour
{
	[Header("ID")]
	[SerializeField]
	private int m_ID;

	[Header("Director Settings")]
	[SerializeField]
	private bool m_OnStart = true;

	[SerializeField]
	private bool m_SmoothLocationNode;

	[Header("Pathing")]
	[SerializeField]
	private Transform m_StartLocation;

	[SerializeField]
	private CharacterPatrolPath m_PatrolPath;

	[SerializeField]
	private Transform m_SpecialPath;

	[Header("Audio")]
	[SerializeField]
	private S13AudioHandler m_S13AudioHandler;

	[Header("Prefab")]
	[SerializeField]
	private Character m_Character;

	private SectionID m_SectionID;

	private Enemy m_Enemy;

	private CharacterInteractionNode m_CurrentNode;

	private int m_CurrentPatrolPathPoint;

	private bool m_HasSpecialPath;

	private bool m_IsInitialized;

	private bool m_IsYoYoing;

	private float m_BrokenPathTimer;

	private float m_BrokenPathTimerLimit = 3f;

	public bool OnStart => m_OnStart;

	public bool IsInitialized => m_IsInitialized;

	public event EventHandler OnNodeReached;

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
		m_IsInitialized = true;
		SectionDataObject sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(m_SectionID);
		if (sectionDataObject != null)
		{
			CharacterDirectorDataObject characterDirectorDataObject = (CharacterDirectorDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, CharacterDirectorDataObject>(sectionDataObject.ID, m_ID);
			if (characterDirectorDataObject == null)
			{
				m_Enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>(m_Character);
				m_Enemy.transform.SetParent(base.transform);
				m_Enemy.transform.position = m_StartLocation.position;
				m_Enemy.transform.eulerAngles = m_StartLocation.eulerAngles;
				m_Enemy.SetTarget(m_PatrolPath.Path[m_CurrentPatrolPathPoint]);
				m_Enemy.Initialize();
				characterDirectorDataObject = DataObject<int, CharacterDirectorDataObject>.Create(m_ID);
				characterDirectorDataObject.SetEnemyType(m_Enemy.EnemyType);
				if (m_Enemy.Content is LostOneCharacterContent lostOneCharacterContent)
				{
					characterDirectorDataObject.SetLostOneType(lostOneCharacterContent.LostOneType);
				}
				characterDirectorDataObject.SetTransform(m_Enemy.transform);
				characterDirectorDataObject.SetPathIndex(m_CurrentPatrolPathPoint);
				characterDirectorDataObject.SetSpecialPath(m_HasSpecialPath);
				sectionDataObject.CharacterDirectorData.Add(m_ID, characterDirectorDataObject);
			}
			else
			{
				yield return InternalLoadData();
			}
		}
		yield return null;
	}

	public void LoadData()
	{
		StartCoroutine(InternalLoadData());
	}

	private IEnumerator InternalLoadData()
	{
		m_IsInitialized = true;
		SectionDataObject sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(m_SectionID);
		if (sectionDataObject != null)
		{
			CharacterDirectorDataObject characterDirectorDataObject = (CharacterDirectorDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, CharacterDirectorDataObject>(sectionDataObject.ID, m_ID);
			if (characterDirectorDataObject != null)
			{
				m_CurrentPatrolPathPoint = characterDirectorDataObject.CurrentPathIndex;
				m_HasSpecialPath = characterDirectorDataObject.HasSpecialPath;
				m_Enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>(PrefabCheck.GetEnemy(characterDirectorDataObject.EnemyType));
				m_Enemy.transform.SetParent(base.transform);
				if (characterDirectorDataObject.EnemyType == EnemyType.LostOne || characterDirectorDataObject.EnemyType == EnemyType.LostOneColor)
				{
					CharacterContent characterContent = GameManager.Instance.AssetManager.CreateAsset<CharacterContent>(PrefabCheck.GetCharacter(characterDirectorDataObject.EnemyType, characterDirectorDataObject.LostOneType));
					characterContent.transform.SetParent(m_Enemy.transform);
					characterContent.transform.localPosition = Vector3.zero;
					characterContent.transform.localEulerAngles = Vector3.zero;
					characterContent.transform.localScale = Vector3.one;
				}
				m_Enemy.transform.position = characterDirectorDataObject.Transform.Position;
				m_Enemy.transform.rotation = characterDirectorDataObject.Transform.Rotation;
				if (m_HasSpecialPath && m_SpecialPath != null)
				{
					m_Enemy.SetTarget(m_SpecialPath);
				}
				else
				{
					m_Enemy.SetTarget(m_PatrolPath.Path[characterDirectorDataObject.CurrentPathIndex]);
				}
				m_Enemy.Initialize();
			}
		}
		yield return null;
	}

	public void UnloadData()
	{
		SectionDataObject sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(m_SectionID);
		if (sectionDataObject == null)
		{
			return;
		}
		CharacterDirectorDataObject characterDirectorDataObject = (CharacterDirectorDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, CharacterDirectorDataObject>(sectionDataObject.ID, m_ID);
		if (characterDirectorDataObject != null)
		{
			sectionDataObject.CharacterDirectorData.Remove(characterDirectorDataObject);
			if (m_Enemy != null)
			{
				m_Enemy.OnAnimationComplete -= HandleAnimationComplete;
				m_Enemy.Dispose();
				m_Enemy = null;
			}
		}
	}

	public void UpdateData()
	{
		if (!(m_Enemy != null))
		{
			return;
		}
		SectionDataObject sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(m_SectionID);
		if (sectionDataObject == null)
		{
			return;
		}
		CharacterDirectorDataObject characterDirectorDataObject = (CharacterDirectorDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, CharacterDirectorDataObject>(sectionDataObject.ID, m_ID);
		if (characterDirectorDataObject != null)
		{
			characterDirectorDataObject.SetEnemyType(m_Enemy.EnemyType);
			if (m_Enemy.Content is LostOneCharacterContent lostOneCharacterContent)
			{
				characterDirectorDataObject.SetLostOneType(lostOneCharacterContent.LostOneType);
			}
			if (GameManager.Instance.Player != null)
			{
				characterDirectorDataObject.SetCombatState(m_Enemy.Target == GameManager.Instance.Player.transform);
			}
			characterDirectorDataObject.SetTransform(m_Enemy.transform);
			characterDirectorDataObject.SetPathIndex(m_CurrentPatrolPathPoint);
			characterDirectorDataObject.SetSpecialPath(m_HasSpecialPath);
		}
	}

	public void InitializeNoData()
	{
		m_IsInitialized = true;
		m_Enemy = m_Character as Enemy;
		m_Enemy.SetTarget(m_PatrolPath.Path[0]);
		m_Enemy.Initialize();
	}

	private void Update()
	{
		if (!m_IsInitialized || m_Enemy == null || m_Enemy.CurrentState == State.Character.Death || m_PatrolPath == null)
		{
			return;
		}
		if (m_Enemy.Target == null && m_Enemy.CurrentNode == null && m_Enemy.CurrentState == State.Character.Flee)
		{
			m_Enemy.SetTarget(m_PatrolPath.Path[m_CurrentPatrolPathPoint]);
			m_Enemy.SetState(State.Character.Patrol);
		}
		else
		{
			if (m_Enemy.Target == null)
			{
				return;
			}
			if (m_Enemy.Target != GameManager.Instance.Player.transform)
			{
				if (!(Vector3.Distance(m_Enemy.transform.position, m_Enemy.Target.position) <= m_Enemy.Agent.Agent.stoppingDistance + 0.1f))
				{
					return;
				}
				this.OnNodeReached.Send(this);
				m_CurrentNode = m_Enemy.Target.GetComponent<CharacterInteractionNode>();
				if (m_CurrentNode != null)
				{
					if (m_CurrentNode.NodeType == CharacterNodeType.Location)
					{
						m_Enemy.SetTarget(null);
						m_Enemy.transform.DOKill();
						m_Enemy.transform.DOMove(m_CurrentNode.transform.position, 0.5f).SetEase(Ease.InOutSine);
						m_Enemy.transform.DORotate(m_CurrentNode.transform.eulerAngles, 0.5f).SetEase(Ease.InOutSine).OnComplete(delegate
						{
							m_CurrentNode = null;
							m_Enemy.CancelPath();
							m_Enemy.ForceStop();
							if (m_SmoothLocationNode)
							{
								float currentMovementState = m_Enemy.Content.Animator.GetFloat("MovementState");
								DOTween.To(() => currentMovementState, delegate(float x)
								{
									currentMovementState = x;
								}, 0f, 0.35f).OnUpdate(delegate
								{
									m_Enemy.Content.Animator.SetMovementState(currentMovementState, smooth: false);
								});
								float currentCombatState = m_Enemy.Content.Animator.GetFloat("CombatState");
								DOTween.To(() => currentCombatState, delegate(float x)
								{
									currentCombatState = x;
								}, 0f, 0.35f).OnUpdate(delegate
								{
									m_Enemy.Content.Animator.SetCombatState(currentCombatState, smooth: false);
								});
							}
							else
							{
								m_Enemy.Content.Animator.SetMovementState(0f, smooth: false);
								m_Enemy.Content.Animator.SetCombatState(0f, smooth: false);
							}
						});
					}
					else if (m_CurrentNode.NodeType == CharacterNodeType.Interaction)
					{
						if (m_Enemy.CurrentState != State.Character.Patrol)
						{
							m_Enemy.SetState(State.Character.Patrol);
						}
						Interaction("Interact", "Interact");
					}
					else if (m_CurrentNode.NodeType == CharacterNodeType.InteractionLoop)
					{
						if (m_Enemy.CurrentState != State.Character.Patrol)
						{
							m_Enemy.SetState(State.Character.Patrol);
						}
						Interaction("SpecialAnimation01", "SpecialAnimationEnter", "SpecialAnimation02");
					}
				}
				else
				{
					if (m_Enemy.CurrentState != State.Character.Patrol)
					{
						m_Enemy.SetState(State.Character.Patrol);
					}
					SetNextPatrolPoint();
					if (!m_PatrolPath.IsSinglePath || m_CurrentPatrolPathPoint < m_PatrolPath.PathLength)
					{
						m_Enemy.SetTarget(m_PatrolPath.Path[m_CurrentPatrolPathPoint]);
					}
				}
				return;
			}
			if (Vector3.Distance(m_Enemy.transform.position, m_Enemy.Target.position) > m_Enemy.CharacterVision.SightRange || GameManager.Instance.Player.CombatStatus == CombatStatus.Hide)
			{
				LoseTarget();
				return;
			}
			if (m_Enemy.Agent.Agent.pathStatus == NavMeshPathStatus.PathPartial || m_Enemy.Agent.Agent.pathStatus == NavMeshPathStatus.PathInvalid)
			{
				if (m_BrokenPathTimer >= m_BrokenPathTimerLimit)
				{
					LoseTarget();
					return;
				}
				m_BrokenPathTimer += Time.deltaTime;
			}
			else
			{
				m_BrokenPathTimer = 0f;
			}
			m_Enemy.OnAnimationComplete -= HandleAnimationComplete;
		}
	}

	public void TriggerSpecialPath()
	{
		if (!m_HasSpecialPath && m_Enemy != null && m_SpecialPath != null)
		{
			if (m_Enemy.CurrentState != State.Character.Patrol)
			{
				m_Enemy.SetState(State.Character.Patrol);
			}
			m_Enemy.SetTarget(m_SpecialPath);
			m_HasSpecialPath = true;
			if (m_S13AudioHandler != null)
			{
				m_S13AudioHandler.transform.position = m_Enemy.transform.position;
				m_S13AudioHandler.Play();
			}
		}
	}

	private void LoseTarget()
	{
		m_HasSpecialPath = false;
		m_Enemy.OnAnimationComplete -= HandleAnimationComplete;
		m_CurrentNode = null;
		m_Enemy.SetTarget(m_PatrolPath.Path[m_CurrentPatrolPathPoint]);
		m_Enemy.SetState(State.Character.Flee);
	}

	private void Interaction(string clipName, string triggerName, string loopClipName = "")
	{
		m_Enemy.SetTarget(null);
		m_Enemy.transform.DOMove(m_CurrentNode.transform.position, 0.75f).SetEase(Ease.InOutSine);
		m_Enemy.transform.DORotate(m_CurrentNode.transform.eulerAngles, 0.75f).SetEase(Ease.InOutSine).OnComplete(delegate
		{
			m_Enemy.CancelPath();
			m_Enemy.ForceStop();
			m_Enemy.Content.Animator.SetCombatState(0f, smooth: false);
			m_CurrentNode.Interactable.Trigger();
			m_CurrentNode.InteractionClip.name = clipName;
			if (loopClipName != "" && m_CurrentNode.InteractionLoopClip != null)
			{
				m_CurrentNode.InteractionLoopClip.name = loopClipName;
			}
			if (m_CurrentNode.InteractionLoopClip != null)
			{
				m_Enemy.Content.UpdateClipOverrides(m_CurrentNode.InteractionClip, m_CurrentNode.InteractionLoopClip);
			}
			else
			{
				m_Enemy.Content.UpdateClipOverrides(m_CurrentNode.InteractionClip);
			}
			m_Enemy.Content.SetAnimationTrigger(triggerName);
			m_CurrentNode.SendOnInteract();
		});
		m_Enemy.OnAnimationComplete -= HandleAnimationComplete;
		m_Enemy.OnAnimationComplete += HandleAnimationComplete;
	}

	private void HandleAnimationComplete(object sender, EventArgs e)
	{
		m_Enemy.OnAnimationComplete -= HandleAnimationComplete;
		if (m_CurrentNode != null)
		{
			if (m_CurrentNode.EndLocation != null)
			{
				m_Enemy.transform.position = m_CurrentNode.EndLocation.position;
				m_Enemy.transform.eulerAngles = m_CurrentNode.EndLocation.eulerAngles;
			}
			m_CurrentNode = null;
		}
		if (m_Enemy.CurrentState != State.Character.Patrol)
		{
			m_Enemy.SetState(State.Character.Patrol);
		}
		if (m_HasSpecialPath)
		{
			m_HasSpecialPath = false;
		}
		SetNextPatrolPoint();
		if (!m_PatrolPath.IsSinglePath || m_CurrentPatrolPathPoint < m_PatrolPath.PathLength)
		{
			m_Enemy.SetTarget(m_PatrolPath.Path[m_CurrentPatrolPathPoint]);
		}
	}

	private void SetNextPatrolPoint()
	{
		if (!m_PatrolPath.IsYoYo)
		{
			m_CurrentPatrolPathPoint++;
			if (!m_PatrolPath.IsSinglePath && m_CurrentPatrolPathPoint >= m_PatrolPath.PathLength)
			{
				m_CurrentPatrolPathPoint = 0;
			}
			else if (m_PatrolPath.IsSinglePath && m_CurrentPatrolPathPoint >= m_PatrolPath.PathLength)
			{
				m_CurrentPatrolPathPoint = m_PatrolPath.PathLength - 1;
			}
		}
		else if (!m_IsYoYoing)
		{
			m_CurrentPatrolPathPoint++;
			if (m_CurrentPatrolPathPoint >= m_PatrolPath.PathLength)
			{
				m_CurrentPatrolPathPoint = m_PatrolPath.PathLength - 2;
				m_IsYoYoing = true;
			}
		}
		else
		{
			m_CurrentPatrolPathPoint--;
			if (m_CurrentPatrolPathPoint <= 0)
			{
				m_CurrentPatrolPathPoint = 0;
				m_IsYoYoing = false;
			}
		}
	}

	protected override void OnDisposed()
	{
		if (m_Enemy != null)
		{
			m_Enemy.OnAnimationComplete -= HandleAnimationComplete;
			m_Enemy = null;
		}
		m_CurrentNode = null;
		this.OnNodeReached = null;
		base.OnDisposed();
	}
}
