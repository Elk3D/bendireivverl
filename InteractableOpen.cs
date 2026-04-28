using DG.Tweening;
using UnityEngine;

public class InteractableOpen : InteractableInputDisplay
{
	[Header("Open Options")]
	[SerializeField]
	private Transform m_Pivot;

	[SerializeField]
	private Vector3 m_OpenPivot;

	[SerializeField]
	private float m_Speed = 1f;

	[SerializeField]
	private Ease m_OpenEase;

	private Sequence m_Sequence;

	private Vector3 m_ClosedPivot;

	public override void Awake()
	{
		base.Awake();
		m_ClosedPivot = m_Pivot.localEulerAngles;
	}

	protected override void OnInternalInteract(Vector3 origin, RaycastHit hit, object sender = null)
	{
		base.OnInternalInteract(origin, hit, sender);
		m_Sequence?.Kill();
		m_Sequence = DOTween.Sequence();
		m_Sequence.Insert(0f, m_Pivot.DOLocalRotate(m_OpenPivot, m_Speed, RotateMode.LocalAxisAdd).SetEase(m_OpenEase));
	}

	protected override void OnDisposed()
	{
		m_Sequence?.Kill();
		m_Sequence = null;
		base.OnDisposed();
	}
}
