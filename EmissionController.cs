using DG.Tweening;
using UnityEngine;

public class EmissionController : JMonoBehaviour
{
	[SerializeField]
	private SkinnedMeshRenderer m_MaterialEmission;

	[SerializeField]
	private Texture2D m_Sigil;

	[SerializeField]
	private string m_EmissionVariable = "_EmitPower";

	[SerializeField]
	private float m_EndValue;

	[SerializeField]
	private float m_Duration;

	public void SetEmission()
	{
		if (!(m_MaterialEmission == null))
		{
			Texture2D texture2D = m_Sigil;
			if (texture2D == null)
			{
				texture2D = GameManager.Instance.AssetManager.GetAsset<Texture2D>("Audrey_Sigil_Banish");
			}
			m_MaterialEmission.sharedMaterial.SetTexture("_Emission", texture2D);
			m_MaterialEmission.sharedMaterial.SetFloat(m_EmissionVariable, m_EndValue);
		}
	}

	public void TweenEmission()
	{
		if (!(m_MaterialEmission == null))
		{
			Texture2D texture2D = m_Sigil;
			if (texture2D == null)
			{
				texture2D = GameManager.Instance.AssetManager.GetAsset<Texture2D>("Audrey_Sigil_Banish");
			}
			m_MaterialEmission.sharedMaterial.SetTexture("_Emission", texture2D);
			m_MaterialEmission.sharedMaterial.DOKill();
			m_MaterialEmission.sharedMaterial.DOFloat(m_EndValue, m_EmissionVariable, m_Duration).SetEase(Ease.InOutSine);
		}
	}

	protected override void OnDisposed()
	{
		if (m_MaterialEmission != null)
		{
			m_MaterialEmission.sharedMaterial.DOKill();
		}
		base.OnDisposed();
	}
}
