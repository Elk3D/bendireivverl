using System;
using UnityEngine;

public class RagdollController : JMonoBehaviour
{
	[SerializeField]
	private GameObject m_Model;

	private Ragdoll[] m_Ragdolls;

	private bool m_IsActive;

	public override void Awake()
	{
		m_Ragdolls = m_Model.GetComponentsInChildren<Ragdoll>(includeInactive: true);
		for (int i = 0; i < m_Ragdolls.Length; i++)
		{
			m_Ragdolls[i].OnHit -= HandleRagdollOnHit;
			m_Ragdolls[i].OnHit += HandleRagdollOnHit;
		}
	}

	private void HandleRagdollOnHit(object sender, EventArgs e)
	{
		for (int i = 0; i < m_Ragdolls.Length; i++)
		{
			Ragdoll obj = m_Ragdolls[i];
			obj.Rigidbody.WakeUp();
			obj.Enable();
		}
	}

	private void FixedUpdate()
	{
		if (!m_IsActive || m_Model == null || m_Ragdolls == null || m_Ragdolls.Length == 0 || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < m_Ragdolls.Length; i++)
		{
			Ragdoll obj = m_Ragdolls[i];
			obj.Rigidbody.sleepThreshold = 1f;
			if (obj.Rigidbody.IsSleeping())
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			Sleep();
		}
	}

	public void Sleep()
	{
		for (int i = 0; i < m_Ragdolls.Length; i++)
		{
			m_Ragdolls[i].Disable();
		}
	}

	public void Activate(RaycastHit hit, bool isColor = false)
	{
		m_IsActive = true;
		for (int i = 0; i < m_Ragdolls.Length; i++)
		{
			m_Ragdolls[i].Activate(isColor);
		}
		for (int j = 0; j < m_Ragdolls.Length; j++)
		{
			m_Ragdolls[j].AddForce(50f, hit.point, 20f, 1f);
		}
	}

	public void ForceActivate()
	{
		m_IsActive = true;
		for (int i = 0; i < m_Ragdolls.Length; i++)
		{
			m_Ragdolls[i].Activate();
		}
	}
}
