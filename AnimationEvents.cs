using UnityEngine;

public class AnimationEvents : JMonoBehaviour
{
	public const string TRIGGER_FUNCTION_NAME = "TriggerEventCommand";

	[SerializeField]
	private MonoBehaviour m_CommandReciever;

	[SerializeField]
	private CharacterAnimationEventData[] m_AnimationEventData;

	private Animator m_Animator;

	public override void Start()
	{
		if (m_Animator == null)
		{
			m_Animator = GetComponent<Animator>();
		}
		if (m_AnimationEventData == null)
		{
			return;
		}
		CharacterAnimationEventData[] animationEventData = m_AnimationEventData;
		foreach (CharacterAnimationEventData characterAnimationEventData in animationEventData)
		{
			foreach (AnimationEventCommand eventCommand in characterAnimationEventData.AnimationEventCommands.EventCommands)
			{
				AnimationEventUtility.AddAnimationEvent(ref m_Animator, characterAnimationEventData.Clip.name, "TriggerEventCommand", eventCommand.Frame, eventCommand.CommandName);
			}
		}
	}

	public void TriggerEventCommand(AnimationEvent animationEvent)
	{
		SendCommand(animationEvent.stringParameter);
	}

	private void SendCommand(string name)
	{
		if (name != string.Empty)
		{
			m_CommandReciever?.SendMessage(name, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void SetReciever(MonoBehaviour reciever)
	{
		m_CommandReciever = reciever;
	}

	protected override void OnDisposed()
	{
		m_Animator = null;
		base.OnDisposed();
	}
}
