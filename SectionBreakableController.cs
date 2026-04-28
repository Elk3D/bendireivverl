using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionBreakableController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private BreakableGroup[] m_Group;

	public SectionID SectionID => m_SectionID;

	public BreakableGroup[] Group => m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			BreakableGroup breakableGroup = m_Group[i];
			if (breakableGroup != null)
			{
				breakableGroup.Controller.OnBroken -= HandleBreakableOnBroken;
				if ((BreakableDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, BreakableDataObject>(m_SectionID, breakableGroup.ID) != null)
				{
					breakableGroup.IsComplete = true;
					breakableGroup.Controller.ForceComplete();
				}
				else
				{
					breakableGroup.Controller.OnBroken += HandleBreakableOnBroken;
				}
			}
			yield return null;
		}
	}

	private void HandleBreakableOnBroken(object sender, EventArgs e)
	{
		Breakable breakable = sender as Breakable;
		breakable.OnBroken -= HandleBreakableOnBroken;
		for (int i = 0; i < m_Group.Length; i++)
		{
			BreakableGroup breakableGroup = m_Group[i];
			if (breakableGroup.Controller == breakable)
			{
				BreakableDataObject breakableDataObject = (BreakableDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, BreakableDataObject>(m_SectionID, breakableGroup.ID);
				if (breakableDataObject == null)
				{
					breakableDataObject = DataObject<int, BreakableDataObject>.Create(breakableGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, breakableDataObject);
				}
				breakableGroup.IsComplete = true;
				break;
			}
		}
	}

	protected override void OnDisposed()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			m_Group[i].Controller.OnBroken -= HandleBreakableOnBroken;
		}
		base.OnDisposed();
	}
}
