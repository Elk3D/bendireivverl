using DG.Tweening;
using UnityEngine;

public class HittableWobble : Hittable
{
	[SerializeField]
	private Transform m_Pivot;

	[SerializeField]
	private float m_Value = 10f;

	[SerializeField]
	private float m_Duration = 1f;

	protected override bool InternalHit(RaycastHit hit)
	{
		m_Pivot.DOKill();
		m_Pivot.localEulerAngles = Vector3.zero;
		m_Pivot.DOPunchRotation(Random.insideUnitSphere.normalized * m_Value, m_Duration).SetEase(Ease.InOutElastic);
		return true;
	}

	protected override void OnDisposed()
	{
		m_Pivot.DOKill();
		base.OnDisposed();
	}
}
