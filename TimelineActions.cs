using System;
using UnityEngine;

[Serializable]
public class TimelineActions
{
	public TimelineActionType ActionType;

	public TimelineActionCameraShake CameraShake;

	public TimelineActionDOTween DOTween;

	public void Action()
	{
		if (ActionType == TimelineActionType.CameraShake)
		{
			CameraShake.Action();
		}
		else if (ActionType == TimelineActionType.DOTween)
		{
			DOTween.Action();
		}
		else
		{
			Debug.Log("TimelineActions :: No Action Selected - ActionType = " + ActionType);
		}
	}
}
