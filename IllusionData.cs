using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Illusion of Living/New Illusion Book")]
public class IllusionData : ScriptableObject
{
	[Header("Section Identifier")]
	[SerializeField]
	private SectionID m_SectionID;

	[Header("Illusion Identifier")]
	[SerializeField]
	private IllusionID m_ID;

	public SectionID SectionID => m_SectionID;

	public IllusionID ID => m_ID;
}
