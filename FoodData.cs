using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Food/New Food")]
public class FoodData : ScriptableObject
{
	[SerializeField]
	private Interactable m_Interactable;

	[SerializeField]
	private string m_FoodName = "";

	[SerializeField]
	private int m_Value;

	public Interactable Interactable => m_Interactable;

	public string FoodName => m_FoodName;

	public int Value => m_Value;
}
