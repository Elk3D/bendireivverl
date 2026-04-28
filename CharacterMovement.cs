using System;
using UnityEngine;

[Serializable]
public class CharacterMovement : JDisposable
{
	[SerializeField]
	private float m_MoveSpeed = 1f;

	[SerializeField]
	private float m_RunSpeed = 2f;

	[SerializeField]
	private float m_LungeSpeed = 3f;

	[SerializeField]
	private float m_JumpSpeed = 4f;

	private readonly float m_Gravity = 0.04f;

	private float m_ActiveGravity;

	private Vector3 m_ExternalForce = Vector3.zero;

	private Vector3 m_MoveDirection = Vector3.zero;

	public float MoveSpeed => m_MoveSpeed;

	public float RunSpeed => m_RunSpeed;

	public float LungeSpeed => m_LungeSpeed;

	public Character Character { get; private set; }

	public Vector3 MoveDirection => m_MoveDirection;

	public Vector2 MovementInput { get; private set; }

	public bool PreviouslyGrounded { get; private set; }

	public bool CrouchInput { get; private set; }

	public bool JumpInput { get; private set; }

	public float CurrentSpeed { get; private set; }

	public bool CanRun { get; private set; }

	public bool IsRunning { get; private set; }

	public bool CanCrouch { get; private set; }

	public bool IsCrouched { get; private set; }

	public bool CanJump { get; private set; }

	public void Initialize(Character character)
	{
		Character = character;
		CanJump = true;
		CanRun = true;
		CanCrouch = true;
		PreviouslyGrounded = true;
	}

	public void UpdateInput()
	{
		GetJumpInput(Character.JumpInput);
		GetCrouchInput(Character.CrouchInput);
		GetMovementInput(Character.MoveXInput, Character.MoveYInput, Character.RunInput);
		CurrentSpeed *= 0.1f;
	}

	private void GetJumpInput(bool jumpInput)
	{
		if (JumpInput || !Character.Controller.isGrounded || !CanJump)
		{
			return;
		}
		JumpInput = jumpInput;
		if (JumpInput)
		{
			if (IsRunning && MovementInput.y > 0f)
			{
				m_ActiveGravity += m_JumpSpeed * 0.11f;
			}
			else
			{
				m_ActiveGravity += m_JumpSpeed * 0.1f;
			}
		}
		JumpInput = false;
	}

	private void GetCrouchInput(bool crouchInput)
	{
		if (!CrouchInput && Character.Controller.isGrounded && CanCrouch)
		{
			CrouchInput = crouchInput;
			if (CrouchInput)
			{
				IsCrouched = !IsCrouched;
			}
			CrouchInput = false;
		}
	}

	private void GetMovementInput(float moveX, float moveY, bool run)
	{
		float x = MovementInput.x;
		float num = MovementInput.y;
		if (Character.Controller.isGrounded)
		{
			x = moveX;
			num = moveY;
		}
		IsRunning = num > 0f && CanRun && run;
		if (IsRunning && IsCrouched)
		{
			IsCrouched = false;
		}
		if (IsCrouched)
		{
			IsRunning = false;
		}
		CurrentSpeed = (IsRunning ? m_RunSpeed : m_MoveSpeed);
		if (IsCrouched)
		{
			CurrentSpeed = m_MoveSpeed / 2f;
		}
		MovementInput = new Vector2(x, num);
		if (MovementInput.magnitude > 1f || MovementInput.magnitude < -1f)
		{
			MovementInput.Normalize();
		}
	}

	public void Update()
	{
		GetMovement();
		GetPhysics();
		GetGrounding();
		GetCrouch();
	}

	private void GetMovement()
	{
		if (Character.Controller.enabled)
		{
			Vector3 vector = Character.transform.forward * MovementInput.y + Character.transform.right * MovementInput.x;
			m_MoveDirection.x = vector.x;
			m_MoveDirection.z = vector.z;
			float y = MoveDirection.y;
			m_MoveDirection *= CurrentSpeed;
			m_MoveDirection.y = y;
			bool flag = false;
			if (m_ActiveGravity <= 0f && Character.Controller.isGrounded)
			{
				flag = true;
			}
			if (flag)
			{
				m_ActiveGravity = 0f;
			}
			else
			{
				m_ActiveGravity -= m_Gravity;
				m_MoveDirection.y = m_ActiveGravity;
			}
			CollisionFlags collisionFlags = Character.Controller.Move(MoveDirection);
			if (m_ActiveGravity < 0f)
			{
				collisionFlags = Character.Controller.Move(Vector3.up * -0.01f);
			}
			if (collisionFlags == CollisionFlags.Above && m_ActiveGravity > 0f)
			{
				m_ActiveGravity = 0f;
			}
		}
	}

	private void GetPhysics()
	{
		if (!(m_ExternalForce.magnitude <= 0f))
		{
			Character.Controller.Move(m_ExternalForce * Time.fixedDeltaTime);
			m_ExternalForce = Vector3.MoveTowards(m_ExternalForce, Vector3.zero, 0.95f);
		}
	}

	private void GetGrounding()
	{
		if (Character.Controller.isGrounded && !PreviouslyGrounded && m_ActiveGravity <= 0f)
		{
			m_ActiveGravity = 0f;
			m_ExternalForce = Vector3.zero;
		}
		PreviouslyGrounded = Character.Controller.isGrounded;
	}

	private void GetCrouch()
	{
		if (Character.Controller.isGrounded && CrouchInput)
		{
			IsCrouched = !IsCrouched;
		}
	}

	public void CancelMovement()
	{
		MovementInput = Vector3.zero;
		CurrentSpeed = 0f;
		m_ActiveGravity = 0f;
		m_ExternalForce = Vector3.zero;
		CrouchInput = false;
		JumpInput = false;
	}

	public void ForceStand()
	{
		IsCrouched = false;
		CrouchInput = false;
	}

	public void AddForce(Vector3 force)
	{
		m_ExternalForce += force;
	}

	public void SetMoveSpeed(float speed)
	{
		m_MoveSpeed = speed;
	}

	public void SetRunSpeed(float speed)
	{
		m_RunSpeed = speed;
	}

	protected override void OnDisposed()
	{
		Character = null;
		base.OnDisposed();
	}
}
