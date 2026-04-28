using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CharacterControllable : Character
{
	[Header("Controllable")]
	[SerializeField]
	private Transform m_CameraPivot;

	[SerializeField]
	private Transform m_CameraContainer;

	[SerializeField]
	private Transform m_CameraLookAt;

	[SerializeField]
	private Transform m_GroundLocation;

	[SerializeField]
	private ParticleSystem m_GroundParticles;

	private Sequence m_SlideSequence;

	private Transform m_Freeroam;

	private float m_HitType;

	private float m_LastHitType;

	private float[] m_HitTypes = new float[2] { 0f, 1f };

	private float m_MoveY;

	private float m_LookRotation;

	private bool m_TestScale;

	public override bool JumpInput => false;

	public override bool CrouchInput => false;

	public override bool RunInput => PlayerInput.Run();

	public override float MoveXInput => 0f;

	public override float MoveYInput => PlayerInput.MoveY();

	public override float RotateXInput => PlayerInput.LookX();

	public override bool AttackInput => PlayerInput.Attack();

	protected override void InternalInitialize()
	{
		GameManager.Instance.BeastBendy = this;
		StartCoroutine(InitializeDelay());
	}

	private IEnumerator InitializeDelay()
	{
		yield return new WaitForSeconds(0.5f);
		m_CameraPivot.SetParent(null);
		m_CameraPivot.position = base.transform.position;
		m_CameraPivot.rotation = base.transform.rotation;
		Enable();
	}

	protected override void InternalUpdate()
	{
		if (!(m_Freeroam == null))
		{
			m_LookRotation = RotateXInput;
			m_MoveY = MoveYInput;
		}
	}

	protected override void InternalFixedUpdate()
	{
		if (m_Freeroam == null || GetInputs())
		{
			return;
		}
		m_CameraPivot.position = Vector3.Lerp(m_CameraPivot.position, base.transform.position, Time.deltaTime * 5f);
		Vector3 eulerAngles = m_CameraPivot.eulerAngles;
		Vector3 normalized = (base.transform.position - m_CameraContainer.position).normalized;
		normalized.y *= 0f;
		float num = Vector3.Angle(base.transform.forward, normalized);
		if (base.CurrentState == State.Character.Controllable)
		{
			if (m_MoveY > 0f)
			{
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, m_CameraPivot.rotation, Time.deltaTime * 5f);
			}
			else if (m_MoveY < 0f)
			{
				if (num > 120f)
				{
					eulerAngles.y += 180f;
				}
				Quaternion b = Quaternion.Euler(eulerAngles);
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, Time.deltaTime * 5f);
			}
		}
		m_CameraPivot.eulerAngles += new Vector3(0f, m_LookRotation, 0f);
		Vector3 vector = base.transform.position + Vector3.up * 5f;
		Vector3 direction = m_CameraContainer.position - vector;
		Vector3 b2 = m_CameraContainer.position;
		int layerMask = ~((1 << LayerMask.NameToLayer("InvisibleCollider")) | (1 << LayerMask.NameToLayer("AI")));
		if (Physics.SphereCast(vector, 4f, direction, out var hitInfo, 15f, layerMask, QueryTriggerInteraction.Ignore))
		{
			Vector3 vector2 = hitInfo.point - vector;
			b2 = hitInfo.point - vector2.normalized * 2f;
		}
		b2.y = m_CameraContainer.position.y;
		m_Freeroam.position = Vector3.Lerp(m_Freeroam.position, b2, Time.deltaTime * 5f);
		Quaternion b3 = Quaternion.LookRotation(m_CameraLookAt.position - m_Freeroam.position);
		m_Freeroam.rotation = Quaternion.Slerp(m_Freeroam.rotation, b3, Time.deltaTime * 8f);
	}

	private bool GetInputs()
	{
		bool result = false;
		if (base.CurrentState == State.Character.Controllable)
		{
			if (PlayerInput.Attack())
			{
				bool num = RunInput && MoveYInput > 0f;
				base.Content.Animator.SetMovementState(0f, smooth: false);
				base.Content.Animator.SetMovementSpeed(0f, smooth: false);
				base.Agent.CancelPath();
				SetState(State.Character.Attack);
				SetPreviousState(State.Character.Controllable);
				if (num)
				{
					if (m_SlideSequence != null)
					{
						m_SlideSequence.Kill();
					}
					m_SlideSequence = DOTween.Sequence();
					m_SlideSequence.SetUpdate(UpdateType.Fixed);
					m_SlideSequence.InsertCallback(0f, delegate
					{
						m_GroundParticles.Emit(2);
					});
					m_SlideSequence.InsertCallback(0.15f, delegate
					{
						m_GroundParticles.Emit(2);
					});
					m_SlideSequence.InsertCallback(0.25f, delegate
					{
						m_GroundParticles.Emit(2);
					});
					m_SlideSequence.InsertCallback(0.35f, delegate
					{
						m_GroundParticles.Emit(2);
					});
					m_SlideSequence.InsertCallback(0.45f, delegate
					{
						m_GroundParticles.Emit(2);
					});
					m_SlideSequence.OnUpdate(delegate
					{
						base.Controller.Move(base.transform.forward * 0.5f);
					});
				}
				result = true;
			}
			else if (PlayerInput.AttackSecondary())
			{
				base.Content.Animator.SetMovementState(0f, smooth: false);
				base.Content.Animator.SetMovementSpeed(0f, smooth: false);
				base.Agent.CancelPath();
				SetState(State.Character.Cutscene);
				SetPreviousState(State.Character.Controllable);
				base.Content.Animator.SetTrigger("AttackBehind");
				result = true;
			}
			else if (PlayerInput.Jump())
			{
				base.Content.Animator.SetMovementState(0f, smooth: false);
				base.Content.Animator.SetMovementSpeed(0f, smooth: false);
				base.Agent.CancelPath();
				SetState(State.Character.Cutscene);
				SetNextState(State.Character.Controllable);
				base.Content.Animator.SetTrigger("Alert");
				result = true;
			}
		}
		return result;
	}

	public void Enable()
	{
		base.Agent.Agent.enabled = false;
		m_Freeroam = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		SetState(State.Character.Controllable);
		base.Rotation.ResetRotation();
		SetActive(active: true);
	}

	public void Disable()
	{
		SetActive(active: false);
		SetState(State.Character.Cutscene);
	}

	public override void OnAttack()
	{
		RaycastHit hit = new RaycastHit
		{
			point = base.transform.position + base.transform.forward * 5f
		};
		Collider[] array = Physics.OverlapSphere(hit.point, 10f);
		array.Shuffle();
		int num = 0;
		Collider[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			IHittable componentInParent = array2[i].GetComponentInParent<IHittable>();
			if (componentInParent == null || componentInParent.IsBroken)
			{
				continue;
			}
			if (componentInParent is Dummy)
			{
				if (num < 5)
				{
					num++;
					componentInParent.Hit(hit);
				}
			}
			else
			{
				componentInParent.Hit(hit);
			}
		}
		CameraEffects.ShakeRotation(0.5f, 0.35f);
	}

	public void OnAttacks(int amount)
	{
		string assetKey = ((Random.value < 0.5f) ? "GroundCrack/Decal_Ground_Crack_01" : "GroundCrack/Decal_Ground_Crack_02");
		Transform obj = GameManager.Instance.AssetManager.CreateAsset<Transform>(assetKey);
		obj.position = m_GroundLocation.position;
		obj.eulerAngles = new Vector3(m_GroundLocation.eulerAngles.x, Random.Range(0f, 360f), 0f);
		obj.localScale = Vector3.one * ((float)amount * 1.5f);
		Object.Destroy(obj.gameObject, 5f);
		RaycastHit hit = new RaycastHit
		{
			point = m_GroundLocation.position
		};
		Collider[] array = Physics.OverlapSphere(m_GroundLocation.position, amount * 4);
		array.Shuffle();
		int num = 0;
		Collider[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			IHittable componentInParent = array2[i].GetComponentInParent<IHittable>();
			if (componentInParent == null || componentInParent.IsBroken)
			{
				continue;
			}
			if (componentInParent is Dummy)
			{
				if (num < amount)
				{
					num++;
					componentInParent.Hit(hit);
				}
			}
			else
			{
				componentInParent.Hit(hit);
			}
		}
		CameraEffects.ShakeRotation(0.5f, amount);
		m_GroundParticles.Emit(5);
	}

	public override void OnHit(RaycastHit hit)
	{
		if (base.m_State.ID != State.Character.Hit)
		{
			base.Content.Animator.SetMovementState(0f, smooth: false);
			base.Content.Animator.SetMovementSpeed(0f, smooth: false);
			base.Agent.CancelPath();
			SetState(State.Character.Hit);
		}
		base.Content.Animator.Hit(m_HitType);
		while (m_HitType == m_LastHitType)
		{
			m_HitType = m_HitTypes[Random.Range(0, m_HitTypes.Length)];
		}
		m_LastHitType = m_HitType;
	}
}
