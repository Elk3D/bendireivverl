using UnityEngine.Events;

public class TimelineUnityEvent : JMonoBehaviour
{
	public UnityEvent UnityEvent;

	public void Action()
	{
		if (UnityEvent != null)
		{
			UnityEvent.Invoke();
		}
	}
}
