using UnityEngine;
using UnityEngine.Events;

public class AnimationUnityEvent : JMonoBehaviour
{
	[SerializeField]
	private UnityEvent m_UnityEvent;

	public void AnimationComplete()
	{
		m_UnityEvent?.Invoke();
	}
}
