using UnityEngine;

public class PhotoModeController : JMonoBehaviour
{
	[SerializeField]
	private Transform m_CameraPivot;

	[SerializeField]
	private Transform m_CameraContainer;

	[SerializeField]
	private Transform m_CameraLookAt;

	[Space]
	[SerializeField]
	private AudioSource m_AudioSourceMusic;

	[SerializeField]
	private GameObject m_Overlay;

	private float m_LookX;

	private float m_LookY;

	private Transform m_Freeroam;

	private bool m_IsActive;

	private bool m_HasCamera;

	private float m_Distance = 8f;

	public override void Start()
	{
		m_Overlay.SetActive(value: false);
	}

	public void Enable()
	{
		m_HasCamera = true;
	}

	public void Disable()
	{
		m_HasCamera = false;
	}

	private void Update()
	{
		if (!m_HasCamera)
		{
			return;
		}
		if (m_IsActive)
		{
			if (PlayerInput.Pause())
			{
				Deactivate();
				return;
			}
			m_LookX = PlayerInput.LookX();
			m_LookY = PlayerInput.LookY();
			m_Distance -= Input.mouseScrollDelta.y;
		}
		else if (Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.P))
		{
			Activate();
		}
	}

	private void LateUpdate()
	{
		UpdateCamera();
	}

	private void UpdateCamera()
	{
		if (m_HasCamera && m_HasCamera && !(m_Freeroam == null))
		{
			m_CameraPivot.eulerAngles += new Vector3(0f, m_LookX, 0f);
			Vector3 localPosition = m_CameraContainer.localPosition;
			if (m_Distance <= 3f)
			{
				m_Distance = 3f;
			}
			else if (m_Distance >= 30f)
			{
				m_Distance = 30f;
			}
			localPosition.z = m_Distance;
			m_CameraContainer.localPosition = localPosition;
			Vector3 vector = base.transform.position + Vector3.up * 5f;
			Vector3 direction = m_CameraContainer.position - vector;
			Vector3 b = m_CameraContainer.position;
			int layerMask = ~((1 << LayerMask.NameToLayer("InvisibleCollider")) | (1 << LayerMask.NameToLayer("AI")) | (1 << LayerMask.NameToLayer("Player")));
			if (Physics.SphereCast(vector, 1f, direction, out var hitInfo, m_Distance, layerMask, QueryTriggerInteraction.Ignore))
			{
				Vector3 vector2 = hitInfo.point - vector;
				b = hitInfo.point - vector2.normalized * 0.5f;
			}
			Vector3 b2 = m_CameraLookAt.localPosition + new Vector3(0f, m_LookY, 0f);
			if (b2.y <= 2f)
			{
				b2.y = 2f;
			}
			else if (b2.y >= 8f)
			{
				b2.y = 8f;
			}
			m_CameraLookAt.localPosition = Vector3.Lerp(m_CameraLookAt.localPosition, b2, Time.unscaledDeltaTime * 5f);
			b.y = m_CameraContainer.position.y;
			m_Freeroam.position = Vector3.Lerp(m_Freeroam.position, b, Time.unscaledDeltaTime * 10f);
			Quaternion b3 = Quaternion.LookRotation(m_CameraLookAt.position - m_Freeroam.position);
			m_Freeroam.rotation = Quaternion.Slerp(m_Freeroam.rotation, b3, Time.unscaledDeltaTime * 15f);
		}
	}

	private void Activate()
	{
		m_IsActive = true;
		m_Overlay.SetActive(value: true);
		GameManager.Instance.Player.SetAllTrackers(active: false);
		GameManager.Instance.Player.ModelLayers.EnableAll();
		GameManager.Instance.Pause();
		if (m_CameraPivot == null)
		{
			m_CameraPivot = new GameObject().transform;
			m_CameraPivot.position = GameManager.Instance.Player.transform.position + Vector3.up * 5f;
		}
		m_Distance = 8f;
		m_Freeroam = GameManager.Instance.GameCamera.InitializeFreeRoamCam(m_CameraPivot);
		m_Freeroam.localPosition = m_CameraContainer.localPosition;
		m_Freeroam.localEulerAngles = m_CameraContainer.localEulerAngles;
		m_AudioSourceMusic.Play();
		UIManager.SetCursor(active: false);
	}

	private void Deactivate()
	{
		m_IsActive = false;
		m_Overlay.SetActive(value: false);
		GameManager.Instance.Unpause();
		GameManager.Instance.GameCamera.ExitFreeRoamCam();
		m_Freeroam = null;
		GameManager.Instance.Player.ResetRotation();
		GameManager.Instance.Player.SetHeadTracker(active: true);
		GameManager.Instance.Player.ModelLayers.EnableFirstPerson();
		GameManager.Instance.Player.ShowFirstPersonArms();
		m_AudioSourceMusic.Stop();
		UIManager.SetCursor(active: false);
	}
}
