using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionComboLockController : SectionController
{
	private ComboLock[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		m_Group = base.transform.GetComponentsInChildren<ComboLock>(includeInactive: true);
		for (int i = 0; i < m_Group.Length; i++)
		{
			ComboLock comboLock = m_Group[i];
			ComboLockDataObject comboLockDataObject = (ComboLockDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, ComboLockDataObject>(base.Section.SectionID, comboLock.ID);
			if (comboLockDataObject != null)
			{
				if (comboLockDataObject.IsComplete == 1)
				{
					comboLock.ForceComplete();
					continue;
				}
				comboLock.InitializeCode(comboLockDataObject.Code);
				comboLock.OnComplete -= HandleGroupOnComplete;
				comboLock.OnComplete += HandleGroupOnComplete;
			}
			else
			{
				comboLockDataObject = DataObject<int, ComboLockDataObject>.Create(comboLock.ID);
				comboLockDataObject.SetCode(SetCode(comboLock));
				GameManager.Instance.GameData.CurrentSave.AddData(base.Section.SectionID, comboLockDataObject);
				comboLock.Initialize();
				comboLock.OnComplete -= HandleGroupOnComplete;
				comboLock.OnComplete += HandleGroupOnComplete;
			}
		}
		yield return null;
	}

	private void HandleGroupOnComplete(object sender, EventArgs e)
	{
		ComboLock comboLock = sender as ComboLock;
		SaveComplete(comboLock);
	}

	private int SetCode(ComboLock group)
	{
		string text = "";
		for (int i = 0; i < group.Content.NumberWheels.Count; i++)
		{
			text += group.Content.NumberWheels[i].CurrentIndex;
		}
		return text.ParseInt();
	}

	public void SaveComplete(ComboLock group)
	{
		SaveData();
		((ComboLockDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, ComboLockDataObject>(base.Section.SectionID, group.ID))?.SetComplete();
	}

	public override void SaveData()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			ComboLock comboLock = m_Group[i];
			ComboLockDataObject comboLockDataObject = (ComboLockDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, ComboLockDataObject>(base.Section.SectionID, comboLock.ID);
			if (comboLockDataObject != null)
			{
				comboLockDataObject.SetCode(SetCode(comboLock));
				continue;
			}
			comboLockDataObject = DataObject<int, ComboLockDataObject>.Create(comboLock.ID);
			comboLockDataObject.SetCode(SetCode(comboLock));
			GameManager.Instance.GameData.CurrentSave.AddData(base.Section.SectionID, comboLockDataObject);
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		if (m_Group != null)
		{
			for (int i = 0; i < m_Group.Length; i++)
			{
				m_Group[i].OnComplete -= HandleGroupOnComplete;
			}
			m_Group = null;
		}
	}
}
