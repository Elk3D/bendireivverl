using System.Linq;
using UnityEngine;

public static class DecalBuilder
{
	private static readonly DecalMeshBuilder builder = new DecalMeshBuilder();

	public static GameObject[] BuildDynamic(Decal decal)
	{
		return Build(decal, isStatic: false);
	}

	public static GameObject[] BuildStatic(Decal decal)
	{
		return Build(decal, isStatic: true);
	}

	private static GameObject[] Build(Decal decal, bool isStatic)
	{
		MeshFilter meshFilter = decal.MeshFilter;
		MeshRenderer meshRenderer = decal.MeshRenderer;
		if (meshFilter.sharedMesh != null && !meshFilter.sharedMesh.isReadable)
		{
			return null;
		}
		if (decal.data.SelectedDecalMaterial == null || decal.data.Texture == null)
		{
			Object.DestroyImmediate(meshFilter.sharedMesh);
			meshFilter.sharedMesh = null;
			meshRenderer.sharedMaterial = null;
			return null;
		}
		MeshFilter[] affectedObjects = DecalUtility.GetAffectedObjects(decal);
		MeshFilter[] array = affectedObjects;
		foreach (MeshFilter meshFilter2 in array)
		{
			if (isStatic)
			{
				BuildStatic(decal, meshFilter2);
			}
			else
			{
				BuildDynamic(decal, meshFilter2);
			}
		}
		builder.Push(decal.pushDistance);
		if (meshFilter.sharedMesh == null)
		{
			meshFilter.sharedMesh = new Mesh();
			meshFilter.sharedMesh.name = "StaticDecal";
		}
		builder.ToMesh(meshFilter.sharedMesh);
		return affectedObjects.Select((MeshFilter meshFilter3) => meshFilter3.gameObject).ToArray();
	}

	private static void BuildStatic(Decal decal, MeshFilter @object)
	{
		Mesh sharedMesh = @object.sharedMesh;
		if (sharedMesh.isReadable)
		{
			BuildMesh(decal, sharedMesh, @object.transform);
		}
	}

	private static void BuildDynamic(Decal decal, MeshFilter @object)
	{
		Mesh sharedMesh = @object.sharedMesh;
		if (!sharedMesh.isReadable)
		{
			MeshCollider component = @object.GetComponent<MeshCollider>();
			if (component == null)
			{
				return;
			}
			sharedMesh = component.sharedMesh;
		}
		BuildMesh(decal, sharedMesh, @object.transform);
	}

	private static void BuildMesh(Decal decal, Mesh mesh, Transform _transform)
	{
		Matrix4x4 matrix4x = decal.transform.worldToLocalMatrix * _transform.localToWorldMatrix;
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;
		for (int i = 0; i < triangles.Length; i += 3)
		{
			int num = triangles[i];
			int num2 = triangles[i + 1];
			int num3 = triangles[i + 2];
			Vector3 v = matrix4x.MultiplyPoint(vertices[num]);
			Vector3 v2 = matrix4x.MultiplyPoint(vertices[num2]);
			Vector3 v3 = matrix4x.MultiplyPoint(vertices[num3]);
			AddTriangle(decal, v, v2, v3);
		}
	}

	private static void AddTriangle(Decal decal, Vector3 v1, Vector3 v2, Vector3 v3)
	{
		Rect uvRect = To01(new Rect(0f, 0f, decal.data.Texture.width, decal.data.Texture.height), decal.data.Texture);
		Vector3 normalized = Vector3.Cross(v2 - v1, v3 - v1).normalized;
		if (Vector3.Angle(Vector3.forward, -normalized) <= decal.maxAngle)
		{
			Vector3[] array = PolygonClippingUtility.Clip(v1, v2, v3);
			if (array.Length != 0)
			{
				builder.AddPolygon(array, normalized, uvRect);
			}
		}
	}

	private static Rect To01(Rect rect, Texture2D texture)
	{
		rect.x /= texture.width;
		rect.y /= texture.height;
		rect.width /= texture.width;
		rect.height /= texture.height;
		return rect;
	}
}
