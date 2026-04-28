using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionMovableController : SectionController
{
	private Movable[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		m_Group = base.transform.GetComponentsInChildren<Movable>(includeInactive: true);
		for (int i = 0; i < m_Group.Length; i++)
		{
			Movable group = m_Group[i];
			RemoveListeners(ref group);
			MovableDataObject dataObject = (MovableDataObject)GameManager.Instance.GameData.CurrentSave.GetData<MovableID, MovableDataObject>(group.SectionID, group.ID);
			if (dataObject != null)
			{
				group.Data.SetActive(dataObject.IsActive);
				group.Data.SetCurrentIndex(dataObject.CurrentIndex);
				group.Data.SetMoveState(dataObject.MoveState == 1);
				group.Data.SetSide(dataObject.Side);
			}
			else
			{
				group.Data.SetCurrentIndex(group.Path.GetStartLocation());
				dataObject = DataObject<MovableID, MovableDataObject>.Create(group.ID);
				dataObject.SetActive(group.Data.IsActive);
				dataObject.SetCurrentIndex(group.Data.CurrentIndex);
				dataObject.SetMoveState(canMove: false);
				dataObject.SetSide(0);
				GameManager.Instance.GameData.CurrentSave.AddData(group.SectionID, dataObject);
			}
			AddListeners(ref group);
			group.Initialize();
			yield return null;
			if (dataObject.MoveState == 1)
			{
				if (dataObject.Side == 0)
				{
					group.ForceEnterFront(dataObject);
				}
				else if (dataObject.Side == 1)
				{
					group.ForceEnterBack(dataObject);
				}
			}
			else
			{
				group.SetStartLocation();
			}
		}
		yield return null;
	}

	private void HandleGroupOnEnabled(object sender, EventArgs e)
	{
		Movable movable = sender as Movable;
		movable.Data.SetActive(active: true);
		UpdateGroup(movable);
	}

	private void HandleGroupOnDisabled(object sender, EventArgs e)
	{
		Movable movable = sender as Movable;
		movable.Data.SetActive(active: false);
		UpdateGroup(movable);
	}

	private void HandleGroupOnFrontEnter(object sender, EventArgs e)
	{
		Movable movable = sender as Movable;
		UpdateGroup(movable);
	}

	private void HandleGroupOnBackEnter(object sender, EventArgs e)
	{
		Movable movable = sender as Movable;
		UpdateGroup(movable);
	}

	private void HandleGroupOnExit(object sender, EventArgs e)
	{
		Movable movable = sender as Movable;
		UpdateGroup(movable);
	}

	private void UpdateGroup(Movable group)
	{
		MovableDataObject movableDataObject = (MovableDataObject)GameManager.Instance.GameData.CurrentSave.GetData<MovableID, MovableDataObject>(group.SectionID, group.ID);
		if (movableDataObject != null)
		{
			movableDataObject.SetActive(group.Data.IsActive);
			movableDataObject.SetCurrentIndex(group.Data.CurrentIndex);
			movableDataObject.SetMoveState(group.Data.MoveState == 1);
			movableDataObject.SetSide(group.Data.Side);
			if (group.Data.MoveState == 1)
			{
				movableDataObject.SetRotationX(GameManager.Instance.Player.AnimationContainer.localRotation);
				movableDataObject.SetRotationY(GameManager.Instance.Player.HeadContainer.localRotation);
			}
			return;
		}
		movableDataObject = DataObject<MovableID, MovableDataObject>.Create(group.ID);
		movableDataObject.SetActive(group.Data.IsActive);
		movableDataObject.SetCurrentIndex(group.Data.CurrentIndex);
		movableDataObject.SetMoveState(group.Data.MoveState == 1);
		movableDataObject.SetSide(group.Data.Side);
		if (group.Data.MoveState == 1)
		{
			movableDataObject.SetRotationX(GameManager.Instance.Player.AnimationContainer.localRotation);
			movableDataObject.SetRotationY(GameManager.Instance.Player.HeadContainer.localRotation);
		}
		GameManager.Instance.GameData.CurrentSave.AddData(group.SectionID, movableDataObject);
	}

	private void AddListeners(ref Movable group)
	{
		group.OnEnabled += HandleGroupOnEnabled;
		group.OnDisabled += HandleGroupOnDisabled;
		group.OnFrontEnter += HandleGroupOnFrontEnter;
		group.OnBackEnter += HandleGroupOnBackEnter;
		group.OnExit += HandleGroupOnExit;
	}

	private void RemoveListeners(ref Movable group)
	{
		group.OnEnabled -= HandleGroupOnEnabled;
		group.OnDisabled -= HandleGroupOnDisabled;
		group.OnExit -= HandleGroupOnExit;
		group.OnFrontEnter -= HandleGroupOnFrontEnter;
		group.OnBackEnter -= HandleGroupOnBackEnter;
	}

	public override void SaveData()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			UpdateGroup(m_Group[i]);
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		if (m_Group != null)
		{
			for (int i = 0; i < m_Group.Length; i++)
			{
				Movable group = m_Group[i];
				RemoveListeners(ref group);
			}
		}
	}
}
