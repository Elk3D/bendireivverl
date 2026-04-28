using DG.Tweening;
using UnityEngine;

public class HighlightDistance : JComponent
{
	private const string COLOR_VARIABLE = "_Color";

	private const float COLOR_LERP_TIME = 10f;

	[SerializeField]
	private float m_DistanceMin = 12f;

	[SerializeField]
	private float m_DistanceMax = 14f;

	private MeshRenderer m_MeshRenderer;

	private Sequence m_Sequence;

	private Color m_DefaultColor;

	private bool m_IsActive = true;

	private bool m_IsEntered;

	public override void Start()
	{
		if (m_MeshRenderer == null)
		{
			m_MeshRenderer = GetComponent<MeshRenderer>();
			m_DefaultColor = m_MeshRenderer.material.GetColor("_Color");
			Color black = Color.black;
			black.a = 0f;
			m_MeshRenderer.material.SetColor("_Color", black);
			m_IsActive = true;
		}
	}

	private void Update()
	{
		if (!base.IsDisposed && !GameManager.Instance.IsPaused && m_IsActive && !(GameManager.Instance.Player == null) && !m_IsEntered)
		{
			Vector3 position = GameManager.Instance.Player.transform.position;
			Color color = m_MeshRenderer.material.GetColor("_Color");
			Color color2 = color;
			Vector3 position2 = base.transform.position;
			position2.y = position.y;
			float num = Vector3.Distance(position, position2);
			if (num > m_DistanceMax)
			{
				color2 = Color.black;
			}
			else if (num < m_DistanceMin)
			{
				color2 = m_DefaultColor;
			}
			color2.a = 0f;
			if (color != color2)
			{
				color2 = Color.Lerp(color, color2, Time.deltaTime * 10f);
				m_MeshRenderer.material.SetColor("_Color", color2);
			}
		}
	}

	public void Deactivate()
	{
		if (m_MeshRenderer == null)
		{
			m_MeshRenderer = GetComponent<MeshRenderer>();
			m_DefaultColor = m_MeshRenderer.material.GetColor("_Color");
		}
		m_IsActive = false;
		m_MeshRenderer.material.SetColor("_Color", m_DefaultColor);
		Color black = Color.black;
		black.a = 0f;
		ResetSequence();
		m_Sequence.Insert(0f, m_MeshRenderer.material.DOColor(black, 1f).SetEase(Ease.OutSine));
	}

	public void Enter()
	{
		m_IsEntered = true;
	}

	public void Exit()
	{
		m_IsEntered = false;
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			m_Sequence.Kill();
			m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		KillSequence();
		m_MeshRenderer = null;
		base.OnDisposed();
	}
}
