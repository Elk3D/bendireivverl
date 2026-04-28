using System;
using DG.Tweening;
using InControl;
using UnityEngine;

public class InsaneController : JMonoBehaviour
{
	private const float TORQUE = 30f;

	[Header("Section")]
	[SerializeField]
	private SectionID m_SectionID;

	[Header("Options")]
	[SerializeField]
	private Animator m_Animator;

	[Header("Left Leg")]
	[SerializeField]
	private Rigidbody m_LeftLeg;

	[SerializeField]
	private Rigidbody m_LeftShin;

	[SerializeField]
	private Rigidbody m_LeftThigh;

	[Header("Right Leg")]
	[SerializeField]
	private Rigidbody m_RightLeg;

	[SerializeField]
	private Rigidbody m_RightShin;

	[SerializeField]
	private Rigidbody m_RightThigh;

	[Header("Other")]
	[SerializeField]
	private Rigidbody m_Neck;

	[SerializeField]
	private Rigidbody m_Head;

	[SerializeField]
	private Rigidbody m_Pelvis;

	private float m_StartTimerMax = 4f;

	private float m_StartTimer;

	private float m_TimerMax = 0.75f;

	private float m_LegTimerL;

	private float m_LegTimerR;

	private float m_HeadTimer;

	private float m_PelvisTimer;

	private float m_LookTimer;

	private bool m_IsActive;

	private Rigidbody[] m_RagdollColliders;

	private InsaneFoot[] m_Feet;

	public event EventHandler OnPlay;

	public event EventHandler OnExit;

	public override void Start()
	{
		GameManager.Instance.LockPause();
		UIManager.SetCursor(active: false);
		GameManager.Instance.ClearUI();
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.Dispose();
		}
		m_Feet = GetComponentsInChildren<InsaneFoot>();
		for (int i = 0; i < m_Feet.Length; i++)
		{
			m_Feet[i].OnInteract += HandleFootOnInteract;
		}
		m_RagdollColliders = GetComponentsInChildren<Rigidbody>();
		for (int j = 0; j < m_RagdollColliders.Length; j++)
		{
			m_RagdollColliders[j].isKinematic = true;
		}
		m_LegTimerL = 0f;
		m_LegTimerR = 0f;
		m_HeadTimer = 0f;
		m_PelvisTimer = 0f;
		m_LookTimer = 0f;
		GameManager.Instance.HideScreenBlocker(3f, 1f);
		this.OnPlay.Send(this);
	}

	private void Update()
	{
		if (m_StartTimer >= m_StartTimerMax)
		{
			m_LegTimerL += Time.deltaTime;
			m_LegTimerR += Time.deltaTime;
			m_HeadTimer += Time.deltaTime;
			m_PelvisTimer += Time.deltaTime;
			m_LookTimer += Time.deltaTime;
		}
		else
		{
			m_StartTimer += Time.deltaTime;
		}
	}

	private void FixedUpdate()
	{
		if (m_StartTimer < m_StartTimerMax)
		{
			return;
		}
		float num = PlayerInput.LookX();
		float num2 = PlayerInput.LookY();
		float num3 = 0f;
		float num4 = 0f;
		if (!GameManager.Instance.HasController)
		{
			num3 = PlayerInput.MoveXRaw();
			num4 = PlayerInput.MoveYRaw();
		}
		else
		{
			if (GamepadInput.GetButtonDown(InputControlType.LeftTrigger))
			{
				num3 = -1f;
			}
			if (GamepadInput.GetButtonDown(InputControlType.RightTrigger))
			{
				num3 = 1f;
			}
			if (GamepadInput.GetButtonDown(InputControlType.LeftBumper))
			{
				num4 = 1f;
			}
			if (GamepadInput.GetButtonDown(InputControlType.RightBumper))
			{
				num4 = -1f;
			}
		}
		if (!m_IsActive)
		{
			if (num3 == 0f && num4 == 0f && num == 0f && num2 == 0f)
			{
				return;
			}
			for (int i = 0; i < m_RagdollColliders.Length; i++)
			{
				if (m_RagdollColliders[i].isKinematic)
				{
					m_RagdollColliders[i].isKinematic = false;
				}
			}
			m_IsActive = true;
			m_LegTimerL = 0f;
			m_LegTimerR = 0f;
			m_HeadTimer = 0f;
			m_PelvisTimer = 0f;
			m_LookTimer = 0f;
			DOTween.Sequence().InsertCallback(10f, delegate
			{
				UICredits uICredits = GameManager.Instance.UIManager.Show<UICredits>("UI/Views/UICredits", "VIEW", true);
				uICredits.OnPlayOutComplete -= HandleCreditsOnPlayOutComplete;
				uICredits.OnPlayOutComplete += HandleCreditsOnPlayOutComplete;
			});
			return;
		}
		m_Animator.enabled = false;
		if (m_LookTimer >= m_TimerMax)
		{
			float num5 = num * 30f;
			float num6 = (0f - num2) * 30f;
			m_Neck.AddRelativeTorque(num5, 0f, num6, ForceMode.Impulse);
			m_Head.AddRelativeTorque(num5 * 2f, 0f, num6 * 2f, ForceMode.Acceleration);
		}
		if (num3 > 0f && m_LegTimerL >= m_TimerMax)
		{
			m_LeftShin.AddForce(Vector3.up * 30f, ForceMode.Impulse);
			m_LeftLeg.AddForce(Vector3.up * 30f / 4f, ForceMode.Impulse);
			m_LegTimerL = 0f;
		}
		else if (num3 < 0f && m_LegTimerR >= m_TimerMax)
		{
			m_RightShin.AddForce(Vector3.up * 30f, ForceMode.Impulse);
			m_RightLeg.AddForce(Vector3.up * 30f / 4f, ForceMode.Impulse);
			m_LegTimerR = 0f;
		}
		if (num4 > 0f && m_HeadTimer >= m_TimerMax)
		{
			m_Head.AddForce(Vector3.up * 30f, ForceMode.Impulse);
			m_HeadTimer = 0f;
		}
		else if (num4 < 0f && m_PelvisTimer >= m_TimerMax)
		{
			m_Pelvis.AddForce(Vector3.up * 30f * 3f, ForceMode.Impulse);
			m_PelvisTimer = 0f;
		}
	}

	private void HandleFootOnInteract(object sender, EventArgs e)
	{
		Vector3 vector = (sender as InsaneFoot).ToPosition - m_Pelvis.transform.position;
		m_Pelvis.AddForce(vector.normalized * 30f * 2f, ForceMode.Impulse);
	}

	private void HandleCreditsOnPlayOutComplete(object sender, EventArgs e)
	{
		((UICredits)sender).OnPlayOutComplete -= HandleCreditsOnPlayOutComplete;
		this.OnExit.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnPlay = null;
		this.OnExit = null;
		m_RagdollColliders = null;
		m_Feet = null;
		base.OnDisposed();
	}
}
