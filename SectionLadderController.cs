using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionLadderController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private LadderGroup[] m_Group;

	public SectionID SectionID => m_SectionID;

	public LadderGroup[] Group => m_Group;

	protected override IEnumerator InternalInitialize()
	{
		int activeID = -1;
		for (int i = 0; i < m_Group.Length; i++)
		{
			LadderGroup group = m_Group[i];
			LadderDataObject dataObject = (LadderDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, LadderDataObject>(m_SectionID, group.ID);
			if (dataObject == null)
			{
				dataObject = DataObject<int, LadderDataObject>.Create(group.ID);
				GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, dataObject);
			}
			else if (dataObject.ClimbState == 1)
			{
				activeID = dataObject.ID;
			}
			group.Controller.OnLadderBottomEnter -= HandleLadderBottomOnEnter;
			group.Controller.OnLadderBottomEnter += HandleLadderBottomOnEnter;
			group.Controller.OnLadderBottomExit -= HandleLadderBottomOnExit;
			group.Controller.OnLadderBottomExit += HandleLadderBottomOnExit;
			group.Controller.OnLadderTopEnter -= HandleLadderTopOnEnter;
			group.Controller.OnLadderTopEnter += HandleLadderTopOnEnter;
			group.Controller.OnLadderTopExit -= HandleLadderTopOnExit;
			group.Controller.OnLadderTopExit += HandleLadderTopOnExit;
			group.Controller.OnDeath -= HandleLadderOnDeath;
			group.Controller.OnDeath += HandleLadderOnDeath;
			group.Controller.Initialize();
			yield return null;
			if (group.ID != activeID)
			{
				continue;
			}
			if (dataObject.ClimbDirection == 1)
			{
				if (dataObject.LadderIndex >= group.Controller.LadderIndex)
				{
					dataObject.SetClimbDirection(2);
					group.Controller.ForceEnterTop(dataObject);
				}
				else
				{
					group.Controller.ForceEnterBottom(dataObject);
				}
			}
			else if (dataObject.ClimbDirection == 2)
			{
				if (dataObject.LadderIndex <= 0)
				{
					dataObject.SetClimbDirection(1);
					group.Controller.ForceEnterBottom(dataObject);
				}
				else
				{
					group.Controller.ForceEnterTop(dataObject);
				}
			}
		}
		yield return null;
	}

	private void HandleLadderBottomOnEnter(object sender, EventArgs e)
	{
		Ladder ladder = sender as Ladder;
		LadderDataObject ladderDataObject = (LadderDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, LadderDataObject>(m_SectionID, ladder.ID);
		if (ladderDataObject == null)
		{
			ladderDataObject = DataObject<int, LadderDataObject>.Create(ladder.ID);
			GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, ladderDataObject);
		}
		ladderDataObject.SetClimbState(isClimbing: true);
		ladderDataObject.SetClimbDirection(1);
		ladderDataObject.SetLadderSide(0);
	}

	private void HandleLadderBottomOnExit(object sender, EventArgs e)
	{
		Ladder ladder = sender as Ladder;
		ExitLadder(ladder);
	}

	private void HandleLadderTopOnEnter(object sender, EventArgs e)
	{
		Ladder ladder = sender as Ladder;
		LadderDataObject ladderDataObject = (LadderDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, LadderDataObject>(m_SectionID, ladder.ID);
		if (ladderDataObject == null)
		{
			ladderDataObject = DataObject<int, LadderDataObject>.Create(ladder.ID);
			GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, ladderDataObject);
		}
		ladderDataObject.SetClimbState(isClimbing: true);
		ladderDataObject.SetClimbDirection(2);
		ladderDataObject.SetLadderSide(0);
	}

	private void HandleLadderTopOnExit(object sender, EventArgs e)
	{
		Ladder ladder = sender as Ladder;
		ExitLadder(ladder);
	}

	private void HandleLadderOnDeath(object sender, EventArgs e)
	{
		Ladder ladder = sender as Ladder;
		ExitLadder(ladder);
	}

	private void ExitLadder(Ladder ladder)
	{
		LadderDataObject ladderDataObject = (LadderDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, LadderDataObject>(m_SectionID, ladder.ID);
		if (ladderDataObject != null)
		{
			ladderDataObject.SetClimbState(isClimbing: false);
			ladderDataObject.SetClimbDirection(0);
		}
	}

	protected override void RemoveListeners()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			LadderGroup obj = m_Group[i];
			obj.Controller.OnLadderBottomEnter -= HandleLadderBottomOnEnter;
			obj.Controller.OnLadderBottomExit -= HandleLadderBottomOnExit;
			obj.Controller.OnLadderTopEnter -= HandleLadderTopOnEnter;
			obj.Controller.OnLadderTopExit -= HandleLadderTopOnExit;
			obj.Controller.OnDeath -= HandleLadderOnDeath;
		}
	}

	public override void SaveData()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			LadderGroup ladderGroup = m_Group[i];
			LadderDataObject ladderDataObject = (LadderDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, LadderDataObject>(m_SectionID, ladderGroup.ID);
			if (ladderDataObject != null && ladderDataObject.ClimbState == 1)
			{
				ladderDataObject.SetLadderIndex(ladderGroup.Controller.CurrentLadderIndex);
				ladderDataObject.SetLadderSide(ladderGroup.Controller.CurrentLadderSide);
				ladderDataObject.SetClimbDirection(ladderGroup.Controller.ClimbDirection);
				ladderDataObject.SetRotationX(GameManager.Instance.Player.AnimationContainer.localRotation);
				ladderDataObject.SetRotationY(GameManager.Instance.Player.HeadContainer.localRotation);
			}
		}
	}
}
