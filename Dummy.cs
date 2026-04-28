using System;
using UnityEngine;
using UnityEngine.AI;

public class Dummy : JMonoBehaviour, IHittable
{
	[Header("Content")]
	[SerializeField]
	private CharacterContent m_Content;

	[Header("Nav Mesh Agent")]
	[SerializeField]
	private NavMeshAgent m_Agent;

	[SerializeField]
	private Collider m_Collider;

	[Header("Movement")]
	[SerializeField]
	private float m_StopDistance;

	[Header("Other")]
	[SerializeField]
	private bool m_IsImmune;

	[SerializeField]
	private bool m_IsCompanion;

	protected Transform m_Target;

	protected Vector3 m_CurrentTarget;

	private float m_SampleDistance = 25f;

	private bool m_IsDead;

	private bool m_IsAttacking;

	private float m_AttackType;

	private float m_AttackCooldown;

	private float m_AttackCooldownLimit = 0.75f;

	public CharacterContent Content => m_Content;

	public NavMeshAgent Agent => m_Agent;

	public Transform Target => m_Target;

	public Vector3 CurrentTarget => m_CurrentTarget;

	public bool IsBroken { get; private set; }

	public bool IsPlayerBreakable { get; private set; }

	public event EventHandler OnDeath;

	public override void Start()
	{
		m_Content.GenericAnimationEvents.SetReciever(this);
		if (!m_IsCompanion)
		{
			if (GameManager.Instance.BeastBendy != null)
			{
				SetTarget(GameManager.Instance.BeastBendy.transform);
			}
		}
		else if (GameManager.Instance.DummyManager != null)
		{
			Dummy closest = GameManager.Instance.DummyManager.GetClosest(base.transform);
			if (closest != null)
			{
				SetTarget(closest.transform);
			}
		}
		ResetAgent();
	}

