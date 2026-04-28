using UnityEngine;

public class testmeshbuilder : JMonoBehaviour
{
	public SkinnedMeshRenderer skinnedmeshrenderer;

	public MeshFilter meshfilter;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.B))
		{
			Mesh mesh = new Mesh();
			skinnedmeshrenderer.BakeMesh(mesh);
			meshfilter.mesh = mesh;
		}
	}
}
