using UnityEngine;

public class AutoLoopAnimation : JMonoBehaviour
{
	[SerializeField]
	private Animator m_Animator;

	public void AnimationComplete()
	{
		m_Animator.ResetTrigger("ForceReset");
		m_Animator.SetTrigger("ForceReset");
	}
}
