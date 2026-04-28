using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Jackatoy/New Jackatoy")]
public class JackatoyData : ScriptableObject
{
	[Header("Section Identifier")]
	[SerializeField]
	private SectionID m_SectionID;

	[Header("Jackatoy Identifier")]
	[SerializeField]
	private JackatoyID m_ID;

	public SectionID SectionID => m_SectionID;

	public JackatoyID ID => m_ID;
}
