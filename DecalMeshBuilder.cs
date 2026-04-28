using System.Collections.Generic;
using UnityEngine;

public class DecalMeshBuilder
{
	private readonly List<Vector3> vertices = new List<Vector3>();

	private readonly List<Vector3> normals = new List<Vector3>();

	private readonly List<Vector2> texCoords = new List<Vector2>();

	private readonly List<int> indices = new List<int>();

	public void AddPolygon(Vector3[] poly, Vector3 normal, Rect uvRect)
	{
		int item = AddVertex(poly[0], normal, uvRect);
		for (int i = 1; i < poly.Length - 1; i++)
		{
			int item2 = AddVertex(poly[i], normal, uvRect);
			int item3 = AddVertex(poly[i + 1], normal, uvRect);
			indices.Add(item);
			indices.Add(item2);
			indices.Add(item3);
		}
	}

	private int AddVertex(Vector3 vertex, Vector3 normal, Rect uvRect)
	{
		int num = FindVertex(vertex);
		if (num == -1)
		{
			vertices.Add(vertex);
			normals.Add(normal);
			AddTexCoord(vertex, uvRect);
			return vertices.Count - 1;
		}
		normals[num] = (normals[num] + normal).normalized;
		return num;
	}

	private int FindVertex(Vector3 vertex)
	{
		for (int i = 0; i < vertices.Count; i++)
		{
			if (Vector3.Distance(vertices[i], vertex) < 0.01f)
			{
				return i;
			}
		}
		return -1;
	}

	private void AddTexCoord(Vector3 ver, Rect uvRect)
	{
		float x = Mathf.Lerp(uvRect.xMin, uvRect.xMax, ver.x + 0.5f);
		float y = Mathf.Lerp(uvRect.yMin, uvRect.yMax, ver.y + 0.5f);
		texCoords.Add(new Vector2(x, y));
	}

	public void Push(float distance)
	{
		for (int i = 0; i < vertices.Count; i++)
		{
			vertices[i] += normals[i] * distance;
		}
	}

	public void ToMesh(Mesh mesh)
	{
		mesh.Clear(keepVertexLayout: true);
		if (indices.Count != 0)
		{
			mesh.vertices = vertices.ToArray();
			mesh.normals = normals.ToArray();
			mesh.uv = texCoords.ToArray();
			mesh.uv2 = texCoords.ToArray();
			mesh.triangles = indices.ToArray();
			vertices.Clear();
			normals.Clear();
			texCoords.Clear();
			indices.Clear();
		}
	}
}
