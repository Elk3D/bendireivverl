using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionMemoController : SectionController
{
	private Memo[] m_Memos;

	protected override IEnumerator InternalInitialize()
	{
		m_Memos = base.transform.GetComponentsInChildren<Memo>(includeInactive: true);
		for (int i = 0; i < m_Memos.Length; i++)
		{
			Memo memo = m_Memos[i];
			if (!memo.InitializeOnAwake)
			{
				memo.Initialize();
			}
			yield return null;
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		m_Memos = null;
	}
}
