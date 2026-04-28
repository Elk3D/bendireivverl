using System;
using DG.Tweening;
using UnityEngine;

public class PuzzleWheel : JMonoBehaviour
{
	private static Vector3 WHEEL_ROTATION = new Vector3(-60f, 0f, 0f);

	[SerializeField]
	private Interactable m_Interactable;

	private Sequence m_Sequence;

	public int ID { get; private set; }

	public void Initialize(int id)
	{
		for (ID = id; ID == id; ID = UnityEngine.Random.Range(1, 6))
		{
		}
		if (ID > 1)
		{
			m_Interactable.transform.localEulerAngles += WHEEL_ROTATION * (ID - 1);
		}
		m_Interactable.OnInteract -= HandleInteractableOnInteract;
		m_Interactable.OnInteract += HandleInteractableOnInteract;
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		m_Interactable.OnInteract -= HandleInteractableOnInteract;
		GameManager.Instance.Player.Interaction.ResetInteraction();
		ResetSequence();
		m_Sequence.Insert(0f, m_Interactable.transform.DOLocalRotate(WHEEL_ROTATION, 0.3f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutBack));
		m_Sequence.OnComplete(WheelOnComplete);
	}

	private void WheelOnComplete()
	{
		ID++;
		if (ID > 6)
		{
			ID = 1;
		}
		m_Interactable.OnInteract -= HandleInteractableOnInteract;
		m_Interactable.OnInteract += HandleInteractableOnInteract;
		m_Interactable.ResetAction();
		m_Interactable.SetActive(active: true);
	}

	public void SetActive(bool active)
	{
		m_Interactable.SetActive(active);
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			m_Sequence.Kill();
			m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		KillSequence();
		base.OnDisposed();
	}
}
