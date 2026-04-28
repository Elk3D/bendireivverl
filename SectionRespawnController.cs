using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionRespawnController : SectionController
{
	private PlayerRespawn[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		m_Group = base.transform.GetComponentsInChildren<PlayerRespawn>(includeInactive: true);
		for (int i = 0; i < m_Group.Length; i++)
		{
			m_Group[i].Initialize();
		}
		yield return null;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		m_Group = null;
	}
}
