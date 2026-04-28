using System;
using UnityEngine;

namespace S13Audio.BATDR;

[Serializable]
public class S13AreaHandlerReference : IS13AreaSound
{
	[SerializeField]
	private S13PortalFocusAreaSound portalFocusAreaSound;

	[SerializeField]
	private S13SimpleAreaSound simpleAreaSound;

	public Vector3 targetPosition
	{
		get
		{
			if ((bool)portalFocusAreaSound)
			{
				return portalFocusAreaSound.targetPosition;
			}
			if ((bool)simpleAreaSound)
			{
				return simpleAreaSound.targetPosition;
			}
			return Vector3.zero;
		}
		set
		{
			if ((bool)portalFocusAreaSound)
			{
				portalFocusAreaSound.targetPosition = value;
			}
			else if ((bool)simpleAreaSound)
			{
				simpleAreaSound.targetPosition = value;
			}
		}
	}

	public bool targetWithinTrigger
	{
		get
		{
			if ((bool)portalFocusAreaSound)
			{
				return portalFocusAreaSound.targetWithinTrigger;
			}
			if ((bool)simpleAreaSound)
			{
				return simpleAreaSound.targetWithinTrigger;
			}
			return false;
		}
	}

	public string name
	{
		get
		{
			if ((bool)portalFocusAreaSound)
			{
				return portalFocusAreaSound.name;
			}
			if ((bool)simpleAreaSound)
			{
				return simpleAreaSound.name;
			}
			return "Empty S13AreaHandlerReference";
		}
	}

	public event Action onAreaEnter
	{
		add
		{
			if ((bool)portalFocusAreaSound)
			{
				portalFocusAreaSound.onAreaEnter += value;
			}
			else if ((bool)simpleAreaSound)
			{
				simpleAreaSound.onAreaEnter += value;
			}
		}
		remove
		{
			if ((bool)portalFocusAreaSound)
			{
				portalFocusAreaSound.onAreaEnter -= value;
			}
			else if ((bool)simpleAreaSound)
			{
				simpleAreaSound.onAreaEnter -= value;
			}
		}
	}

	public event Action onAreaExit
	{
		add
		{
			if ((bool)portalFocusAreaSound)
			{
				portalFocusAreaSound.onAreaExit += value;
			}
			else if ((bool)simpleAreaSound)
			{
				simpleAreaSound.onAreaExit += value;
			}
		}
		remove
		{
			if ((bool)portalFocusAreaSound)
			{
				portalFocusAreaSound.onAreaExit -= value;
			}
			else if ((bool)simpleAreaSound)
			{
				simpleAreaSound.onAreaExit -= value;
			}
		}
	}
}
