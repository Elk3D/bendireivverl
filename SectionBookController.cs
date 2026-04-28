using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionBookController : SectionController
{
	private Book[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		m_Group = base.transform.GetComponentsInChildren<Book>(includeInactive: true);
		for (int i = 0; i < m_Group.Length; i++)
		{
			Book book = m_Group[i];
			if (!book.InitializeOnAwake)
			{
				book.Initialize();
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
