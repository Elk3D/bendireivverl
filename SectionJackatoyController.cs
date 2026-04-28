using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionJackatoyController : SectionController
{
	private Jackatoy[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		m_Group = base.transform.GetComponentsInChildren<Jackatoy>(includeInactive: true);
		for (int i = 0; i < m_Group.Length; i++)
		{
			Jackatoy jackatoy = m_Group[i];
			if (!jackatoy.InitializeOnAwake)
			{
				jackatoy.Initialize();
			}
			yield return null;
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		m_Group = null;
	}
}
