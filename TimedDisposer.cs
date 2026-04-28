using UnityEngine;

public class TimedDisposer : JMonoBehaviour
{
	[SerializeField]
	private float m_TimeDelay;

	public override void OnEnable()
	{
		base.OnEnable();
		Invoke("Dispose", m_TimeDelay);
	}
}
