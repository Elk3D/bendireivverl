using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class LightGroup : MonoBehaviour
{
	[Header("General")]
	public bool startOff = true;

	[Header("Bounds")]
	[Tooltip("When true, will use the bounds to turn on this volume if the player is contained by any of the child bounds. When false, you will have to manually turn on and off this Light Group, via the LightGroups public API.")]
	public bool useBounds = true;

	[Header("Debugging")]
	[SerializeField]
	private SimpleCull sc;

	[SerializeField]
	private bool isOn = true;

	private HashSet<BoundObject> boundsList = new HashSet<BoundObject>();

	private HashSet<Light> lights = new HashSet<Light>();

	private HashSet<VideoPlayer> videoPlayerList = new HashSet<VideoPlayer>();

	private bool _started;

	private void Awake()
	{
		boundsList.Clear();
		boundsList.AddRange(GetComponentsInChildren<BoundObject>());
		isOn = true;
	}

	private IEnumerator Start()
	{
		int counter = 100;
		do
		{
			yield return null;
			sc = Object.FindObjectOfType<SimpleCull>();
		}
		while (!sc && counter-- > 0);
		if (counter <= 0)
		{
			Debug.LogError(Log("Unable to find instance of SimpleCull..."), this);
			yield break;
		}
		counter = 100;
		do
		{
			yield return null;
		}
		while (!sc.cullTransform && counter-- > 0);
		if (counter <= 0)
		{
			Debug.LogError(Log("Unable to find sc.cullTransform"), this);
			yield break;
		}
		SetupLights(sc.GetLightList());
		SetupVideoPlayers();
		if (startOff)
		{
			ToggleLightGroup(turnOn: false);
		}
		_started = true;
	}

	private void Update()
	{
		if (_started && useBounds)
		{
			ToggleLightGroup(Contains(sc.cullTransform.position));
		}
	}

	public void OnDrawGizmosSelected()
	{
		foreach (Light light in lights)
		{
			if ((bool)light && light.enabled)
			{
				Gizmos.color = (light.enabled ? Color.green : Color.red);
				Gizmos.DrawWireSphere(light.transform.position, 3f);
			}
		}
		if (!Application.isPlaying || !useBounds)
		{
			return;
		}
		Gizmos.color = (isOn ? Color.green : Color.red);
		foreach (BoundObject bounds in boundsList)
		{
			if ((bool)bounds)
			{
				Gizmos.DrawWireCube(bounds.bounds.center, bounds.bounds.size);
			}
		}
	}

	public bool Contains(Vector3 point)
	{
		foreach (BoundObject bounds in boundsList)
		{
			if (bounds.bounds.Contains(point))
			{
				return true;
			}
		}
		return false;
	}

	public void ToggleLightGroup(bool turnOn)
	{
		if (turnOn && !isOn)
		{
			if ((bool)sc)
			{
				sc.AddLights(lights);
			}
			videoPlayerList.ForEach(delegate(VideoPlayer x)
			{
				if ((bool)x && (bool)x.gameObject && !x.gameObject.activeSelf)
				{
					x.gameObject.SetActive(value: true);
				}
			});
		}
		else if (!turnOn && isOn)
		{
			if ((bool)sc)
			{
				sc.RemoveLights(lights);
			}
			lights.ForEach(delegate(Light x)
			{
				if ((bool)x && x.enabled)
				{
					x.enabled = false;
				}
			});
			videoPlayerList.ForEach(delegate(VideoPlayer x)
			{
				if ((bool)x && (bool)x.gameObject && x.gameObject.activeSelf)
				{
					x.gameObject.SetActive(value: false);
				}
			});
		}
		isOn = turnOn;
	}

	private void SetupLights(IEnumerable<Light> newLights)
	{
		lights.Clear();
		foreach (Light newLight in newLights)
		{
			if (!newLight || (bool)newLight.GetComponent<NoLightCullingMarker>())
			{
				continue;
			}
			foreach (BoundObject bounds in boundsList)
			{
				if (bounds.includeLightsInBounds && bounds.bounds.Contains(newLight.transform.position))
				{
					lights.Add(newLight);
				}
			}
		}
	}

	private void SetupVideoPlayers()
	{
		videoPlayerList.Clear();
		VideoPlayer[] array = Object.FindObjectsOfType<VideoPlayer>();
		foreach (VideoPlayer videoPlayer in array)
		{
			if (!videoPlayer || !videoPlayer.gameObject.activeSelf || !videoPlayer.playOnAwake)
			{
				continue;
			}
			foreach (BoundObject bounds in boundsList)
			{
				if (bounds.includeLightsInBounds && bounds.bounds.Contains(videoPlayer.transform.position))
				{
					videoPlayerList.Add(videoPlayer);
				}
			}
		}
	}

	private string Log(string msg)
	{
		return "[LIGHT GROUP (" + base.gameObject.name + ")] " + msg;
	}
}
