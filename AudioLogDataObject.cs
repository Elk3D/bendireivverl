using System;
using UnityEngine;

[Serializable]
public class AudioLogDataObject : DataObject<AudioLogID, AudioLogDataObject>, IDataObject<AudioLogID>, IDataObject
{
	[SerializeField]
	private AudioLogID m_AudioLogID;

	public override AudioLogID ID => m_AudioLogID;

	protected override void Deserialize()
	{
		m_AudioLogID = m_ID;
	}
}
