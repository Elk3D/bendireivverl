using UnityEngine;

[DefaultExecutionOrder(200)]
public class ActiveSetter : JMonoBehaviour
{
	[Header("Setters")]
	[SerializeField]
	private bool m_InitializeOnStart = true;

	[SerializeField]
	private bool m_ActiveOnStart;

	[Header("Setters")]
	[SerializeField]
	private GameObject[] m_Activated;

	[SerializeField]
	private GameObject[] m_Deactivated;

	[SerializeField]
	private GameObject[] m_ForceComplete;

	private bool m_IsForceComplete;

	public bool IsActive { get; private set; }

	public override void Awake()
	{
		if (!m_InitializeOnStart)
		{
			InitializeOnStart();
		}
	}

	public override void Start()
	{
		if (m_InitializeOnStart)
		{
			InitializeOnStart();
		}
	}

	private void InitializeOnStart()
	{
		if (!m_IsForceComplete)
		{
			SetActive(m_ActiveOnStart);
		}
	}

	public void SetActiveTrue()
	{
		SetActive(active: true);
	}

	public void SetActiveFalse()
	{
		SetActive(active: false);
	}

	public void SetActive(bool active)
	{
		IsActive = active;
		if (m_Activated != null)
		{
			for (int i = 0; i < m_Activated.Length; i++)
			{
				GameObject gameObject = m_Activated[i];
				if (gameObject != null)
				{
					gameObject.SetActive(IsActive);
				}
			}
		}
		if (m_Deactivated == null)
		{
			return;
		}
		for (int j = 0; j < m_Deactivated.Length; j++)
		{
			GameObject gameObject2 = m_Deactivated[j];
			if (gameObject2 != null)
			{
				gameObject2.SetActive(!IsActive);
			}
		}
	}

	public void ForceComplete()
	{
		if (m_ForceComplete != null)
		{
			for (int i = 0; i < m_ForceComplete.Length; i++)
			{
				GameObject gameObject = m_ForceComplete[i];
				if (gameObject != null)
				{
					gameObject.SetActive(value: false);
				}
			}
		}
		SetActiveTrue();
		m_IsForceComplete = true;
	}
}
