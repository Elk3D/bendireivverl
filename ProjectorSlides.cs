using System;
using DG.Tweening;
using UnityEngine;

public class ProjectorSlides : JMonoBehaviour
{
	[Header("ID")]
	[SerializeField]
	private int m_ProjectorID;

	[Header("Index")]
	[SerializeField]
	private int m_ID = 3;

	[Space]
	[SerializeField]
	private Interactable m_Interactable;

	[Space]
	[SerializeField]
	private GameObject m_Lights;

	[Header("Sprite Renderer")]
	[SerializeField]
	private SpriteRenderer m_Slide;

	[Header("Sprites")]
	[SerializeField]
	private Sprite[] m_Slides;

	[Header("Reels")]
	[SerializeField]
	private Transform m_BackReel;

	[SerializeField]
	private Transform m_FrontReel;

	private Sequence m_Sequence;

	private bool m_Isnteracted;

	public int ID => m_ProjectorID;

	public int Index => m_ID;

	public void Initialize()
	{
		if (m_Interactable != null)
		{
			m_Interactable.OnInteract -= HandleInteractableOnInteract;
			m_Interactable.OnInteract += HandleInteractableOnInteract;
		}
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		if (!m_Isnteracted)
		{
			NextSlide();
		}
	}

	private void NextSlide()
	{
		Disable();
		m_ID++;
		if (m_ID > 5)
		{
			m_ID = 1;
		}
		m_Slide.sprite = m_Slides[m_ID - 1];
		ResetSequence();
		m_Sequence.Insert(0f, m_BackReel.DOLocalRotate(new Vector3(40f, 0f, 0f), 0.4f, RotateMode.LocalAxisAdd).SetEase(Ease.InBack));
		m_Sequence.Insert(0f, m_FrontReel.DOLocalRotate(new Vector3(40f, 0f, 0f), 0.5f, RotateMode.LocalAxisAdd).SetEase(Ease.InBack));
		m_Sequence.OnComplete(Enable);
	}

	private void Disable()
	{
		m_Isnteracted = true;
		m_Lights.SetActive(value: false);
	}

	private void Enable()
	{
		m_Isnteracted = false;
		m_Lights.SetActive(value: true);
	}

	public void SetIndex(int index)
	{
		m_ID = index;
		if (m_ID > 5)
		{
			m_ID = 1;
		}
		m_Slide.sprite = m_Slides[m_ID - 1];
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
		if (m_Interactable != null)
		{
			m_Interactable.OnInteract -= HandleInteractableOnInteract;
		}
		base.OnDisposed();
	}
}
