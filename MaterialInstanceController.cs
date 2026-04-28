using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MaterialInstanceController : JMonoBehaviour
{
	[SerializeField]
	public enum PropertyContainerType
	{
		TextureProp,
		FloatProp,
		ColorProp,
		BoolProp,
		SliderProp
	}

	[SerializeField]
	private string m_RefrenceShaderName;

	[SerializeField]
	private List<int> m_MaterialHashIDs = new List<int>();

	[SerializeField]
	private List<MaterialInstancePropertyContainer> m_MaterialChangedProperties = new List<MaterialInstancePropertyContainer>();

	private MaterialPropertyBlock m_MaterialProperties;

	private Renderer m_Renderer;

	public Renderer Renderer
	{
		get
		{
			if (m_Renderer == null)
			{
				m_Renderer = GetComponent<Renderer>();
			}
			return m_Renderer;
		}
	}

	[ExecuteInEditMode]
	public override void OnEnable()
	{
		base.OnEnable();
		ApplyMaterialBlock();
	}

	public string GetShaderType()
	{
		return Renderer.sharedMaterial.shader.name;
	}

	private string SetupContainers(string _checkedShader)
	{
		if (m_RefrenceShaderName == _checkedShader)
		{
			return _checkedShader;
		}
		return m_RefrenceShaderName = _checkedShader;
	}

	public bool CheckPropertyBlockStatus(string _Identifier = "")
	{
		int hashCode = _Identifier.GetHashCode();
		return m_MaterialHashIDs.Contains(hashCode);
	}

	public MaterialInstancePropertyContainer GetPropertyBlock(string _Identifier)
	{
		int hashCode = _Identifier.GetHashCode();
		return m_MaterialChangedProperties[m_MaterialHashIDs.IndexOf(hashCode)];
	}

	public void AddPropertyBlock(string _Identifier, PropertyContainerType _propertyType)
	{
		int hashCode = _Identifier.GetHashCode();
		if (!m_MaterialHashIDs.Contains(hashCode))
		{
			if (!m_MaterialChangedProperties.Contains(new MaterialInstancePropertyContainer(_propertyType, _Identifier)))
			{
				m_MaterialChangedProperties.Add(new MaterialInstancePropertyContainer(_propertyType, _Identifier));
			}
			m_MaterialHashIDs.Add(hashCode);
		}
	}

	public void RemovePropertyBlock(string _Identifier)
	{
		int hashCode = _Identifier.GetHashCode();
		if (m_MaterialHashIDs.Contains(hashCode))
		{
			m_MaterialChangedProperties.RemoveAt(m_MaterialHashIDs.IndexOf(hashCode));
			m_MaterialHashIDs.Remove(hashCode);
		}
	}

	public void ApplyMaterialBlock()
	{
		m_MaterialProperties = new MaterialPropertyBlock();
		for (int i = 0; i < m_MaterialChangedProperties.Count; i++)
		{
			MaterialInstancePropertyContainer materialInstancePropertyContainer = m_MaterialChangedProperties[i];
			switch (materialInstancePropertyContainer.PropertyType)
			{
			case PropertyContainerType.TextureProp:
				if (materialInstancePropertyContainer.NewTexture != null)
				{
					m_MaterialProperties.SetTexture(materialInstancePropertyContainer.Name, materialInstancePropertyContainer.NewTexture);
				}
				break;
			case PropertyContainerType.FloatProp:
			case PropertyContainerType.BoolProp:
			case PropertyContainerType.SliderProp:
				m_MaterialProperties.SetFloat(materialInstancePropertyContainer.Name, materialInstancePropertyContainer.NewFloat);
				break;
			case PropertyContainerType.ColorProp:
				m_MaterialProperties.SetColor(materialInstancePropertyContainer.Name, materialInstancePropertyContainer.NewColor);
				break;
			}
		}
		Renderer.SetPropertyBlock(m_MaterialProperties);
	}

	public void ClearMaterialBlock()
	{
		Renderer.SetPropertyBlock(null);
	}
}
