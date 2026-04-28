using System;
using UnityEngine;

[Serializable]
public class CharacterInteraction
{
	public Animator InteractableProp;

	public string AnimationTrigger;

	public ActionEventController ActionEvent;

	public TimelineUnityEvent TimelineUnityEvent;

	public bool IsTriggered { get; private set; }

	public void Trigger()
	{
		if (InteractableProp != null)
		{
			InteractableProp.SetTrigger(AnimationTrigger);
		}
		else if (ActionEvent != null)
		{
			ActionEvent.Content.Action();
		}
		if (TimelineUnityEvent != null)
		{
			TimelineUnityEvent.Action();
		}
		IsTriggered = true;
	}

	public void ResetTrigger()
	{
		IsTriggered = false;
	}
}
