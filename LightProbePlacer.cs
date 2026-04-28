using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LightProbeGroup))]
public class LightProbePlacer : MonoBehaviour
{
	public Vector3 Volume = Vector3.one;

	private const float Spacing = 3f;

	private const float WallCheckDistance = 2f;

	private static LayerMask ObjectLayers;

	private const float padding = 5f;

	public static void GenerateLightProbes()
	{
		LightProbePlacer probeGroup = GetProbeGroup();
		List<Vector3> list = new List<Vector3>();
		ObjectLayers = LayerMask.GetMask("Default", "NavMesh");
		int num = 0;
		GetSceneBounds(probeGroup);
		for (float num2 = 0f; num2 < probeGroup.Volume.x; num2 += 3f)
		{
			for (float num3 = 0f; num3 < probeGroup.Volume.z; num3 += 3f)
			{
				for (float num4 = 0f; num4 < probeGroup.Volume.y; num4 += 3f)
				{
					Vector3 vector = new Vector3(num2, num4, num3);
					if (checkDistances(probeGroup.transform.position + vector))
					{
						list.Add(vector);
						num++;
						if (num > 200000)
						{
							Debug.LogError("Too many probes! Reduce volume!");
							return;
						}
					}
				}
			}
		}
		Debug.LogWarning("Probe Count:" + num);
	}

	public static bool checkDistances(Vector3 Position)
	{
		if (Physics.CheckSphere(Position, 2f, ObjectLayers, QueryTriggerInteraction.Ignore) && !Physics.CheckSphere(Position, 0.25f, ObjectLayers, QueryTriggerInteraction.Ignore))
		{
			return true;
		}
		return false;
	}

	public static LightProbePlacer GetProbeGroup()
	{
		LightProbePlacer lightProbePlacer = Object.FindObjectOfType<LightProbePlacer>();
		if (lightProbePlacer == null)
		{
			GameObject obj = new GameObject();
			lightProbePlacer = obj.AddComponent<LightProbePlacer>();
			obj.name = "LIGHT_PROBE_GROUP";
		}
		return lightProbePlacer;
	}

	public static void GetSceneBounds(LightProbePlacer ProbeGroup)
	{
		Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
		Object[] array = Object.FindObjectsOfType(typeof(Renderer));
		for (int i = 0; i < array.Length; i++)
		{
			Renderer renderer = (Renderer)array[i];
			bounds.Encapsulate(renderer.bounds);
		}
		ProbeGroup.Volume = bounds.size + Vector3.one * 5f * 2f;
		ProbeGroup.transform.position = bounds.center - bounds.extents - Vector3.one * 5f;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(base.transform.position + Volume / 2f, Volume);
	}
}
