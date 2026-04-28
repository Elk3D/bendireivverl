using UnityEngine;
using UnityEngine.AI;

public class CharacterAgent : JDisposable
{
	protected Vector3 m_CurrentTarget;

	private float m_SampleDistance = 25f;

	private bool isLinking;

	public Character Character { get; private set; }

	public NavMeshAgent Agent { get; private set; }

	public Vector3 CurrentTarget => m_CurrentTarget;

	public CharacterAgent(Character character)
	{
		Character = character;
		Agent = Character.GetComponent<NavMeshAgent>();
	}

	public bool MoveTo(Vector3 desiredTarget)
	{
		if (QueryNavmesh(desiredTarget, ref m_CurrentTarget) && Agent.pathStatus != NavMeshPathStatus.PathInvalid && !Agent.isOnOffMeshLink)
		{
			isLinking = false;
			NavMeshPath navMeshPath;
			return GetPath(m_CurrentTarget, out navMeshPath);
		}
		if (!isLinking && Agent.isOnOffMeshLink)
		{
			Agent.speed = 0.001f;
			isLinking = true;
		}
		return false;
	}

	public bool IsTargetReached()
	{
		if ((Agent.isOnNavMesh || Agent.isOnOffMeshLink) && !Agent.pathPending)
		{
			return Agent.remainingDistance <= Agent.stoppingDistance;
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

	public bool GetPath(Vector3 destination, out NavMeshPath navMeshPath)
	{
		bool result = false;
		navMeshPath = new NavMeshPath();
		if (NavMesh.CalculatePath(Character.transform.position, destination, -1, navMeshPath))
		{
			Agent.SetPath(navMeshPath);
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
		if (Agent.isOnNavMesh || Agent.isOnOffMeshLink)
		{
			Agent.ResetPath();
			Agent.CompleteOffMeshLink();
		}
	}

	public void ResetAgent()
	{
		if (Agent != null)
		{
			Agent.enabled = false;
			Agent.enabled = true;
		}
	}

	public void SetSampleDistance(float value)
	{
		m_SampleDistance = value;
	}

	protected override void OnDisposed()
	{
		Agent = null;
		Character = null;
		base.OnDisposed();
	}
}
