using UnityEngine;

public class CharacterRotation : JDisposable
{
	[SerializeField]
	private float m_Sensitivity = 3f;

	private Quaternion m_CharacterTargetRotation;

	private bool m_IsRotationInitialized;

	private float m_RotationInput;

	public Character Character { get; private set; }

	public Quaternion ForcedRotation { get; private set; }

	public CharacterRotation(Character character)
	{
		Character = character;
		ResetRotation(Character.transform);
	}

	public void ForceRotation(Quaternion rotation)
	{
		m_CharacterTargetRotation = rotation;
	}

	public void SetRotation(Quaternion rotation)
	{
		ForcedRotation = rotation;
	}

	public void UpdateInput()
	{
		float rotateXInput = Character.RotateXInput;
		m_RotationInput = rotateXInput * m_Sensitivity;
	}

	public void Update()
	{
		if (!IsNullRotation(m_RotationInput))
		{
			m_CharacterTargetRotation *= Quaternion.Euler(0f, m_RotationInput, 0f);
			Character.transform.rotation = m_CharacterTargetRotation;
		}
		else
		{
			Character.transform.rotation = m_CharacterTargetRotation;
		}
	}

	public void ResetRotation()
	{
		ResetRotation(Character.transform);
	}

	public void ResetRotation(Transform character)
	{
		m_CharacterTargetRotation.eulerAngles = new Vector3(0f, character.localRotation.eulerAngles.y, 0f);
	}

	private bool IsNullRotation(float horizontal)
	{
		if (horizontal == 0f)
		{
			return true;
		}
		if (!m_IsRotationInitialized)
		{
			m_IsRotationInitialized = true;
			return true;
		}
		return false;
	}

	public void SetSensitivity(float sensitivity)
	{
		m_Sensitivity = sensitivity * 5f;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
