using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class SimpleCull : MonoBehaviour
{
	[FormerlySerializedAs("overrideMaxDist")]
	public bool overrideCullingDistance;

	[FormerlySerializedAs("lightCullingDistance")]
	[Tooltip("When 'override culling distance' is false, this is set to the far clipping plane of the player camera.")]
	public float cullingDistance = 100f;

	[Header("Debugging")]
	[SerializeField]
	private Transform _cullTransform;

	[SerializeField]
	private Camera playerCamera;

	private HashSet<ParticleSystem> particleSystems = new HashSet<ParticleSystem>();

	private HashSet<Light> _lightSet = new HashSet<Light>();

	private Vector3 lightVector;

	private Vector3 itemPosition;

	public Transform cullTransform => _cullTransform;

	private void Awake()
	{
		SetupLights();
		SetupParticleSystems();
	}

	private IEnumerator Start()
	{
		int counter = 100;
		playerCamera = null;
		do
		{
			playerCamera = GameManager.Instance.GameCamera.Camera;
			yield return null;
		}
		while (playerCamera == null && counter-- > 0);
		if (counter <= 0)
		{
			Debug.LogError(Log("Could not find GameManager.Instance.GameCamera...."), this);
			yield break;
		}
		_cullTransform = playerCamera.transform;
		float farClipPlane = playerCamera.farClipPlane;
		if (!overrideCullingDistance)
		{
			cullingDistance = farClipPlane;
		}
	}

	private void Update()
	{
		if ((bool)_cullTransform)
		{
			CullLights();
			CullParticleSystems();
		}
	}

	private void OnDrawGizmosSelected()
	{
		foreach (Light item in _lightSet)
		{
			if ((bool)item)
			{
				Gizmos.color = (item.enabled ? Color.green : Color.red);
				Gizmos.DrawWireSphere(item.transform.position, 3f);
			}
		}
		foreach (ParticleSystem particleSystem in particleSystems)
		{
			if ((bool)particleSystem)
			{
				Gizmos.color = (particleSystem.isPaused ? Color.red : Color.green);
				Gizmos.DrawWireCube(particleSystem.transform.position, Vector3.one * 3f);
			}
		}
	}

	public IEnumerable<Light> GetLightList()
	{
		return _lightSet.ToList();
	}

	public void AddLights(IEnumerable<Light> list)
	{
		_lightSet.AddRange(list);
	}

	public void RemoveLights(IEnumerable<Light> list)
	{
		_lightSet.RemoveRange(list);
	}

	private void CullLights()
	{
		Vector3 forward = _cullTransform.forward;
		foreach (Light item in _lightSet)
		{
			if (!item)
			{
				continue;
			}
			itemPosition = item.transform.position;
			lightVector = itemPosition - _cullTransform.position;
			float num = cullingDistance + item.range;
			if (lightVector.sqrMagnitude > num * num)
			{
				if (item.enabled)
				{
					item.enabled = false;
				}
			}
			else if (Vector3.Dot(lightVector, forward) < 0f && Vector3.Project(lightVector, forward).sqrMagnitude > item.range * item.range)
			{
				if (item.enabled)
				{
					item.enabled = false;
				}
			}
			else if (!item.enabled)
			{
				item.enabled = true;
			}
		}
	}

	private void SetupLights()
	{
		_lightSet.Clear();
		_lightSet.AddRange(Object.FindObjectsOfType<Light>());
		List<Light> list = new List<Light>();
		foreach (Light item in _lightSet)
		{
			if (!item.isActiveAndEnabled || !item.gameObject.activeInHierarchy)
			{
				list.Add(item);
			}
			else if ((bool)item.gameObject.GetComponentInParent<NoLightCullingMarker>())
			{
				list.Add(item);
			}
		}
		_lightSet.RemoveRange(list);
	}

	private void CullParticleSystems()
	{
		float num = cullingDistance * cullingDistance;
		foreach (ParticleSystem particleSystem in particleSystems)
		{
			if (!particleSystem)
			{
				continue;
			}
			if ((_cullTransform.position - particleSystem.transform.position).sqrMagnitude > num)
			{
				if (!particleSystem.isPaused)
				{
					particleSystem.Pause();
				}
			}
			else if (!particleSystem.isPlaying)
			{
				particleSystem.Play();
			}
		}
	}

	private void SetupParticleSystems()
	{
		particleSystems.Clear();
		ParticleSystem[] array = Object.FindObjectsOfType<ParticleSystem>();
		foreach (ParticleSystem particleSystem in array)
		{
			if (particleSystem.main.loop && particleSystem.gameObject.activeInHierarchy && particleSystem.isPlaying && !particleSystem.GetComponent<ParticleSystemPause>())
			{
				particleSystems.Add(particleSystem);
			}
		}
	}

	private string Log(string msg)
	{
		return "[SIMPLE CULL] " + msg;
	}
}
