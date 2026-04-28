using UnityEngine;

[AddComponentMenu("Decals/Static Decal")]
public class StaticDecal : Decal
{
	public override void OnEnable()
	{
		if (Application.isPlaying)
		{
			base.enabled = false;
			DeserializeMeshData();
		}
	}

	public override GameObject[] Build()
	{
		m_AffectedObjects = new GameObject[0];
		base.MeshFilter.sharedMesh = null;
		m_AffectedObjects = DecalBuilder.BuildStatic(this);
		DeserializeMeshData();
		SerializeMeshData();
		return m_AffectedObjects;
	}
}
