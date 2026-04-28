using UnityEngine;

public class GenericAnimationEvents : JMonoBehaviour
{
	[SerializeField]
	protected MonoBehaviour m_FunctionReciever;

	[SerializeField]
	protected bool m_FireEvents = true;

	[SerializeField]
	protected bool m_DisableEvents;

	public Animator Animator { get; private set; }

	public void SetActive(bool active)
	{
		m_FireEvents = active;
	}

	public void SetReciever(MonoBehaviour reciever)
	{
		m_FunctionReciever = reciever;
	}

	public override void Start()
	{
		Animator = GetComponent<Animator>();
		if (Animator != null)
		{
			Animator.fireEvents = !m_DisableEvents;
		}
	}

	public void TriggerEvent(AnimationEvent animationEvent)
	{
		if (m_FireEvents)
		{
			SendFunction(animationEvent.stringParameter, animationEvent.floatParameter, animationEvent.intParameter);
		}
	}

	private void SendFunction(string name, float floatValue, int intValue)
	{
		if (m_FunctionReciever == null)
		{
			Debug.LogError("m_FunctionReciever is null on " + base.name);
		}
		else if (!(name == ""))
		{
			if (floatValue != 0f && intValue != 0)
			{
				object[] value = new object[2] { floatValue, intValue };
				m_FunctionReciever?.SendMessage(name, value, SendMessageOptions.DontRequireReceiver);
			}
			else if (floatValue != 0f)
			{
				m_FunctionReciever?.SendMessage(name, floatValue, SendMessageOptions.DontRequireReceiver);
			}
			else if (intValue != 0)
			{
				m_FunctionReciever?.SendMessage(name, intValue, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				m_FunctionReciever?.SendMessage(name, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	protected override void OnDisposed()
	{
		Animator = null;
		base.OnDisposed();
	}

	public void RumbleEvent(AnimationEvent animationEvent)
	{
		if (m_FireEvents)
		{
			float strength = float.Parse(animationEvent.stringParameter);
			GameManager.Instance.TriggerRumble(animationEvent.floatParameter, strength);
		}
	}
}
