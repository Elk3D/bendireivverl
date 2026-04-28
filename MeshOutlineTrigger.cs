using System;
using S13Audio.BATDR;
using UnityEngine;

public class MeshOutlineTrigger : JComponent
{
	[Serializable]
	public class MeshOutlineGroup
	{
		public MeshRenderer MeshRenderer;

		public SkinnedMeshRenderer SkinnedMeshRenderer;

		public Material DefaultMaterial;

		public Material OutlineMaterial;

		public int MaterialIndex = -1;
	}

	[SerializeField]
	private bool m_IsActive = true;

	[SerializeField]
	private EventTrigger m_EventTrigger;

	[Header("Mesh Group")]
	[SerializeField]
	private MeshOutlineGroup[] m_MeshOutlineGroup;

	public MeshOutlineGroup[] MeshOutlineGroups => m_MeshOutlineGroup;

	public override void Start()
	{
		if (m_IsActive)
		{
			Enable();
		}
	}

	private void Update()
	{
		if (!base.IsDisposed && !GameManager.Instance.IsPaused && !m_EventTrigger.IsActive)
		{
			Exit();
		}
	}

	private void HandleInteractableOnEnter(object sender, EventArgs e)
	{
		if (m_EventTrigger.IsActive)
		{
			Enter();
		}
	}

	public void Enter()
	{
		for (int i = 0; i < m_MeshOutlineGroup.Length; i++)
		{
			MeshOutlineGroup meshOutlineGroup = m_MeshOutlineGroup[i];
			if (meshOutlineGroup.MeshRenderer != null)
			{
				SetMaterial(meshOutlineGroup.MeshRenderer, meshOutlineGroup.OutlineMaterial, isEnter: true, meshOutlineGroup.MaterialIndex);
			}
			else if (meshOutlineGroup.SkinnedMeshRenderer != null)
			{
				SetMaterial(meshOutlineGroup.SkinnedMeshRenderer, meshOutlineGroup.OutlineMaterial, isEnter: true, meshOutlineGroup.MaterialIndex);
			}
		}
		BATDRUIAudioManager.UIHighlightBegin();
	}

	private void HandleInteractableOnExit(object sender, EventArgs e)
	{
		if (m_EventTrigger.IsActive)
		{
			Exit();
		}
	}

	public void Exit()
	{
		for (int i = 0; i < m_MeshOutlineGroup.Length; i++)
		{
			MeshOutlineGroup meshOutlineGroup = m_MeshOutlineGroup[i];
			if (meshOutlineGroup.MeshRenderer != null)
			{
				SetMaterial(meshOutlineGroup.MeshRenderer, meshOutlineGroup.DefaultMaterial, isEnter: false, meshOutlineGroup.MaterialIndex);
			}
			else if (meshOutlineGroup.SkinnedMeshRenderer != null)
			{
				SetMaterial(meshOutlineGroup.SkinnedMeshRenderer, meshOutlineGroup.DefaultMaterial, isEnter: false, meshOutlineGroup.MaterialIndex);
			}
		}
		BATDRUIAudioManager.UIHighlightEnd();
	}

	private void SetMaterial(Renderer renderer, Material material, bool isEnter, int materialIndex)
	{
		HighlightDistance highlightDistance = null;
		if (renderer != null)
		{
			Material[] sharedMaterials = renderer.sharedMaterials;
			if (materialIndex == -1)
			{
				for (int i = 0; i < sharedMaterials.Length; i++)
				{
					sharedMaterials[i] = material;
				}
			}
			else
			{
				sharedMaterials[materialIndex] = material;
			}
			renderer.sharedMaterials = sharedMaterials;
			highlightDistance = renderer.gameObject.GetComponent<HighlightDistance>();
		}
		if (highlightDistance != null)
		{
			if (isEnter)
			{
				highlightDistance.Enter();
			}
			else
			{
				highlightDistance.Exit();
			}
		}
	}

	private void SetMaterial(Renderer renderer, Material material, int materialIndex)
	{
		if (!(renderer != null))
		{
			return;
		}
		Material[] sharedMaterials = renderer.sharedMaterials;
		if (materialIndex == -1)
		{
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				sharedMaterials[i] = material;
			}
		}
		else
		{
			sharedMaterials[materialIndex] = material;
		}
		renderer.sharedMaterials = sharedMaterials;
	}

	public void SetActive(bool active)
	{
		m_IsActive = active;
	}

	public void Enable()
	{
		AddListeners();
		for (int i = 0; i < m_MeshOutlineGroup.Length; i++)
		{
			MeshOutlineGroup meshOutlineGroup = m_MeshOutlineGroup[i];
			if (meshOutlineGroup.MeshRenderer != null)
			{
				SetMaterial(meshOutlineGroup.MeshRenderer, meshOutlineGroup.DefaultMaterial, meshOutlineGroup.MaterialIndex);
			}
			else if (meshOutlineGroup.SkinnedMeshRenderer != null)
			{
				SetMaterial(meshOutlineGroup.SkinnedMeshRenderer, meshOutlineGroup.DefaultMaterial, meshOutlineGroup.MaterialIndex);
			}
		}
		SetActive(active: true);
	}

	public void Disable()
	{
		SetActive(active: false);
		RemoveListeners();
		for (int i = 0; i < m_MeshOutlineGroup.Length; i++)
		{
			MeshOutlineGroup meshOutlineGroup = m_MeshOutlineGroup[i];
			Material defaultMaterial = meshOutlineGroup.DefaultMaterial;
			if (meshOutlineGroup.MeshRenderer != null)
			{
				SetMaterial(meshOutlineGroup.MeshRenderer, defaultMaterial, isEnter: true, meshOutlineGroup.MaterialIndex);
			}
			else if (meshOutlineGroup.SkinnedMeshRenderer != null)
			{
				SetMaterial(meshOutlineGroup.SkinnedMeshRenderer, defaultMaterial, isEnter: true, meshOutlineGroup.MaterialIndex);
			}
		}
	}

	private void AddListeners()
	{
		RemoveListeners();
		if ((bool)m_EventTrigger)
		{
			m_EventTrigger.OnEnter += HandleInteractableOnEnter;
			m_EventTrigger.OnExit += HandleInteractableOnExit;
		}
	}

	private void RemoveListeners()
	{
		if ((bool)m_EventTrigger)
		{
			m_EventTrigger.OnEnter -= HandleInteractableOnEnter;
			m_EventTrigger.OnExit -= HandleInteractableOnExit;
		}
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		base.OnDisposed();
	}
}
