using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionIllusionController : SectionController
{
	private Illusion[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		m_Group = base.transform.GetComponentsInChildren<Illusion>(includeInactive: true);
		for (int i = 0; i < m_Group.Length; i++)
		{
			Illusion illusion = m_Group[i];
			if (!illusion.InitializeOnAwake)
			{
				illusion.Initialize();
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
