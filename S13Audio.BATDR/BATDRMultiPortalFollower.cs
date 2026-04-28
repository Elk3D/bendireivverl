using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRMultiPortalFollower : MonoBehaviour, IS13AreaSound
{
	private class PortalDataPoint
	{
		public Collider col;

		public Vector3 pos;

		public float dist;

		public bool isPortal;

		public S13ClosablePortal closePortal;
	}

	[Tooltip("The transform that should be moved toward the target.")]
	public Transform[] followers;

	[Tooltip("Transform whose hierarchy contains the set of portal triggers. Optional, and used only when pressing the 'Get Portal Triggers' button.")]
	[SerializeField]
	private Transform portalContainer;

	[Tooltip("Defines the set of colliders wherein the follower may venture when the target position is outside the area.")]
	public List<Collider> portalTriggers = new List<Collider>();

	[Tooltip("Defines the set of colliders wherein the follower may venture.")]
	public List<Collider> triggers = new List<Collider>();

	private Vector3 _targetPosition = Vector3.zero;

	private bool _targetWithinTrigger;

	private bool _targetWithinPortal;

	private List<PortalDataPoint> _triggerData = new List<PortalDataPoint>();

	public Vector3 targetPosition
	{
		get
		{
			return _targetPosition;
		}
		set
		{
			_targetPosition = value;
			UpdateBoundedPosition();
		}
	}

	public bool targetWithinTrigger
	{
		get
		{
			return _targetWithinTrigger;
		}
		set
		{
			_targetWithinTrigger = value;
		}
	}

	public event Action onAreaEnter;

	public event Action onAreaExit;

	public void GetContainedTriggers()
	{
		triggers = (from x in GetComponentsInChildren<Collider>()
			where x.isTrigger
			select x).ToList();
	}

	public void GetPortalTriggers()
	{
		Collider[] componentsInChildren = portalContainer.GetComponentsInChildren<Collider>();
		portalTriggers = componentsInChildren.Where((Collider x) => x.isTrigger).ToList();
	}

	private void Start()
	{
		_triggerData = new List<PortalDataPoint>(triggers.Count);
		for (int i = 0; i < triggers.Count; i++)
		{
			_triggerData.Add(new PortalDataPoint
			{
				col = triggers[i],
				pos = Vector3.positiveInfinity,
				dist = float.MaxValue,
				isPortal = portalTriggers.Contains(triggers[i]),
				closePortal = triggers[i].GetComponent<S13ClosablePortal>()
			});
		}
	}

	private void HandleInTrigger(bool isPortal)
	{
		if (!targetWithinTrigger)
		{
			S13Debug.Log(base.transform.parent?.name + " " + base.gameObject.name + " " + (isPortal ? "Portal " : " ") + " Entered", base.gameObject, S13Debug.LogType.TriggerMessage);
			this.onAreaEnter?.Invoke();
		}
		targetWithinTrigger = true;
		_targetWithinPortal = isPortal;
	}

	private void HandleOutTrigger()
	{
		if (targetWithinTrigger)
		{
			S13Debug.Log(base.transform.parent?.name + " " + base.gameObject.name + " " + (_targetWithinPortal ? "Portal " : " ") + " Exited", base.gameObject, S13Debug.LogType.TriggerMessage);
			this.onAreaExit?.Invoke();
			_targetWithinPortal = false;
			targetWithinTrigger = false;
		}
	}

	private void UpdateBoundedPosition()
	{
		if (followers.Length == 0 || triggers == null || triggers.Count <= 0 || _triggerData == null || _triggerData.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < _triggerData.Count; i++)
		{
			if (_triggerData[i].isPortal && (bool)_triggerData[i].closePortal && !_triggerData[i].closePortal.IsOpen())
			{
				_triggerData[i].dist = float.MaxValue;
				continue;
			}
			_triggerData[i].pos = _triggerData[i].col.ClosestPoint(targetPosition);
			_triggerData[i].dist = Vector3.Distance(targetPosition, _triggerData[i].pos);
			if (_triggerData[i].dist < float.Epsilon)
			{
				HandleInTrigger(_triggerData[i].isPortal);
			}
		}
		_triggerData.Sort((PortalDataPoint x, PortalDataPoint y) => x.dist.CompareTo(y.dist));
		if (float.IsPositiveInfinity(_triggerData[0].pos.x))
		{
			_ = triggers.First().bounds.center;
		}
		HandleOutTrigger();
		for (int num = 0; num < followers.Length; num++)
		{
			followers[num].position = _triggerData[num].pos;
		}
	}
}
