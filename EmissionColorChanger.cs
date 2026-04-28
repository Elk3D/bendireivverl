using DG.Tweening;
using UnityEngine;

public class EmissionColorChanger : JMonoBehaviour
{
	[SerializeField]
	private SkinnedMeshRenderer m_MeshRenderer;

	[SerializeField]
	private float m_HueMin;

	[SerializeField]
	private float m_HueMax;

	[SerializeField]
	private float m_SaturationMin;

	[SerializeField]
	private float m_SaturationMax;

	[SerializeField]
	private float m_ValueMin;

	[SerializeField]
	private float m_ValueMax;

	[SerializeField]
	private float m_AlphaMin;

	[SerializeField]
	private float m_AlphaMax;

	public override void Start()
	{
		DOColor();
	}

	private void DOColor()
	{
		if (m_MeshRenderer != null && m_MeshRenderer.gameObject != null)
		{
			Color32 color = Random.ColorHSV(m_HueMin, m_HueMax, m_SaturationMin, m_SaturationMax, m_ValueMin, m_ValueMax, m_AlphaMin, m_AlphaMax);
			Sequence sequence = DOTween.Sequence();
			for (int i = 0; i < m_MeshRenderer.materials.Length; i++)
			{
				sequence.Insert(0f, m_MeshRenderer.materials[i].DOColor(color, "_EmissionColor", 0.1f).SetEase(Ease.Linear));
			}
			sequence.OnComplete(DOColor);
		}
	}
}
