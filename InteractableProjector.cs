using DG.Tweening;
using UnityEngine;

public class InteractableProjector : InteractableInputDisplay
{
	[Header("Reel Options")]
	[SerializeField]
	private Transform[] m_Reels;

	public bool IsOn { get; private set; }

	protected override void OnInternalInteract(Vector3 origin, RaycastHit hit, object sender = null)
	{
		base.OnInternalInteract(origin, hit, sender);
		IsOn = !IsOn;
		if (IsOn)
		{
			TurnReels();
		}
		else
		{
			StopReels();
		}
	}

	public void TurnReels()
	{
		SetInteractionType(InteractionType.INTERACTION_TURN_OFF);
		float num = -360f;
		for (int i = 0; i < m_Reels.Length; i++)
		{
			Transform target = m_Reels[i];
			target.DOKill();
			target.DOLocalRotate(new Vector3(num, 0f, 0f), 2f, RotateMode.LocalAxisAdd).SetLoops(-1).SetEase(Ease.Linear);
			num *= -1f;
		}
	}

	private void StopReels()
	{
		SetInteractionType(InteractionType.INTERACTION_TURN_ON);
		KillReelTweens();
	}

	private void KillReelTweens()
	{
		for (int i = 0; i < m_Reels.Length; i++)
		{
			m_Reels[i].DOKill();
		}
	}

	protected override void OnDisposed()
	{
		KillReelTweens();
		base.OnDisposed();
	}
}
