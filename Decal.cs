using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public abstract class Decal : JMonoBehaviour
{
	[SerializeField]
	protected Vector3[] vertices;

	[SerializeField]
	protected Vector3[] normals;

	[SerializeField]
	protected Vector2[] uv;

	[SerializeField]
	protected Vector2[] uv2;

	[SerializeField]
	protected int[] triangles;

	[SerializeField]
	public DecalData data;

	protected GameObject[] m_AffectedObjects;

	protected MaterialPropertyBlock m_MaterialPropertyBlock;

	public float maxAngle = 90f;

	public float pushDistance = 0.0005f;

	public LayerMask affectedLayers = -1;

	[SerializeField]
	protected MeshFilter m_MeshFilter;

	[SerializeField]
	protected MeshRenderer m_MeshRenderer;

	[SerializeField]
	protected Transform m_MeshTransform;

	public MeshFilter MeshFilter => m_MeshFilter ?? (m_MeshFilter = MeshTransform?.GetComponent<MeshFilter>());

	public MeshRenderer MeshRenderer => m_MeshRenderer ?? (m_MeshRenderer = MeshTransform?.GetComponent<MeshRenderer>());

	public Transform MeshTransform => m_MeshTransform ?? (m_MeshTransform = ((base.transform.childCount > 0) ? base.transform.GetChild(0) : null));

	public abstract GameObject[] Build();

	protected void DeserializeMeshData()
	{
		ResetMeshTransform();
		GetRendererMaterial();
		if (!(MeshFilter.sharedMesh != null) && vertices != null && vertices.Length != 0)
		{
			MeshFilter.sharedMesh = new Mesh
			{
				vertices = vertices,
				normals = normals,
				uv = uv,
				uv2 = uv2,
				triangles = triangles
			};
		}
	}

	private void ResetMeshTransform()
	{
		if (!(MeshTransform == null))
		{
			if (MeshTransform.localPosition != Vector3.zero)
			{
				MeshTransform.localPosition = Vector3.zero;
			}
			if (MeshTransform.localEulerAngles != Vector3.zero)
			{
				MeshTransform.localEulerAngles = Vector3.zero;
			}
			if (MeshTransform.localScale != Vector3.one)
			{
				MeshTransform.localScale = Vector3.one;
			}
			CheckHideFlags();
		}
	}

	private void CheckHideFlags()
	{
	}

	private void GetRendererMaterial()
	{
		if (data != null && (m_MaterialPropertyBlock == null || !(m_MaterialPropertyBlock.GetTexture("_MainTex") == data.Texture)))
		{
			m_MaterialPropertyBlock = new MaterialPropertyBlock();
			MeshRenderer.GetPropertyBlock(m_MaterialPropertyBlock);
			MeshRenderer.sharedMaterial = data.SelectedDecalMaterial;
			if ((bool)data.Texture)
			{
				m_MaterialPropertyBlock.SetTexture("_MainTex", data.Texture);
			}
			if ((bool)data.Emissive)
			{
				m_MaterialPropertyBlock.SetTexture("_Emissive", data.Emissive);
			}
			if ((bool)data.Normals)
			{
				m_MaterialPropertyBlock.SetTexture("_Normal", data.Normals);
			}
			m_MaterialPropertyBlock.SetColor("_Color", data.Color);
			m_MaterialPropertyBlock.SetColor("_EmissiveColor", data.EmissiveColor);
			m_MaterialPropertyBlock.SetFloat("_Specular", data.Specular);
			m_MaterialPropertyBlock.SetFloat("_Alpha", data.Alpha);
			MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
		}
	}

	protected void SerializeMeshData()
	{
		Mesh sharedMesh = MeshFilter.sharedMesh;
		if (!(sharedMesh == null))
		{
			vertices = sharedMesh.vertices;
			normals = sharedMesh.normals;
			uv = sharedMesh.uv;
			uv2 = sharedMesh.uv2;
			triangles = sharedMesh.triangles;
		}
	}

	protected override void OnDisposed()
	{
		m_AffectedObjects = null;
		m_MaterialPropertyBlock = null;
		base.OnDisposed();
	}
}
