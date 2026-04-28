using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionFoodController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private FoodGroup[] m_Group;

	public SectionID SectionID => m_SectionID;

	public FoodGroup[] Group => m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			FoodGroup foodGroup = m_Group[i];
			if (foodGroup != null)
			{
				foodGroup.Controller.OnActivate -= HandleControllerOnActivate;
				if ((FoodDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, FoodDataObject>(m_SectionID, foodGroup.ID) != null)
				{
					foodGroup.IsComplete = true;
					foodGroup.Controller.Content.ForceActivateComplete();
				}
				else
				{
					(foodGroup.Controller.Content as FoodContent).InitializeContent();
					foodGroup.Controller.OnActivate += HandleControllerOnActivate;
				}
			}
			yield return null;
		}
	}

	private void HandleControllerOnActivate(object sender, EventArgs e)
	{
		Food food = sender as Food;
		food.OnActivate -= HandleControllerOnActivate;
		for (int i = 0; i < m_Group.Length; i++)
		{
			FoodGroup foodGroup = m_Group[i];
			if (foodGroup.Controller == food)
			{
				FoodDataObject foodDataObject = (FoodDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, FoodDataObject>(m_SectionID, foodGroup.ID);
				if (foodDataObject == null)
				{
					foodDataObject = DataObject<int, FoodDataObject>.Create(foodGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, foodDataObject);
				}
				foodGroup.IsComplete = true;
				break;
			}
		}
	}

	protected override void OnDisposed()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			m_Group[i].Controller.OnActivate -= HandleControllerOnActivate;
		}
		base.OnDisposed();
	}
}
