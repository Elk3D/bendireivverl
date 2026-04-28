using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionAudioLogController : SectionController
{
	private AudioLog[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		m_Group = base.transform.GetComponentsInChildren<AudioLog>(includeInactive: true);
		for (int i = 0; i < m_Group.Length; i++)
		{
			AudioLog audioLog = m_Group[i];
			if (!audioLog.InitializeOnAwake)
			{
				audioLog.Initialize();
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
