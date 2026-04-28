using UnityEngine;

public class Moon : JMonoBehaviour
{
	[SerializeField]
	private float m_Distance = 20f;

	[SerializeField]
	private Renderer m_VisibleCheck;

	[SerializeField]
	private ActiveSetter m_ActiveSetter;

	private Vector3 m_OriginPosition;

	private bool m_IsActive;

	private bool m_IsActivated;

	public override void Start()
	{
		m_OriginPosition = base.transform.position;
	}

	private void Update()
	{
		Camera camera = GameManager.Instance.GameCamera.Camera;
		Vector3 vector = m_OriginPosition - camera.transform.position;
		Vector3 position = camera.transform.position + vector.normalized * (camera.farClipPlane - m_Distance);
		base.transform.position = position;
		if (!m_IsActive)
		{
			return;
		}
		if (m_VisibleCheck.isVisible)
		{
			if (!m_IsActivated)
			{
				m_IsActivated = true;
			}
		}
		else if (m_IsActivated)
		{
			SetActive(active: false);
			m_IsActivated = false;
		}
	}

	public void SetActive(bool active)
	{
		m_IsActive = active;
		if (m_ActiveSetter != null)
		{
			m_ActiveSetter.SetActive(active);
		}
	}
}
