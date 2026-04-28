using System;
using DG.Tweening;
using S13Audio.Malee;
using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRDOTweenHook : MonoBehaviour
{
	private enum TweenEventType
	{
		OnTweenCreated,
		OnStart,
		OnPlay,
		OnUpdate,
		OnStepComplete,
		OnComplete,
		OnRewind
	}

	[Serializable]
	private class TweenEventActionList : ReorderableArray<TweenEventAction>
	{
	}

	[Serializable]
	[S13HorizontalPropertyHolder]
	private class TweenEventAction
	{
		public TweenEventType type;

		public S13LocalAction action;

		private MonoBehaviour _enableCheck;

		public void Init(MonoBehaviour enableCheck)
		{
			_enableCheck = enableCheck;
		}

		public void Execute()
		{
			if ((bool)_enableCheck && _enableCheck.enabled)
			{
				action.Execute();
			}
		}
	}

	[SerializeField]
	[Tooltip("Target DOTweenAnimation that we will attach to")]
	private DOTweenAnimation target;

	[Reorderable]
	[SerializeField]
	private TweenEventActionList tweenEventActions;

	private void Start()
	{
		if (!target)
		{
			return;
		}
		foreach (TweenEventAction tweenEventAction in tweenEventActions)
		{
			if (tweenEventAction.action.IsExecutable)
			{
				tweenEventAction.Init(this);
				switch (tweenEventAction.type)
				{
				case TweenEventType.OnTweenCreated:
					target.onTweenCreated?.AddListener(tweenEventAction.Execute);
					target.hasOnTweenCreated = true;
					break;
				case TweenEventType.OnStart:
					target.onStart?.AddListener(tweenEventAction.Execute);
					target.hasOnStart = true;
					break;
				case TweenEventType.OnPlay:
					target.onPlay?.AddListener(tweenEventAction.Execute);
					target.hasOnPlay = true;
					break;
				case TweenEventType.OnUpdate:
					target.onUpdate?.AddListener(tweenEventAction.Execute);
					target.hasOnUpdate = true;
					break;
				case TweenEventType.OnStepComplete:
					target.onStepComplete?.AddListener(tweenEventAction.Execute);
					target.hasOnStepComplete = true;
					break;
				case TweenEventType.OnComplete:
					target.onRewind?.AddListener(tweenEventAction.Execute);
					target.hasOnRewind = true;
					break;
				case TweenEventType.OnRewind:
					target.onRewind?.AddListener(tweenEventAction.Execute);
					target.hasOnComplete = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}
