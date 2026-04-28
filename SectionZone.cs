using UnityEngine;

public class SectionZone : EventTrigger
{
	[Header("SectionZone Options")]
	[SerializeField]
	private string m_ZoneName;

	public string Name => m_ZoneName;

	protected override void InternalInitialize()
	{
		SetActive(active: true);
	}

	public Transform GetClosestTarget(CompanionNode[] targets, Transform primaryTarget, Transform previousTarget = null)
	{
		Transform result = null;
		float num = float.PositiveInfinity;
		Vector3 position = primaryTarget.position;
		foreach (CompanionNode companionNode in targets)
		{
			if (!(previousTarget != null) || !(companionNode.transform == previousTarget))
			{
				float num2 = Vector3.Distance(companionNode.transform.position, position);
				if (num2 < num)
				{
					result = companionNode.transform;
					num = num2;
				}
			}
		}
		return result;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
