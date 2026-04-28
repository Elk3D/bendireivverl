using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InsaneButton : JMonoBehaviour
{
	[SerializeField]
	private Transform m_FootLocation;

	[SerializeField]
	private GameObject m_Active;

	[SerializeField]
	private GameObject m_Close;

	[SerializeField]
	private GameObject m_Inactive;

	private List<InsaneFoot> m_Feet = new List<InsaneFoot>();

	private bool m_IsActivated;

	public override void Awake()
	{
		m_Active.SetActive(value: true);
		m_Inactive.SetActive(value: false);
		m_Close.SetActive(value: false);
	}

	private void Update()
	{
		if (m_IsActivated || m_Feet.Count <= 0 || !PlayerInput.InteractOnPressed())
		{
			return;
		}
		InsaneFoot closestFoot = GetClosestFoot(m_Feet, m_FootLocation);
		if (closestFoot != null)
		{
			m_IsActivated = true;
			closestFoot.Interact(m_FootLocation.position);
			closestFoot.transform.parent.transform.DOMove(m_FootLocation.position, 0.2f).SetEase(Ease.Linear).OnComplete(delegate
			{
				m_Active.SetActive(value: false);
				m_Close.SetActive(value: false);
				m_Inactive.SetActive(value: true);
			});
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (m_IsActivated)
		{
			return;
		}
		InsaneFoot component = other.GetComponent<InsaneFoot>();
		if (!(component == null))
		{
			if (!m_Feet.Contains(component))
			{
				m_Feet.Add(component);
			}
			m_Active.SetActive(value: false);
			m_Close.SetActive(value: true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (m_IsActivated)
		{
			return;
		}
		InsaneFoot component = other.GetComponent<InsaneFoot>();
		if (!(component == null))
		{
			if (m_Feet.Contains(component))
			{
				m_Feet.Remove(component);
			}
			if (m_Feet.Count <= 0)
			{
				m_Active.SetActive(value: true);
				m_Close.SetActive(value: false);
			}
		}
	}

	private InsaneFoot GetClosestFoot(List<InsaneFoot> _targets, Transform fromThis)
	{
		InsaneFoot result = null;
		float num = float.PositiveInfinity;
		Vector3 position = fromThis.position;
		foreach (InsaneFoot _target in _targets)
		{
			float sqrMagnitude = (_target.transform.position - position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = _target;
			}
		}
		return result;
	}
}
