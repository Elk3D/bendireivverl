using UnityEngine;

namespace S13Audio.BATDR;

[DefaultExecutionOrder(1100)]
public class BATDRClosableDoorPortal : S13ClosablePortal
{
	[SerializeField]
	private DoorID m_DoorID;

	private S13ClosablePortal m_ClosablePortal;

	private Door m_Door;

	private void Start()
	{
		if (m_DoorID != DoorID.NONE)
		{
			Door[] componentsInChildren = base.transform.root.GetComponentsInChildren<Door>(includeInactive: true);
			if (componentsInChildren != null)
			{
				foreach (Door door in componentsInChildren)
				{
					if (door.ID == m_DoorID)
					{
						m_ClosablePortal = door.S13CloseablePortal;
						m_Door = door;
						break;
					}
				}
			}
		}
		if (!m_ClosablePortal)
		{
			S13Debug.LogWarning("BATDRClosableDoorPortal on " + base.name + " is not set up, removing", base.gameObject);
			Object.Destroy(this);
		}
	}

	public override bool IsOpen()
	{
		if (m_ClosablePortal != null)
		{
			return m_ClosablePortal.IsOpen();
		}
		if (m_Door != null)
		{
			if (m_Door.Data != null)
			{
				if (m_Door.Data.Status != DoorStatus.Open)
				{
					return m_Door.Data.Status == DoorStatus.Open_Disabled;
				}
				return true;
			}
			if (m_Door.Content != null)
			{
				return m_Door.Content.IsActivated;
			}
		}
		return true;
	}
}
