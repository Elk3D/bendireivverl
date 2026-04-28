using UnityEngine;

public class AnimationCycler : JMonoBehaviour
{
	[SerializeField]
	private float m_TimerLimit = 5f;

	[SerializeField]
	private CharacterContent m_Content;

	[SerializeField]
	private AnimationClip[] m_AnimationClips;

	private bool m_IsActive;

	private float m_Timer;

	private int m_Index;

	private bool m_IsDisabled;

	public override void OnEnable()
	{
		SetActive(active: true);
	}

	public override void OnDisable()
	{
		SetActive(active: false);
	}

	private void Update()
	{
		if (!m_IsActive || m_IsDisabled || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (m_Timer >= m_TimerLimit)
		{
			if (m_IsActive)
			{
				m_IsActive = false;
				m_Timer = 0f;
				AnimationClip animationClip = m_AnimationClips[m_Index];
				animationClip.name = "Interact";
				m_Content.UpdateClipOverride(animationClip);
				m_Content.SetAnimationTrigger("Interact");
			}
		}
		else
		{
			m_Timer += Time.deltaTime;
		}
	}

	public void SetActive(bool active)
	{
		m_IsActive = active;
	}

	public void Disable()
	{
		SetActive(active: false);
		m_IsDisabled = true;
	}

	public void AnimationComplete()
	{
		if (!m_IsDisabled)
		{
			m_Index++;
			if (m_Index >= m_AnimationClips.Length)
			{
				m_Index = 0;
			}
			if (base.gameObject.activeSelf)
			{
				m_IsActive = true;
			}
			m_Timer = 0f;
		}
	}
}
