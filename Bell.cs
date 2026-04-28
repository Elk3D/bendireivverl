using System;
using DG.Tweening;
using UnityEngine;

public class Bell : JMonoBehaviour
{
	[SerializeField]
	private Transform m_BellTop;

	[SerializeField]
	private Interactable m_Interactable;

	private Sequence m_Sequence;

	public event EventHandler OnRing;

	public override void Start()
	{
		m_Interactable.OnInteract -= HandleInteractableOnInteract;
		m_Interactable.OnInteract += HandleInteractableOnInteract;
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		ResetSequence();
		m_Sequence.Insert(0f, m_BellTop.DOLocalMoveY(-0.03f, 0.05f).SetEase(Ease.Linear));
		m_Sequence.Insert(0.05f, m_BellTop.DOLocalMoveY(0f, 0.1f).SetEase(Ease.OutBounce));
		this.OnRing.Send(this);
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
		this.OnRing = null;
		KillSequence();
		base.OnDisposed();
	}
}
