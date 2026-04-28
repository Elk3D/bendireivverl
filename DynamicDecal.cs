using UnityEngine;

[AddComponentMenu("Decals/Dynamic Decal")]
public class DynamicDecal : Decal
{
	public override GameObject[] Build()
	{
		m_AffectedObjects = new GameObject[0];
		base.MeshFilter.sharedMesh = null;
		m_AffectedObjects = DecalBuilder.BuildDynamic(this);
		DeserializeMeshData();
		SerializeMeshData();
		return m_AffectedObjects;
	}
}
