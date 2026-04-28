using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionPeekController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private PeekGroup[] m_Group;

	public SectionID SectionID => m_SectionID;

	public PeekGroup[] Group => m_Group;

	protected override IEnumerator InternalInitialize()
	{
		int num = -1;
		for (int i = 0; i < m_Group.Length; i++)
		{
			PeekGroup peekGroup = m_Group[i];
			PeekDataObject peekDataObject = (PeekDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, PeekDataObject>(m_SectionID, peekGroup.ID);
			if (peekDataObject == null)
			{
				peekDataObject = DataObject<int, PeekDataObject>.Create(peekGroup.ID);
				GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, peekDataObject);
			}
			else if (peekDataObject.PeekState == 1)
			{
				num = peekDataObject.ID;
			}
			peekGroup.Controller.OnEnter -= HandlePeekOnEnter;
			peekGroup.Controller.OnEnter += HandlePeekOnEnter;
			peekGroup.Controller.OnExit -= HandlePeekOnExit;
			peekGroup.Controller.OnExit += HandlePeekOnExit;
			peekGroup.Controller.Initialize();
			if (peekGroup.ID == num)
			{
				peekGroup.Controller.ForceEnter(peekDataObject);
			}
		}
		yield return null;
	}

	private void HandlePeekOnEnter(object sender, EventArgs e)
	{
		Peek peek = sender as Peek;
		PeekDataObject peekDataObject = (PeekDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, PeekDataObject>(m_SectionID, peek.ID);
		if (peekDataObject == null)
		{
			peekDataObject = DataObject<int, PeekDataObject>.Create(peek.ID);
			GameManager.Instance.GameData.CurrentSave.AddData(m_SectionID, peekDataObject);
		}
		peekDataObject.SetIndex(peek.CurrentIndex);
		peekDataObject.SetPeekState(isPeeking: true);
	}

	private void HandlePeekOnExit(object sender, EventArgs e)
	{
		Peek peek = sender as Peek;
		((PeekDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, PeekDataObject>(m_SectionID, peek.ID))?.SetPeekState(isPeeking: false);
	}

	protected override void RemoveListeners()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			PeekGroup obj = m_Group[i];
			obj.Controller.OnEnter -= HandlePeekOnEnter;
			obj.Controller.OnExit -= HandlePeekOnExit;
		}
	}

	public override void SaveData()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			PeekGroup peekGroup = m_Group[i];
			PeekDataObject peekDataObject = (PeekDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, PeekDataObject>(m_SectionID, peekGroup.ID);
			if (peekDataObject != null && peekDataObject.PeekState == 1)
			{
				peekDataObject.SetRotationX(GameManager.Instance.Player.AnimationContainer.localRotation);
				peekDataObject.SetRotationY(GameManager.Instance.Player.HeadContainer.localRotation);
			}
		}
	}
}
