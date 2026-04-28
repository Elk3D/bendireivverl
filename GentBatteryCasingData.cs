using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Gent Batteries/New Gent Battery Casing")]
public class GentBatteryCasingData : ScriptableObject
{
	[SerializeField]
	private Interactable m_Interactable;

	[SerializeField]
	private int m_Value = 1;

	public Interactable Interactable => m_Interactable;

	public int Value => m_Value;
}
