using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace S13Audio.BATDR;

public class BATDRInkWidowSwarmMonitor : MonoBehaviour
{
	private enum SwarmSize
	{
		None,
		Small,
		Large
	}

	public static HashSet<BATRDRInkWidowSwarmEntry> swarmList = new HashSet<BATRDRInkWidowSwarmEntry>();

	[Header("Swarm Position Tracking")]
	[SerializeField]
	private GameObject swarmPositionFollower;

	[SerializeField]
	[Tooltip("Max movement the position follower can move in an update call, used to prevent snapping around")]
	private float swarmPositionMovementDelta = 1f;

	[SerializeField]
	[Tooltip("Max distance an ink widow can be from swarm monitor to be included in the average position calculation")]
	private float maxWidowDistance = 30f;

	[Header("Emitter Follower Settings")]
	[SerializeField]
	private GameObject emitterFollower;

	[SerializeField]
	private float emitterDistance = 15f;

	[Header("Parameter Settings")]
	[SerializeField]
	[S13LockableField]
	[FormerlySerializedAs("paramName")]
	private string accessorID = string.Empty;

	[SerializeField]
	private int maxSwarmSize = 15;

	[Header("Small Swarm")]
	[SerializeField]
	private int minSmallSwarmSize = 5;

	[SerializeField]
	private S13LocalAction onSmallSwarmAction;

	[SerializeField]
	private S13LocalAction onDropBelowSmallAction;

	[Header("Large Swarm")]
	[SerializeField]
	private int minLargeSwarmSize = 10;

	[SerializeField]
	private S13LocalAction onLargeSwarmAction;

	[SerializeField]
	private S13LocalAction onDropBelowLargeAction;

	[Header("SwarmSize")]
	[SerializeField]
	[ReadOnly]
	private int swarmCount;

	[SerializeField]
	[ReadOnly]
	private SwarmSize currentSwarmSize;

	private Transform _listenerTransform;

	private Transform ListenerTransform
	{
		get
		{
			if (!_listenerTransform)
			{
				_listenerTransform = UnityEngine.Object.FindObjectOfType<AudioListener>().transform;
			}
			return _listenerTransform;
		}
	}

	private void Start()
	{
		if (swarmPositionFollower == null || emitterFollower == null)
		{
			S13Debug.LogError("BATDRInkWidowSwarmMonitor does not have its followers assigned, disabling");
			base.enabled = false;
		}
	}

	private void Update()
	{
		swarmCount = swarmList.Count;
		if (swarmCount > 0)
		{
			PositionSwarmFollower();
		}
		PositionEmitter();
		CheckForSwarmResize();
		if (!string.IsNullOrEmpty(accessorID))
		{
			S13Manager.SetAccessor(accessorID, (float)swarmCount / (float)maxSwarmSize);
		}
	}

	private void PositionSwarmFollower()
	{
		Vector3 zero = Vector3.zero;
		int num = 0;
		foreach (BATRDRInkWidowSwarmEntry swarm in swarmList)
		{
			if (!(Vector3.Distance(swarm.transform.position, swarmPositionFollower.transform.position) > maxWidowDistance))
			{
				zero += swarm.transform.position;
				num++;
			}
		}
		if (num != 0 && (bool)swarmPositionFollower)
		{
			Vector3 target = zero / num;
			swarmPositionFollower.transform.position = Vector3.MoveTowards(swarmPositionFollower.transform.position, target, swarmPositionMovementDelta);
		}
	}

	private void PositionEmitter()
	{
		if ((bool)emitterFollower)
		{
			Vector3 vector = swarmPositionFollower.transform.position - ListenerTransform.position;
			vector.y = 0f;
			if (vector.magnitude < emitterDistance)
			{
				emitterFollower.transform.position = ListenerTransform.position + vector;
				return;
			}
			vector.Normalize();
			emitterFollower.transform.position = ListenerTransform.position + vector * emitterDistance;
		}
	}

	private void CheckForSwarmResize()
	{
		switch (currentSwarmSize)
		{
		case SwarmSize.None:
			if (swarmCount >= minSmallSwarmSize)
			{
				onSmallSwarmAction.Execute();
				currentSwarmSize = SwarmSize.Small;
			}
			break;
		case SwarmSize.Small:
			if (swarmCount < minSmallSwarmSize)
			{
				onDropBelowSmallAction.Execute();
				currentSwarmSize = SwarmSize.None;
			}
			else if (swarmCount >= minLargeSwarmSize)
			{
				onLargeSwarmAction.Execute();
				currentSwarmSize = SwarmSize.Large;
			}
			break;
		case SwarmSize.Large:
			if (swarmCount < minLargeSwarmSize)
			{
				onDropBelowLargeAction.Execute();
				currentSwarmSize = SwarmSize.Small;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public static void RegisterInkWidow(BATRDRInkWidowSwarmEntry entry)
	{
		swarmList.Add(entry);
	}

	public static void DeRegisterInkWidow(BATRDRInkWidowSwarmEntry entry)
	{
		swarmList.Remove(entry);
	}
}
