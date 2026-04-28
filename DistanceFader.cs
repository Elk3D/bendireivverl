using UnityEngine;

public class DistanceFader : JComponent
{
	[Header("Renderer")]
	[SerializeField]
	private MeshRenderer m_MeshRenderer;

	[Header("Distance Check")]
	[SerializeField]
	private float m_DistanceMin = 25f;

	[SerializeField]
	private float m_DistanceMax = 30f;

	[Header("Fade Settings")]
	[SerializeField]
	private float m_FadeDuration = 8f;

	[SerializeField]
	private float m_FadeOut;

	[SerializeField]
	private float m_FadeIn = 1f;

	[Header("Other Settings")]
	[SerializeField]
	private bool m_IsSingleFade = true;

	private float m_ColorLerpTime;

	private bool m_IsFadingIn;

	private bool m_IsFadingOut;

	private bool m_IsFading;

	private bool m_IsFaded;

	private void Update()
	{
		if (m_MeshRenderer == null || base.IsDisposed || GameManager.Instance.IsPaused || GameManager.Instance.GameCamera == null || (m_IsSingleFade && m_IsFaded))
		{
			return;
		}
		Vector3 position = GameManager.Instance.GameCamera.transform.position;
		Color color = m_MeshRenderer.material.GetColor("_Color");
		Color color2 = color;
		Vector3 position2 = base.transform.position;
		position2.y = position.y;
		float num = Vector3.Distance(position, position2);
		if (!m_IsSingleFade)
		{
			if (num > m_DistanceMax)
			{
				if (m_IsFadingOut)
				{
					m_IsFadingOut = false;
					m_ColorLerpTime = 0f;
				}
				m_IsFadingIn = true;
				color2.a = m_FadeIn;
			}
			else if (num < m_DistanceMin)
			{
				if (m_IsFadingIn)
				{
					m_IsFadingIn = false;
					m_ColorLerpTime = 0f;
				}
				m_IsFadingOut = true;
				color2.a = m_FadeOut;
			}
		}
		else
		{
			if (num < m_DistanceMin)
			{
				m_IsFading = true;
			}
			if (m_IsFading)
			{
				m_IsFadingOut = true;
				color2.a = m_FadeOut;
			}
		}
		if (color != color2)
		{
			color2 = Color.Lerp(color, color2, m_ColorLerpTime);
			m_MeshRenderer.material.SetColor("_Color", color2);
			if (m_ColorLerpTime < 1f)
			{
				m_ColorLerpTime += Time.deltaTime / m_FadeDuration;
			}
		}
		else if (m_IsFading)
		{
			m_IsFaded = true;
		}
	}
}
