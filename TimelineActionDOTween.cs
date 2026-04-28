using System;
using DG.Tweening;

[Serializable]
public class TimelineActionDOTween
{
	public DOTweenAnimation DOTweenAnimation;

	public bool Stop;

	public void Action()
	{
		if (!Stop)
		{
			DOTweenAnimation?.DOPlay();
		}
		else
		{
			DOTweenAnimation?.DOPause();
		}
	}
}
