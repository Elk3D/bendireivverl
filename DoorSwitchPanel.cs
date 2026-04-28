using System;
using DG.Tweening;
using UnityEngine;

public class DoorSwitchPanel : JMonoBehaviour
{
	[Serializable]
	private class DoorSwitchGroup
	{
		public DoorID DoorID;

		public LightFixture LightFixture;
	}

	[Header("Button")]
	[SerializeField]
	private Interactable m_InteractableButton;

	[SerializeField]
	private Transform m_Button;

	[Header("Door Switch Group")]
	[SerializeField]
	private DoorSwitchGroup[] m_Group;

	private Sequence m_ButtonSequence;

	public void Initialize()
	{
		Door[] componentsInChildren = base.transform.root.GetComponentsInChildren<Door>(includeInactive: true);
		for (int i = 0; i < m_Group.Length; i++)
		{
			DoorSwitchGroup doorSwitchGroup = m_Group[i];
			Door[] array = componentsInChildren;
			foreach (Door door in array)
			{
				if (door.ID == doorSwitchGroup.DoorID)
				{
					if (door.Data.Status == DoorStatus.Open)
					{
						doorSwitchGroup.LightFixture.SetEmission(1f);
					}
					else if (door.Data.Status == DoorStatus.Closed)
					{
						doorSwitchGroup.LightFixture.SetEmission(0f);
					}
				}
			}
		}
		m_InteractableButton.OnInteract -= HandleButtonOnInteract;
		m_InteractableButton.OnInteract += HandleButtonOnInteract;
	}

	private void HandleButtonOnInteract(object sender, EventArgs e)
	{
		m_InteractableButton.OnInteract -= HandleButtonOnInteract;
		Door[] componentsInChildren = base.transform.root.GetComponentsInChildren<Door>(includeInactive: true);
		int num = 0;
		bool flag = false;
		Door[] array;
		for (int i = 0; i < m_Group.Length; i++)
		{
			DoorSwitchGroup doorSwitchGroup = m_Group[i];
			array = componentsInChildren;
			foreach (Door door in array)
			{
				if (door.ID == doorSwitchGroup.DoorID && door.Data.Status == DoorStatus.Open)
				{
					door.Content.ForceDeactivate();
					doorSwitchGroup.LightFixture.SetEmission(0f);
					flag = true;
					break;
				}
			}
			num++;
			if (flag)
			{
				break;
			}
		}
		if (num >= m_Group.Length)
		{
			num = 0;
		}
		m_Group[num].LightFixture.SetEmission(1f);
		array = componentsInChildren;
		foreach (Door door2 in array)
		{
			if (door2.ID == m_Group[num].DoorID)
			{
				door2.Content.ForceActivate();
				break;
			}
		}
		ResetSequence();
		m_ButtonSequence.Insert(0f, m_Button.DOLocalMoveX(0.23f, 0.2f).SetEase(Ease.Linear));
		m_ButtonSequence.Insert(0.2f, m_Button.DOLocalMoveX(0f, 0.2f).SetEase(Ease.OutSine));
		m_ButtonSequence.OnComplete(delegate
		{
			m_InteractableButton.OnInteract += HandleButtonOnInteract;
			m_InteractableButton.ResetAction();
		});
	}

	private void ResetSequence()
	{
		KillSequence();
		m_ButtonSequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_ButtonSequence != null)
		{
			m_ButtonSequence.Kill();
			m_ButtonSequence = null;
		}
	}

	protected override void OnDisposed()
	{
		KillSequence();
		m_InteractableButton.OnInteract -= HandleButtonOnInteract;
		base.OnDisposed();
	}
}
