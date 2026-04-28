using System;
using UnityEngine;

[Serializable]
public class CompanionInteraction
{
	public Animator InteractableProp;

	public string AnimationTrigger;

	public ActionEventController ActionEvent;

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
	}
}
