using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionCollectableController : SectionController
{
	private Collectable[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		m_Group = base.transform.GetComponentsInChildren<Collectable>(includeInactive: true);
		for (int i = 0; i < m_Group.Length; i++)
		{
			Collectable collectable = m_Group[i];
			if (!collectable.InitializeOnAwake)
			{
				collectable.Initialize();
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