	private void FixedUpdate()
	{
		if (m_IsDead || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (m_Target == null && GameManager.Instance.DummyManager != null)
		{
			Dummy closest = GameManager.Instance.DummyManager.GetClosest(base.transform);
			if (closest != null)
			{
				SetTarget(closest.transform);
				closest.SetTarget(base.transform);
				Dummy closest2 = GameManager.Instance.DummyManager.GetClosest(closest.transform);
				if (closest2 != null)
				{
					closest2.SetTarget(base.transform);
					closest2 = GameManager.Instance.DummyManager.GetClosest(GameManager.Instance.BeastBendy.transform);
					if (closest2 != null)
					{
						closest2.SetTarget(base.transform);
					}
				}
			}
		}
		if (m_Target == null)
		{
			Stop();
			return;
		}
		float num = Vector3.Distance(base.transform.position, m_Target.position);
		if (!m_IsCompanion)
		{
			m_Agent.avoidancePriority = ((num > 99f) ? 99 : ((int)num));
		}
		if (num >= m_StopDistance)
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position + Vector3.up * 4f + base.transform.forward * 2f, 0.5f);
			bool flag = false;
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				if (!(collider.transform == base.transform) && collider.gameObject.layer == LayerMask.NameToLayer("AI"))
				{
					flag = true;
					break;
				}
			}
			if (!m_IsCompanion && flag)
			{
				if (num < 20f)
				{
					Stop();
					Attack();
				}
				else if (MoveTo(m_Target.position))
				{
					Move();
				}
				else
				{
					Stop();
				}
			}
			else if (MoveTo(m_Target.position))
			{
				Move();
			}
			else
			{
				Stop();
			}
		}
		else
		{
			Stop();
			Attack();
		}
	}

	public void SetTarget(Transform target)
	{
		m_Target = target;
	}

	public void SetStopDistance(float distance)
	{
		m_StopDistance = distance;
	}

	private void Move()
	{
		m_Content.Animator.SetMovementState(2f);
		m_Content.Animator.SetMovementSpeed(2f);
	}

	private void Stop()
	{
		m_Content.Animator.SetMovementState(0f);
		m_Content.Animator.SetMovementSpeed(0f);
		CancelPath();
	}

	private void Attack()
	{
		if (!m_IsAttacking)
		{
			LookAtTarget();
		}
		m_AttackCooldown += Time.deltaTime;
		if (m_AttackCooldown >= m_AttackCooldownLimit && !m_IsAttacking)
		{
			m_IsAttacking = true;
			m_Content.Animator.Attack(m_AttackType);
			m_AttackType += 1f;
			if (m_AttackType >= 3f)
			{
				m_AttackType = 0f;
			}
		}
	}

	public void OnAttack()
	{
		Vector3 vector = base.transform.position + Vector3.up * (m_IsCompanion ? 2f : 4f);
		float num = (m_IsCompanion ? 20 : 10);
		float radius = 0.2f;
		Vector3 position = vector + base.transform.forward * (num / 2f);
		string layerName = (m_IsCompanion ? "AI" : "Player");
		string layerName2 = (m_IsCompanion ? "Player" : "AI");
		QueryTriggerInteraction queryTriggerInteraction = ((!m_IsCompanion) ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide);
		Collider[] array = new Collider[1];
		if (Physics.OverlapSphereNonAlloc(position, num / 2f, array, 1 << LayerMask.NameToLayer(layerName), queryTriggerInteraction) <= 0)
		{
			return;
		}
		Vector3 position2 = array[0].transform.position;
		position2.y = vector.y;
		if (!Physics.SphereCast(vector, radius, (position2 - vector).normalized, out var hitInfo, num, ~(1 << LayerMask.NameToLayer(layerName2)), queryTriggerInteraction))
		{
			return;
		}
		if (m_IsCompanion)
		{
			Dummy component = hitInfo.transform.GetComponent<Dummy>();
			if (component != null)
			{
				component.Hit(hitInfo);
			}
		}
		else
		{
			CameraEffects.ShakeRotation(0.25f, 0.75f, 10, 90f, fadeOut: false);
		}
	}

	public void SetPreviousState()
	{
		m_Content.Animator.SetMovementState(0f);
		m_Content.Animator.SetMovementSpeed(0f);
		m_AttackCooldown = 0f;
		m_IsAttacking = false;
	}

	private void LookAtTarget()
	{
		Quaternion b = Quaternion.LookRotation((m_Target.position - base.transform.position).normalized);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * 5f);
	}

	public void ForceKil()
	{
		if (!m_IsImmune)
		{
			m_IsDead = true;
			m_Agent.enabled = false;
			m_Collider.enabled = false;
			m_Content.gameObject.SetActive(value: false);
			this.OnDeath.Send(this);
		}
	}

	public void Hit(RaycastHit hit)
	{
		if (!m_IsImmune)
		{
			Transform obj = GameManager.Instance.PoolingManager.GetFromPool("Effects/Effects_Dummy_Death").transform;
			obj.position = base.transform.position;
			obj.SetParent(null);
			m_IsDead = true;
			this.OnDeath.Send(this);
			m_Agent.enabled = false;
			m_Collider.enabled = false;
			m_Content.gameObject.SetActive(value: false);
		}
	}

	public bool MoveTo(Vector3 desiredTarget)
	{
		if (QueryNavmesh(desiredTarget, ref m_CurrentTarget) && m_Agent.pathStatus != NavMeshPathStatus.PathInvalid && !m_Agent.isOnOffMeshLink)
		{
			return GetPath(m_CurrentTarget);
		}
		return false;
	}

	public bool IsTargetReached()
	{
		if ((m_Agent.isOnNavMesh || m_Agent.isOnOffMeshLink) && !m_Agent.pathPending)
		{
			return m_Agent.remainingDistance <= m_Agent.stoppingDistance;
		}
		return false;
	}

	public bool QueryNavmesh(Vector3 desiredTarget, ref Vector3 actualTarget)
	{
		if (NavMesh.SamplePosition(desiredTarget, out var hit, m_SampleDistance, -1))
		{
			actualTarget = hit.position;
			return true;
		}
		return false;
	}

	public bool GetPath(Vector3 destination)
	{
		bool result = false;
		NavMeshPath navMeshPath = new NavMeshPath();
		if (NavMesh.CalculatePath(base.transform.position, destination, -1, navMeshPath))
		{
			m_Agent.SetPath(navMeshPath);
			for (int i = 0; i < navMeshPath.corners.Length; i++)
			{
				if (i < navMeshPath.corners.Length - 1)
				{
					Debug.DrawLine(navMeshPath.corners[i], navMeshPath.corners[i + 1]);
				}
			}
			result = true;
		}
		return result;
	}

	public void CancelPath()
	{
		if (m_Agent.isOnNavMesh || m_Agent.isOnOffMeshLink)
		{
			m_Agent.ResetPath();
			m_Agent.CompleteOffMeshLink();
		}
	}

	public void ResetAgent()
	{
		if (m_Agent != null)
		{
			m_Agent.enabled = false;
			m_Agent.enabled = true;
		}
	}

	public void SendOnDeath()
	{
		this.OnDeath.Send(this);
	}
}
