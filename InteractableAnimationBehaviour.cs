using UnityEngine;
using UnityEngine.Playables;

public class InteractableAnimationBehaviour : PlayableBehaviour
{
	public InteractableAnimation InteractableAnimation;

	public bool OnEnter = true;

	public bool OnExit;

	public bool DoLock;

	public bool DoUnlock;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (Application.isPlaying)
		{
			if (OnEnter)
			{
				InteractableAnimation.SetLocked(isLocked: true);
				InteractableAnimation.SetActive(active: false);
				InteractableAnimation.Enter();
			}
			else if (OnExit)
			{
				InteractableAnimation.Exit();
				InteractableAnimation.SetLocked(isLocked: false);
			}
			else if (DoLock)
			{
				InteractableAnimation.SetLocked(isLocked: true);
				InteractableAnimation.SetActive(active: false);
			}
			else if (DoUnlock)
			{
				InteractableAnimation.SetLocked(isLocked: false);
			}
		}
	}
}
