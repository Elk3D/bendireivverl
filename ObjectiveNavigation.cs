using UnityEngine;

public class ObjectiveNavigation : Objective
{
	[SerializeField]
	protected bool m_IsReal;

	[SerializeField]
	private Transform m_Destination;

	[SerializeField]
	private bool m_IsRanged;

	[SerializeField]
	private float m_Range = 10f;

	private bool m_IsInRange;

	private bool m_IsActiveRange;

	protected override void InternalEnable()
	{
		Initialize();
	}

	protected override void InternalInitialize()
	{
		ShowNavigation();
	}

	protected override void InternalDisable()
	{
		Inactive();
	}

	protected override void InternalInactive()
	{
		ClearNavigation();
	}

	private void ShowNavigation()
	{
		if (m_Destination != null)
		{
			if (m_IsRanged)
			{
				m_IsActiveRange = true;
			}
			else
			{
				GameManager.Instance.ShowNavigation(m_Destination, m_IsReal);
			}
		}
	}

	private void ClearNavigation()
	{
		GameManager.Instance.ClearNavigation();
		m_IsInRange = false;
		m_IsActiveRange = false;
		SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		SendOnComplete();
	}

	protected override void InternalUpdateComplete()
	{
		if (m_IsActiveRange && m_IsRanged && m_Destination != null && GameManager.Instance.Player != null)
		{
			float num = Vector3.Distance(GameManager.Instance.Player.transform.position, m_Destination.position);
			if (!m_IsInRange && num <= m_Range)
			{
				GameManager.Instance.ShowNavigation(m_Destination, m_IsReal);
				m_IsInRange = true;
			}
			else if (m_IsInRange && num > m_Range)
			{
				GameManager.Instance.ClearNavigation();
				m_IsInRange = false;
			}
		}
	}
}
