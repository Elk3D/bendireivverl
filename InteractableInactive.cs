using UnityEngine;

public class InteractableInactive : Interactable
{
	[Header("Initialize Requirements")]
	[SerializeField]
	private bool m_UseInitialize;

	[SerializeField]
	private bool m_IgnoreCombatState;

	[Header("Visual Options")]
	[SerializeField]
	protected bool m_IsReal;

	[Header("Input Options")]
	[SerializeField]
	private InteractionType m_InteractionType;

	[Header("Connected Interactable")]
	[SerializeField]
	private Interactable m_Connected;

	private Collider m_InactiveCollider;

	private Collider m_ActiveCollider;

	private bool m_IsInitialized;

	public override void Awake()
	{
		m_InactiveCollider = base.gameObject.GetComponent<Collider>();
		m_ActiveCollider = m_Connected.GetComponent<Collider>();
	}

	public void InitializeInactive()
	{
		m_IsInitialized = true;
	}

	public void DisableInitialize()
	{
		m_IsInitialized = false;
	}

	private void Update()
	{
		if (m_Connected == null || m_InactiveCollider == null || m_ActiveCollider == null || base.IsDisposed || GameManager.Instance.IsPaused || (m_UseInitialize && !m_IsInitialized))
		{
			return;
		}
		if (m_Connected.IsActive)
		{
			if (base.IsActive)
			{
				SetActive(active: false);
				m_InactiveCollider.enabled = false;
			}
			m_ActiveCollider.enabled = true;
		}
		else
		{
			if (!base.IsActive)
			{
				SetActive(active: true);
				m_InactiveCollider.enabled = true;
			}
			m_ActiveCollider.enabled = false;
		}
	}

	protected override void OnInternalEnter(Vector3 origin, RaycastHit hit, object sender = null)
	{
		string key = "";
		if (m_InteractionType != InteractionType.NOT_SET_UP)
		{
			key = m_InteractionType.ToString();
		}
		if (!m_IgnoreCombatState && GameManager.Instance.Player.CombatStatus == CombatStatus.Combat)
		{
			key = InteractionType.INTERACTION_INVALID_COMBAT.ToString();
		}
		string text = TextUtility.GetKey(key);
		if (m_IsReal)
		{
			text = text.ToUpper();
		}
		GameManager.Instance.ShowInteractInvalid(text, m_IsReal);
	}

	protected override void OnInternalExit(Vector3 origin, RaycastHit hit, object sender = null)
	{
		GameManager.Instance.HideInteractInvalid();
	}

	protected override bool InternalInteractCheck(Vector3 origin, RaycastHit hit, object sender = null)
	{
		return false;
	}

	protected override void OnDisposed()
	{
		m_InactiveCollider = null;
		m_ActiveCollider = null;
		base.OnDisposed();
	}
}
